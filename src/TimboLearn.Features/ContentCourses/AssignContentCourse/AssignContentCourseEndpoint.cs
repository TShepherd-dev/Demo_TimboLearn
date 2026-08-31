using FastEndpoints;
namespace TimboLearn.Features.ContentCourses.AssignContentCourse;

public class AssignContentCourseEndpoint : Endpoint<AssignContentCourseRequest, AssignContentCourseResponse>
{
    private readonly IContentCourseService _service;

    public AssignContentCourseEndpoint(IContentCourseService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Post("/api/content-courses/{id}/assign");
        Policies("CanAssignContentCourse");
        Summary(s =>
        {
            s.Summary = "Assign content course";
            s.Description = "Route parameter: id (course Guid). Assigns to user or team.";
        });
    }

    public override async Task HandleAsync(AssignContentCourseRequest req, CancellationToken ct)
    {
        var courseId = Route<Guid>("id");
        await _service.AssignContentCourseAsync(courseId, req.TargetUserId, req.TargetTeamId, null, ct);
        await SendOkAsync(new AssignContentCourseResponse { Success = true }, ct);
    }
}

public class AssignContentCourseRequest
{
    public Guid? TargetUserId { get; set; }
    public Guid? TargetTeamId { get; set; }
}

public class AssignContentCourseResponse
{
    public bool Success { get; set; }
}
