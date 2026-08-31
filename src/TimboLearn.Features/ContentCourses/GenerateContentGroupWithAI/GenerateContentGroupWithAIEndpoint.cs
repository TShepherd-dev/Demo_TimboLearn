namespace TimboLearn.Features.ContentCourses.GenerateContentCourseWithAI;

public class GenerateContentCourseWithAIEndpoint : Endpoint<GenerateContentCourseRequest, GenerateContentCourseResponse>
{
    private readonly IContentCourseService _contentCourseService;

    public GenerateContentCourseWithAIEndpoint(IContentCourseService contentCourseService)
    {
        _contentCourseService = contentCourseService;
    }

    public override void Configure()
    {
        Post("/api/content-groups/ai-generate");
        Policies("RequireAuthenticatedUser", "CanManageContentCourses");
        Summary(s => {
            s.Summary = "Uses AI Agent to auto-generate a structured content group draft";
            s.Description = "Leverages Microsoft Agents Framework / Semantic Kernel to create comprehensive content groups from topic prompts.";
        });
    }

    public override async Task HandleAsync(GenerateContentCourseRequest req, CancellationToken ct)
    {
        var result = await _contentCourseService.GenerateContentCourseWithAiAsync(req.Prompt, req.DesiredDurationMinutes, ct);

        if (result == null)
        {
            await SendNotFoundAsync();
            return;
        }

        var response = new GenerateContentCourseResponse(
            result.Title,
            result.Description,
            result.Modules
        );

        await SendOkAsync(response, ct);
    }
}
