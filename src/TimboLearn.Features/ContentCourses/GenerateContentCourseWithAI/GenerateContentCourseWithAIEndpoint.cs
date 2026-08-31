using FastEndpoints;
using TimboLearn.Infrastructure.AI;
namespace TimboLearn.Features.ContentCourses.GenerateContentCourseWithAI;

public class GenerateContentCourseWithAIEndpoint : Endpoint<GenerateContentCourseWithAIRequest, GenerateContentCourseWithAIResponse>
{
    private readonly IContentCourseAiAgent _aiAgent;

    public GenerateContentCourseWithAIEndpoint(IContentCourseAiAgent aiAgent)
    {
        _aiAgent = aiAgent;
    }

    public override void Configure()
    {
        Post("/api/content-courses/ai-generate");
        Policies("CanManageContentCourses");
        Summary(s =>
        {
            s.Summary = "AI-generate content course";
            s.Description = "Uses AI to generate a course structure from a prompt";
        });
    }

    public override async Task HandleAsync(GenerateContentCourseWithAIRequest req, CancellationToken ct)
    {
        var result = await _aiAgent.DraftPlanAsync(req.Prompt, req.DesiredDurationMinutes, ct);
        await SendOkAsync(new GenerateContentCourseWithAIResponse
        {
            Title = result.Title,
            Description = result.Description,
            Modules = result.Modules
        }, ct);
    }
}

public class GenerateContentCourseWithAIRequest
{
    public string Prompt { get; set; } = string.Empty;
    public int DesiredDurationMinutes { get; set; }
}

public class GenerateContentCourseWithAIResponse
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Modules { get; set; } = new();
}
