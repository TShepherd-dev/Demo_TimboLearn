namespace TimboLearn.Infrastructure.AI;

/// <summary>
/// Abstraction for AI-powered course generation.
/// 
/// Current implementation (ContentCourseAiAgent) is a MOCK that returns templated responses.
/// See docs/AI-Integration.md for why this is mocked and how to integrate real AI providers
/// (Ollama, Google Gemini, Azure OpenAI, etc.).
/// 
/// Architecture is designed for easy swapping - just implement this interface and register in DI.
/// </summary>
public interface IContentCourseAiAgent
{
    Task<GeneratedContentCourseResult> DraftPlanAsync(
        string prompt, 
        int desiredDurationMinutes, 
        CancellationToken cancellationToken = default);
}

public record GeneratedContentCourseResult(
    string Title,
    string Description,
    List<string> Modules,
    int EstimatedDurationMinutes
);
