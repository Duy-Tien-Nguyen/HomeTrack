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

    public async Task<AISuggestedTagResult> SuggestTagsForImageAsync(Stream imageStream, string mimeType, string? itemContextText = null)
    {
      var requestUrl = $"{BaseUrl}{_modelName}:generateContent?key={_apiKey}";
      var contentParts = new List<object>();

      var promptBuilder = new StringBuilder();
      promptBuilder.AppendLine("Phân tích hình ảnh sau và gợi ý một danh sách các tag (từ khóa) mô tả các đối tượng, cảnh vật, hoặc khái niệm chính trong ảnh. Trả lời bằng tiếng Việt.");
      promptBuilder.AppendLine("Chỉ trả lời bằng một đối tượng JSON với một trường duy nhất là \"suggestedTags\", là một mảng các chuỗi tag (không có markdown code block ```json ... ``` bao quanh):");
      promptBuilder.AppendLine("{ \"suggestedTags\": [\"tag1\", \"tag2\", \"tag3\", ...] }");
      if (!string.IsNullOrEmpty(itemContextText))
      {
        promptBuilder.AppendLine($"\nNgữ cảnh bổ sung (tên đồ vật, mô tả): \"{itemContextText}\"");
      }
      contentParts.Add(new { text = promptBuilder.ToString() });

      if (imageStream == null || imageStream.Length == 0)
      {
        return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = "Dữ liệu ảnh (stream) không được để trống." };
      }
      if (string.IsNullOrEmpty(mimeType))
      {
        return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = "Mime type của ảnh là bắt buộc." };
      }

      try
      {
        // Đọc Stream thành byte array rồi chuyển sang Base64
        using var memoryStream = new MemoryStream();
        await imageStream.CopyToAsync(memoryStream); // Đảm bảo imageStream được truyền vào có thể đọc được
        memoryStream.Position = 0; // Reset vị trí stream nếu nó đã được đọc trước đó
        byte[] imageBytes = memoryStream.ToArray();
        string base64Image = Convert.ToBase64String(imageBytes);

        contentParts.Add(new { inlineData = new { mimeType = mimeType, data = base64Image } });
        Console.WriteLine($"Đã thêm ảnh từ stream vào payload để gợi ý tag. MimeType: {mimeType}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Lỗi khi xử lý image stream để gợi ý tag: {ex.Message}");
        return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = $"Lỗi xử lý dữ liệu ảnh: {ex.Message}" };
      }

      var payload = new
      {
        contents = new[] { new { parts = contentParts.ToArray() } },
        generationConfig = new { temperature = 0.4, maxOutputTokens = 256 }
      };

      var jsonPayload = JsonSerializer.Serialize(payload);
      var httpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

      try
      {
        var response = await _httpClient.PostAsync(requestUrl, httpContent);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
          using var jsonDoc = JsonDocument.Parse(responseString);
          JsonElement candidates = default;
          if (jsonDoc.RootElement.TryGetProperty("candidates", out candidates) && candidates.ValueKind == JsonValueKind.Array && candidates.GetArrayLength() > 0)
          {
            var firstCandidate = candidates[0];
            if (firstCandidate.TryGetProperty("finishReason", out var finishReasonElement) && finishReasonElement.GetString() == "SAFETY")
            {
              // Xử lý nếu bị bộ lọc an toàn chặn
              string safetyFeedback = "Nội dung bị chặn bởi bộ lọc an toàn của AI khi gợi ý tag. ";
              if (firstCandidate.TryGetProperty("safetyRatings", out var safetyRatingsElement) && safetyRatingsElement.ValueKind == JsonValueKind.Array)
              {
                foreach (var rating in safetyRatingsElement.EnumerateArray())
                {
                  var category = rating.TryGetProperty("category", out var catEl) ? catEl.GetString() : "UNKNOWN_CATEGORY";
                  var probability = rating.TryGetProperty("probability", out var probEl) ? probEl.GetString() : "UNKNOWN_PROBABILITY";
                  safetyFeedback += $"{category}: {probability}. ";
                }
              }
              return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = safetyFeedback.Trim(), RawApiResponse = responseString };
            }

            if (firstCandidate.TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var partsElement) &&
                partsElement.ValueKind == JsonValueKind.Array && partsElement.GetArrayLength() > 0)
            {
              var textPart = partsElement[0].TryGetProperty("text", out var textElement) ? textElement.GetString() : null;
              if (!string.IsNullOrEmpty(textPart))
              {
                try
                {
                  var cleanedJsonText = textPart.Trim().TrimStart('`', 'j', 's', 'o', 'n').TrimEnd('`').Trim();
                  var tempResult = JsonSerializer.Deserialize<TemporaryTagSuggestionResponse>(cleanedJsonText,
                      new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                  if (tempResult?.SuggestedTags != null)
                  {
                    return new AISuggestedTagResult { IsSuccess = true, SuggestedTags = tempResult.SuggestedTags, RawApiResponse = textPart };
                  }
                }
                catch (JsonException jsonEx)
                {
                  Console.WriteLine($"Lỗi parse JSON gợi ý tag: {jsonEx.Message}. Response text: {textPart}");
                  return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = $"Lỗi định dạng JSON từ AI: {textPart}", RawApiResponse = responseString };
                }
              }
            }
          }
          Console.WriteLine($"Không tìm thấy nội dung hợp lệ trong phản hồi của Google AI Studio cho gợi ý tag: {responseString}");
          return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = "Không có nội dung hợp lệ từ AI Studio cho gợi ý tag.", RawApiResponse = responseString };
        }
        else
        {
          string detailedError = ExtractErrorMessage(responseString) ?? $"Lỗi API: {response.StatusCode}";
          Console.WriteLine($"Lỗi từ Google AI Studio API (gợi ý tag): {response.StatusCode} - {detailedError}");
          return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = detailedError, RawApiResponse = responseString };
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Lỗi ngoại lệ khi gọi Google AI Studio API (gợi ý tag): {ex.ToString()}");
        return new AISuggestedTagResult { IsSuccess = false, ErrorMessage = $"Lỗi hệ thống: {ex.Message}", RawApiResponse = ex.ToString() };
      }
    }

    // Lớp tạm thời để deserialize phản hồi JSON cho gợi ý tag
    private class TemporaryTagSuggestionResponse
    {
      public List<string>? SuggestedTags { get; set; }
    }

    private string? ExtractErrorMessage(string jsonResponse)
    {
      try
      {
        using var jsonDoc = JsonDocument.Parse(jsonResponse);
        if (jsonDoc.RootElement.TryGetProperty("error", out var errorElement) &&
            errorElement.TryGetProperty("message", out var messageElement))
        {
          return messageElement.GetString();
        }
      }
      catch { /* Bỏ qua */ }
      return null;
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
