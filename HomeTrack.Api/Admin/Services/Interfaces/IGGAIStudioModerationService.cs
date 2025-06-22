using HomeTrack.Domain;

namespace HomeTrack.Application.Interface
{
  public class AIStudioModerationResult
  {
    public bool IsPotentiallyHarmful { get; set; }
    public string? HarmCategory { get; set; }
    public float ConfidenceScore { get; set; }
    public string? Feedback { get; set; }
    public string? RawApiResponse { get; set; }
  }

  public interface IGoogleAIStudioModerationService
  {
    Task<AIStudioModerationResult> ModerateItemContentAsync(Item item, IEnumerable<Tag> associatedTags);
  }
}