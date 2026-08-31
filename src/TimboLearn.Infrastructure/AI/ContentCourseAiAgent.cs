using Microsoft.Extensions.Logging;

namespace TimboLearn.Infrastructure.AI;

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
