namespace TimboLearn.Infrastructure.AI;

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
