using FastEndpoints;
namespace TimboLearn.Features.ContentCourses.CreateContentCourse;

public class CreateContentCourseEndpoint : Endpoint<CreateContentCourseRequest, CreateContentCourseResponse>
{
    private readonly IContentCourseService _service;

    public CreateContentCourseEndpoint(IContentCourseService service)
    {
        _service = service;
    }

    public override void Configure()
    {
        Post("/api/content-courses");
        Policies("CanManageContentCourses");
        Summary(s =>
        {
            s.Summary = "Create content course";
            s.Description = "Creates a new learning content course";
        });
    }

    public override async Task HandleAsync(CreateContentCourseRequest req, CancellationToken ct)
    {
        var course = await _service.CreateContentCourseAsync(req.Title, req.Description, req.EstimatedDurationMinutes, req.IsPublished, ct);
        await SendOkAsync(new CreateContentCourseResponse { Id = course.Id, Title = course.Title }, ct);
    }
}

public class CreateContentCourseRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int EstimatedDurationMinutes { get; set; }
    public bool IsPublished { get; set; }
}

public class CreateContentCourseResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}
