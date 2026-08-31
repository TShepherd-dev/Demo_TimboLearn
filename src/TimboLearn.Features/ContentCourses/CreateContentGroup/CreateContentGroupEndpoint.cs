namespace TimboLearn.Features.ContentCourses.CreateContentCourse;

public class CreateContentCourseEndpoint : Endpoint<CreateContentCourseRequest, CreateContentCourseResponse>
{
    private readonly IContentCourseService _contentCourseService;

    public CreateContentCourseEndpoint(IContentCourseService contentCourseService)
    {
        _contentCourseService = contentCourseService;
    }

    public override void Configure()
    {
        Post("/api/content-groups");
        Policies("RequireAuthenticatedUser", "CanManageContentCourses");
        Summary(s => {
            s.Summary = "Creates a new content group";
            s.ExampleRequest = new CreateContentCourseRequest(
                "Cybersecurity Fundamentals",
                "Learn the basics of cybersecurity and threat awareness",
                120,
                true
            );
        });
    }

    public override async Task HandleAsync(CreateContentCourseRequest req, CancellationToken ct)
    {
        var result = await _contentCourseService.CreateContentCourseAsync(
            req.Title,
            req.Description,
            req.EstimatedDurationMinutes,
            req.IsPublished,
            ct
        );

        await SendCreatedAtAsync<GetContentCourseEndpoint>(new { id = result.Id }, responseBody: result, cancellation: ct);
    }
}

public class GetContentCourseEndpoint : EndpointWithoutRequest<CreateContentCourseResponse>
{
    public override void Configure()
    {
        Get("/api/content-groups/{id}");
        Policies("RequireAuthenticatedUser");
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        throw new NotImplementedException("Retrieval endpoint placeholder");
    }
}
