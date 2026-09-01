using TimboLearn.Infrastructure.Persistence;
using TimboLearn.Infrastructure.AI;
using Microsoft.EntityFrameworkCore;
using TimboLearn.Infrastructure;

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
        // Validate input
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ValidationException("Invalid Course Data", new Dictionary<string, object?>
            {
                { "title", "Course title is required" }
            });
        }

        if (estimatedDurationMinutes <= 0)
        {
            throw new ValidationException("Invalid Course Data", new Dictionary<string, object?>
            {
                { "estimatedDurationMinutes", "Duration must be greater than 0 minutes" }
            });
        }

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
        // Validate course exists
        var contentCourse = await _dbContext.ContentCourses.FindAsync([contentCourseId], cancellationToken);
        if (contentCourse == null)
        {
            throw new NotFoundException("ContentCourse", $"Content course with ID {contentCourseId} was not found");
        }

        // Validate at least one target is provided
        if (!targetUserId.HasValue && !targetTeamId.HasValue)
        {
            throw new ValidationException("Invalid Assignment", new Dictionary<string, object?>
            {
                { "targetUserId", "Either targetUserId or targetTeamId must be provided" }
            });
        }

        // Validate user exists if provided
        if (targetUserId.HasValue)
        {
            var user = await _dbContext.Users.FindAsync([targetUserId.Value], cancellationToken);
            if (user == null)
            {
                throw new NotFoundException("User", $"User with ID {targetUserId.Value} was not found");
            }
        }

        // Validate team exists if provided
        if (targetTeamId.HasValue)
        {
            var team = await _dbContext.Teams.FindAsync([targetTeamId.Value], cancellationToken);
            if (team == null)
            {
                throw new NotFoundException("Team", $"Team with ID {targetTeamId.Value} was not found");
            }
        }

        // Validate due date is in the future if provided
        if (dueDateUtc.HasValue && dueDateUtc.Value <= DateTime.UtcNow)
        {
            throw new ValidationException("Invalid Assignment", new Dictionary<string, object?>
            {
                { "dueDateUtc", "Due date must be in the future" }
            });
        }

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
