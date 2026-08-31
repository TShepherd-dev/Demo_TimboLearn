namespace TimboLearn.Features.ContentCourses.AssignContentCourse;

public class AssignContentCourseEndpoint : Endpoint<AssignContentCourseRequest, AssignContentCourseResponse>
{
    private readonly IContentCourseService _contentCourseService;

    public AssignContentCourseEndpoint(IContentCourseService contentCourseService)
    {
        _contentCourseService = contentCourseService;
    }

    public override void Configure()
    {
        Post("/api/content-groups/{id}/assign");
        Policies("RequireAuthenticatedUser", "CanAssignContentCourse");
        Summary(s => {
            s.Summary = "Assigns a content group to a user or team";
            s.ExampleRequest = new AssignContentCourseRequest(
                TargetTeamId: Guid.NewGuid(),
                DueDateUtc: DateTime.UtcNow.AddDays(30)
            );
        });
    }

    public override async Task HandleAsync(AssignContentCourseRequest req, CancellationToken ct)
    {
        var contentCourseId = Route<Guid>("id");
        
        var result = await _contentCourseService.AssignContentCourseAsync(
            contentCourseId,
            req.TargetUserId,
            req.TargetTeamId,
            req.DueDateUtc,
            ct
        );

        await SendOkAsync(result, ct);
    }
}
