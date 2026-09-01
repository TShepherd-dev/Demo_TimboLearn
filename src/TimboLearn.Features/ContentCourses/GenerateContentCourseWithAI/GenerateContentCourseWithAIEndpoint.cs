using FastEndpoints;
using TimboLearn.Infrastructure.AI;
namespace TimboLearn.Features.ContentCourses.GenerateContentCourseWithAI;

/// <summary>
/// ENDPOINT: POST /api/content-courses/ai-generate
/// 
/// PURPOSE: Generate course structure using AI from a text prompt.
/// 
/// AUTHORIZATION: Requires "CanManageContentCourses" policy
/// (TeamAdmin role OR ContentCourse.Manage permission)
/// 
/// CURRENT STATE: MOCK IMPLEMENTATION
/// The IContentCourseAiAgent returns templated responses, not real AI.
/// See: docs/AI-Integration.md for integration examples (Ollama, Gemini, Azure OpenAI)
/// 
/// REQUEST EXAMPLE:
/// {
///   "prompt": "Leadership Skills for New Managers",
///   "desiredDurationMinutes": 120
/// }
/// 
/// RESPONSE EXAMPLE (Mock):
/// {
///   "title": "AI-Generated Content: Leadership Skills for New Managers",
///   "description": "Comprehensive training course covering Leadership Skills...",
///   "modules": [
///     "Introduction to Leadership Skills for New Managers",
///     "Fundamentals and Core Concepts",
///     "Best Practices for Leadership Skills",
///     "Advanced Techniques",
///     "Hands-on Lab: Applying Leadership Skills",
///     "Assessment: Leadership Skills Knowledge Check"
///   ]
/// }
/// </summary>
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
            s.Description = "Uses AI to generate a course structure from a prompt\n\n**NOTE:** Current implementation is mocked. See docs/AI-Integration.md for real AI integration.";
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

/// <summary>
/// REQUEST DTO: AI course generation parameters
/// </summary>
public class GenerateContentCourseWithAIRequest
{
    /// <summary>Topic/prompt for course generation (e.g., "Effective Leadership")</summary>
    public string Prompt { get; set; } = string.Empty;
    
    /// <summary>Target course duration in minutes (e.g., 60, 120, 180)</summary>
    public int DesiredDurationMinutes { get; set; }
}

/// <summary>
/// RESPONSE DTO: AI-generated course structure
/// </summary>
public class GenerateContentCourseWithAIResponse
{
    /// <summary>Generated course title</summary>
    public string Title { get; set; } = string.Empty;
    
    /// <summary>Generated course description</summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>List of module titles (typically 5-7 modules)</summary>
    public List<string> Modules { get; set; } = new();
}
