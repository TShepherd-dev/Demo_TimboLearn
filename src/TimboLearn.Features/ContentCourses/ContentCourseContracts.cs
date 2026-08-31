namespace TimboLearn.Features.ContentCourses;

public record CreateContentCourseRequest(
    string Title,
    string Description,
    int EstimatedDurationMinutes,
    bool IsPublished = false
);

public record CreateContentCourseResponse(
    int Id,
    string Title,
    string Description,
    int EstimatedDurationMinutes,
    bool IsPublished,
    DateTime CreatedAtUtc
);

public record AssignContentCourseRequest(
    int? TargetUserId = null,
    int? TargetTeamId = null,
    DateTime? DueDateUtc = null
);

public record AssignContentCourseResponse(
    int AssignmentId,
    int ContentCourseId,
    int? TargetUserId,
    int? TargetTeamId,
    DateTime AssignedAtUtc,
    DateTime? DueDateUtc
);

public record GenerateContentCourseRequest(string Prompt, int DesiredDurationMinutes);

public record GenerateContentCourseResponse(
    string Title,
    string Description,
    List<string> Modules
);
