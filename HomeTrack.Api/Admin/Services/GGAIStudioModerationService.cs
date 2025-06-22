using System.Text;
using System.Text.Json;
using HomeTrack.Application.Interface;

using HomeTrack.Domain;

namespace HomeTrack.Application.Services
{
  public class GoogleAIStudioModerationService : IGoogleAIStudioModerationService
  {
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _modelName;
    private readonly IWebHostEnvironment _hostEnvironment;
    private const string BaseUrl =
      "https://generativelanguage.googleapis.com/v1beta/models/";

    public GoogleAIStudioModerationService(IConfiguration configuration, HttpClient httpClient, IWebHostEnvironment hostEnvironment)
    {
      _apiKey = configuration["GoogleAIStudio:ApiKey"] ?? throw new ArgumentNullException("GoogleAIStudio:ApiKey not configured.");
      _modelName = configuration["GoogleAIStudio:ModelName"] ?? "gemini-2.0-flash";
      _httpClient = httpClient; // Inject HttpClient
      _hostEnvironment = hostEnvironment;
    }

    public async Task<AIStudioModerationResult> ModerateItemContentAsync(Item item,
      IEnumerable<Tag> associatedTags)
    {
      var requestUrl = $"{BaseUrl}{_modelName}:generateContent?key={_apiKey}";

      var tagNames = associatedTags?.Select(t => t.Name).ToList() ?? new List<string>();

      var contentParts = new List<object>();

      var imageLocalPath = item.ImageUrl;

      var promptBuilder = new StringBuilder();
      promptBuilder.AppendLine("Bạn là một chuyên gia kiểm duyệt nội dung. Hãy đánh giá nội dung sau đây để xác định xem nó có chứa bất kỳ yếu tố nào không phù hợp, vi phạm chính sách, hoặc có hại không (ví dụ: ngôn từ thù địch, nội dung người lớn, bạo lực, thông tin sai lệch, quảng cáo bất hợp pháp, v.v.). Trả lời bằng tiếng Việt");
      promptBuilder.AppendLine("Chỉ trả lời bằng một đối tượng JSON với các trường sau (không có markdown code block ```json ... ``` bao quanh):");
      promptBuilder.AppendLine("{");
      promptBuilder.AppendLine("  \"isPotentiallyHarmful\": boolean,"); // true hoặc false
      promptBuilder.AppendLine("  \"harmCategory\": \"string\","); // Loại vi phạm nếu có, ví dụ: \"HATE_SPEECH\", \"ADULT_CONTENT\", \"NONE\"
      promptBuilder.AppendLine("  \"confidenceScore\": number,"); // Điểm tin cậy từ 0.0 đến 1.0
      promptBuilder.AppendLine("  \"feedback\": \"string\""); // Giải thích ngắn gọn
      promptBuilder.AppendLine("}");
      promptBuilder.AppendLine("\nNội dung cần đánh giá:");
      promptBuilder.AppendLine($"- Tên đồ vật: \"{item.Name}\"");

      if (!string.IsNullOrEmpty(item.Description))
      {
        promptBuilder.AppendLine($"- Mô tả: \"{item.Description}\"");
      }
      if (tagNames.Any())
      {
        promptBuilder.AppendLine($"- Các tag liên quan: \"{string.Join(", ", tagNames)}\"");
      }

      contentParts.Add(new { text = promptBuilder.ToString() });

      // Part 2: Dữ liệu ảnh (nếu item.ImageUrl có giá trị)
      if (!string.IsNullOrEmpty(item.ImageUrl))
      {
        try
        {
          // ImageUrl được lưu là đường dẫn tương đối từ wwwroot, ví dụ: /uploads/items/1/image.jpg
          var fullImagePath = Path.Combine(_hostEnvironment.WebRootPath ?? "wwwroot", item.ImageUrl.TrimStart('/'));

          if (File.Exists(fullImagePath))
          {
            byte[] imageBytes = await File.ReadAllBytesAsync(fullImagePath);
            string base64Image = Convert.ToBase64String(imageBytes);
            string mimeType = GetMimeType(fullImagePath);

            contentParts.Add(new
            {
              inlineData = new
              {
                mimeType = mimeType,
                data = base64Image
              }
            });
            Console.WriteLine($"Đã thêm ảnh vào payload: {item.ImageUrl}");
          }
          else
          {
            Console.WriteLine($"Ảnh không tồn tại tại đường dẫn: {fullImagePath} (từ item.ImageUrl: {item.ImageUrl})");
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Lỗi khi đọc hoặc xử lý file ảnh '{item.ImageUrl}': {ex.Message}");
        }
      }

      var payload = new
      {
        contents = new[]
                {
                    new { parts = contentParts.ToArray() }
                },
        // (Tùy chọn) Thêm generationConfig nếu cần kiểm soát output
        generationConfig = new
        {
          temperature = 0.2, // Giảm nhiệt độ để kết quả nhất quán hơn
          maxOutputTokens = 256, // Giới hạn output
          // responseMimeType = "application/json" // Yêu cầu output là JSON
        }
      };

      var jsonPayload = JsonSerializer.Serialize(payload);
      var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

      try
      {
        var response = await _httpClient.PostAsync(requestUrl, httpContent);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
          // Phân tích phản hồi JSON từ AI Studio
          // Phản hồi thường có cấu trúc như:
          // { "candidates": [ { "content": { "parts": [ { "text": "{ \"isPotentiallyHarmful\": ... }" } ], "role": "model" } } ] }
          using var jsonDoc = JsonDocument.Parse(responseString);
          var candidates = jsonDoc.RootElement.TryGetProperty("candidates", out var c) ? c : default;
          if (candidates.ValueKind == JsonValueKind.Array && candidates.GetArrayLength() > 0)
          {
            var firstCandidate = candidates[0];
            var content = firstCandidate.TryGetProperty("content", out var co) ? co : default;
            var parts = content.TryGetProperty("parts", out var p) ? p : default;
            if (parts.ValueKind == JsonValueKind.Array && parts.GetArrayLength() > 0)
            {
              var textPart = parts[0].TryGetProperty("text", out var t) ? t.GetString() : null;
              if (!string.IsNullOrEmpty(textPart))
              {
                try
                {
                  // Loại bỏ các ký tự markdown code block nếu có
                  var cleanedJsonText = textPart.Trim().TrimStart('`', 'j', 's', 'o', 'n').TrimEnd('`');

                  var moderationResult = JsonSerializer.Deserialize<AIStudioModerationResult>(cleanedJsonText,
                      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                  if (moderationResult != null)
                  {
                    moderationResult.RawApiResponse = textPart; // Lưu lại để debug
                    return moderationResult;
                  }
                }
                catch (JsonException jsonEx)
                {
                  Console.WriteLine($"Lỗi parse JSON từ Google AI Studio: {jsonEx.Message}. Response text: {textPart}");
                  return new AIStudioModerationResult { IsPotentiallyHarmful = true, Feedback = $"Lỗi định dạng JSON: {textPart}", RawApiResponse = responseString };
                }
              }
            }
          }
          Console.WriteLine($"Không tìm thấy nội dung hợp lệ trong phản hồi của Google AI Studio: {responseString}");
          return new AIStudioModerationResult { IsPotentiallyHarmful = false, Feedback = "Không có nội dung hợp lệ từ AI Studio.", RawApiResponse = responseString };
        }
        else
        {
          Console.WriteLine($"Lỗi từ Google AI Studio API: {response.StatusCode} - {responseString}");
          return new AIStudioModerationResult { IsPotentiallyHarmful = true, HarmCategory = "API_ERROR", Feedback = $"Lỗi API: {response.StatusCode} - {responseString}", RawApiResponse = responseString };
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Lỗi ngoại lệ khi gọi Google AI Studio API: {ex.ToString()}");
        return new AIStudioModerationResult { IsPotentiallyHarmful = true, HarmCategory = "EXCEPTION", Feedback = $"Lỗi hệ thống: {ex.Message}", RawApiResponse = ex.ToString() };
      }
    }

    private string GetMimeType(string filePath)
    {
      var extension = Path.GetExtension(filePath).ToLowerInvariant();
      return extension switch
      {
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        _ => "application/octet-stream",
      };
    }
  }
}
