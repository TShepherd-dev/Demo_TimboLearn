using Microsoft.Extensions.Logging;

namespace TimboLearn.Infrastructure.AI;

/// <summary>
/// MOCK IMPLEMENTATION - Returns templated responses without calling real AI.
/// 
/// WHY MOCKED: This is intentional for a demo/portfolio project because:
/// 1. No truly free AI provider exists without signup/API keys
/// 2. Provider landscape changes rapidly (avoid dating the project)
/// 3. Demo must work offline/at conferences without external dependencies
/// 
/// TO INTEGRATE REAL AI: See docs/AI-Integration.md for complete examples
/// for Ollama (local/free), Google Gemini (free tier), and Azure OpenAI (enterprise).
/// Simply implement IContentCourseAiAgent and register in DI - architecture is ready.
/// </summary>
public class ContentCourseAiAgent : IContentCourseAiAgent
{
    private readonly ILogger<ContentCourseAiAgent> _logger;

    public ContentCourseAiAgent(ILogger<ContentCourseAiAgent> logger)
    {
        _logger = logger;
    }

    public Task<GeneratedContentCourseResult> DraftPlanAsync(
        string prompt, 
        int desiredDurationMinutes, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating content group for prompt: {Prompt}", prompt);

        var result = new GeneratedContentCourseResult(
            Title: $"AI-Generated Content: {prompt}",
            Description: $"Comprehensive content group covering {prompt}. This group is designed to be completed in approximately {desiredDurationMinutes} minutes.",
            Modules: new List<string>
            {
                $"Introduction to {prompt}",
                $"Fundamentals and Core Concepts",
                $"Best Practices for {prompt}",
                $"Advanced Techniques",
                $"Hands-on Lab: Applying {prompt}",
                $"Assessment: {prompt} Knowledge Check"
            },
            EstimatedDurationMinutes: desiredDurationMinutes
        );

        return Task.FromResult(result);
    }
}
