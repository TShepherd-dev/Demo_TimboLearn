namespace TimboLearn.Features.ContentCourses;

public record CreateContentCourseRequest(
    string Title,
    string Description,
    int EstimatedDurationMinutes,
    bool IsPublished = false
);

public record CreateContentCourseResponse(
    Guid Id,
    string Title,
    string Description,
    int EstimatedDurationMinutes,
    bool IsPublished,
    DateTime CreatedAtUtc
);

public record AssignContentCourseRequest(
    Guid? TargetUserId = null,
    Guid? TargetTeamId = null,
    DateTime? DueDateUtc = null
);

public record AssignContentCourseResponse(
    Guid AssignmentId,
    Guid ContentCourseId,
    Guid? TargetUserId,
    Guid? TargetTeamId,
    DateTime AssignedAtUtc,
    DateTime? DueDateUtc
);

public record GenerateContentCourseRequest(string Prompt, int DesiredDurationMinutes);

public record GenerateContentCourseResponse(
    string Title,
    string Description,
    List<string> Modules
);
