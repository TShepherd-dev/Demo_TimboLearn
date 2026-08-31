using TimboLearn.Infrastructure.Persistence;
using TimboLearn.Infrastructure.AI;
using Microsoft.EntityFrameworkCore;

namespace TimboLearn.Features.ContentCourses;

public interface IContentCourseService
{
    Task<CreateContentCourseResponse> CreateContentCourseAsync(
        string title,
        string description,
        int estimatedDurationMinutes,
        bool isPublished,
        CancellationToken cancellationToken = default);

    Task<AssignContentCourseResponse> AssignContentCourseAsync(
        int contentCourseId,
        int? targetUserId,
        int? targetTeamId,
        DateTime? dueDateUtc,
        CancellationToken cancellationToken = default);

    Task<GeneratedContentCourseResult?> GenerateContentCourseWithAiAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken cancellationToken = default);

    Task<CreateContentCourseResponse> SaveGeneratedContentCourseAsync(
        GeneratedContentCourseResult result,
        CancellationToken cancellationToken = default);
}

public class ContentCourseService : IContentCourseService
{
    private readonly TimboLearnDbContext _dbContext;
    private readonly IContentCourseAiAgent _aiAgent;

    public ContentCourseService(
        TimboLearnDbContext dbContext,
        IContentCourseAiAgent aiAgent)
    {
        _dbContext = dbContext;
        _aiAgent = aiAgent;
    }

    public async Task<CreateContentCourseResponse> CreateContentCourseAsync(
        string title,
        string description,
        int estimatedDurationMinutes,
        bool isPublished,
        CancellationToken cancellationToken = default)
    {
        var contentCourse = new ContentCourse
        {
            Title = title,
            Description = description,
            EstimatedDurationMinutes = estimatedDurationMinutes,
            IsPublished = isPublished,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.ContentCourses.Add(contentCourse);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateContentCourseResponse(
            contentCourse.Id,
            contentCourse.Title,
            contentCourse.Description,
            contentCourse.EstimatedDurationMinutes,
            contentCourse.IsPublished,
            contentCourse.CreatedAtUtc
        );
    }

    public async Task<AssignContentCourseResponse> AssignContentCourseAsync(
        int contentCourseId,
        int? targetUserId,
        int? targetTeamId,
        DateTime? dueDateUtc,
        CancellationToken cancellationToken = default)
    {
        var contentCourse = await _dbContext.ContentCourses.FindAsync(new object[] { contentCourseId }, cancellationToken);
        if (contentCourse == null)
            throw new InvalidOperationException($"Content group {contentCourseId} not found");

        var assignment = new ContentCourseAssignment
        {
            ContentCourseId = contentCourseId,
            TargetUserId = targetUserId,
            TargetTeamId = targetTeamId,
            DueDateUtc = dueDateUtc,
            AssignedAtUtc = DateTime.UtcNow,
            Status = AssignmentStatus.NotStarted
        };

        _dbContext.ContentCourseAssignments.Add(assignment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AssignContentCourseResponse(
            assignment.Id,
            contentCourseId,
            targetUserId,
            targetTeamId,
            assignment.AssignedAtUtc,
            assignment.DueDateUtc
        );
    }

    public async Task<GeneratedContentCourseResult?> GenerateContentCourseWithAiAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken cancellationToken = default)
    {
        return await _aiAgent.DraftPlanAsync(prompt, desiredDurationMinutes, cancellationToken);
    }

    public async Task<CreateContentCourseResponse> SaveGeneratedContentCourseAsync(
        GeneratedContentCourseResult result,
        CancellationToken cancellationToken = default)
    {
        var contentCourse = new ContentCourse
        {
            Title = result.Title,
            Description = result.Description,
            EstimatedDurationMinutes = result.EstimatedDurationMinutes,
            IsPublished = false,
            CreatedAtUtc = DateTime.UtcNow
        };

        _dbContext.ContentCourses.Add(contentCourse);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateContentCourseResponse(
            contentCourse.Id,
            contentCourse.Title,
            contentCourse.Description,
            contentCourse.EstimatedDurationMinutes,
            contentCourse.IsPublished,
            contentCourse.CreatedAtUtc
        );
    }
}
