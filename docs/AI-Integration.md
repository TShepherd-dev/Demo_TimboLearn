# AI Integration Guide

## Current State: Mock Implementation

**As of this writing, the AI course generation feature uses a hardcoded mock implementation.**

The `ContentCourseAiAgent` class returns templated responses with the user's prompt text inserted into predefined module titles. No actual AI/LLM service is called.

### Why Is This Mocked?

This is a **deliberate design decision** for the following reasons:

#### 1. No Truly Free, No-Signup Provider Exists

While many AI providers offer "free tiers," they all require:
- Account registration
- API key management
- Credit card verification (even for free tiers)
- Ongoing key rotation/management

For a **demo/portfolio project** that should work out-of-the-box, this creates friction.

#### 2. Provider Multiplicity

The AI landscape is fragmented with dozens of providers, each with:
- Different API formats
- Different authentication schemes
- Different rate limits
- Different pricing models
- Different model capabilities

Choosing one would:
- Date the project quickly (providers rise/fall)
- Alienate users who prefer alternatives
- Require ongoing maintenance as APIs change

#### 3. Demo Portability

This project is designed to:
- Run at meetups/conferences (possibly offline)
- Work without external dependencies
- Demonstrate **architecture patterns**, not specific AI integrations

A mock ensures the demo **always works** regardless of:
- Network connectivity
- API quota exhaustion
- Provider service outages
- API key configuration errors

---

## Architecture: Ready for Real Integration

The codebase is **architecturally ready** for real AI integration:

```
┌─────────────────────────┐
│  GenerateContentCourse  │
│     Endpoint (API)      │
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│  IContentCourseAiAgent  │
│    (Abstraction Layer)  │
└───────────┬─────────────┘
            │
            ▼
┌─────────────────────────┐
│  ContentCourseAiAgent   │
│   (Current: Mock)       │
│   (Future: Real API)    │
└─────────────────────────┘
```

**To integrate a real provider, you only need to:**
1. Implement `IContentCourseAiAgent` with real API calls
2. Register the new implementation in DI
3. (Optional) Add configuration for API keys, model selection, etc.

---

## Integration Examples

Below are examples for popular providers. **Choose one** based on your needs.

---

### Option 1: Ollama (Local, Free, No API Key)

**Best for:** Local demos, offline presentations, zero-cost development

**Setup:**
```bash
# Install Ollama
# macOS/Linux:
curl -fsSL https://ollama.ai/install.sh | sh

# Windows: Download from https://ollama.ai/download

# Pull a model (e.g., Llama 3.2)
ollama run llama3.2
```

**Implementation:**

```csharp
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace TimboLearn.Infrastructure.AI;

public class OllamaContentCourseAiAgent : IContentCourseAiAgent
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaContentCourseAiAgent> _logger;
    private readonly string _modelName;

    public OllamaContentCourseAiAgent(
        HttpClient httpClient,
        ILogger<OllamaContentCourseAiAgent> logger,
        string model = "llama3.2")
    {
        _httpClient = httpClient;
        _logger = logger;
        _modelName = model;
    }

    public async Task<GeneratedContentCourseResult> DraftPlanAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Generating course with Ollama model: {Model}", _modelName);

        var systemPrompt = """
            You are an instructional designer creating online course outlines.
            Return a JSON response with this exact structure:
            {
                "title": "Course Title",
                "description": "Course description (2-3 sentences)",
                "modules": ["Module 1", "Module 2", "Module 3", ...],
                "estimatedDurationMinutes": 120
            }
            
            Create 5-7 modules appropriate for the requested duration.
            Be specific and actionable in module titles.
            """;

        var userPrompt = $"""
            Create a {desiredDurationMinutes}-minute online course about: {prompt}
            
            Return ONLY the JSON response, no additional text.
            """;

        var request = new
        {
            model = _modelName,
            prompt = userPrompt,
            system = systemPrompt,
            stream = false,
            format = "json"
        };

        var response = await _httpClient.PostAsJsonAsync(
            "http://localhost:11434/api/generate",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OllamaResponse>(cancellationToken);
        var content = result?.Response ?? throw new InvalidOperationException("Empty response from Ollama");

        // Parse the JSON response from the model
        var courseData = System.Text.Json.JsonSerializer.Deserialize<CourseData>(content);

        return new GeneratedContentCourseResult(
            courseData?.Title ?? $"Course: {prompt}",
            courseData?.Description ?? $"AI-generated course on {prompt}",
            courseData?.Modules ?? new List<string> { "Introduction", "Core Concepts", "Advanced Topics" },
            courseData?.EstimatedDurationMinutes ?? desiredDurationMinutes
        );
    }

    private record OllamaResponse(string Response);
    private record CourseData(string Title, string Description, List<string> Modules, int EstimatedDurationMinutes);
}
```

**Registration in Program.cs:**

```csharp
// Add Ollama HTTP client
builder.Services.AddHttpClient<IContentCourseAiAgent, OllamaContentCourseAiAgent>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = TimeSpan.FromMinutes(2); // Ollama can be slow on first run
});
```

**Pros:**
- ✅ Completely free
- ✅ No API keys
- ✅ Works offline
- ✅ Privacy-friendly (data never leaves your machine)

**Cons:**
- ❌ Requires Ollama installation (~2GB)
- ❌ Needs decent RAM (8GB+ recommended)
- ❌ Model quality varies
- ❌ Slower than cloud APIs

---

### Option 2: Google Gemini (Generous Free Tier)

**Best for:** Real cloud AI with good free tier

**Setup:**
1. Go to https://aistudio.google.com/
2. Sign in with Google account
3. Create API key
4. Add to `appsettings.json`:

```json
{
  "AI": {
    "Gemini": {
      "ApiKey": "your-api-key-here",
      "Model": "gemini-1.5-flash"
    }
  }
}
```

**Implementation:**

```csharp
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace TimboLearn.Infrastructure.AI;

public class GeminiContentCourseAiAgent : IContentCourseAiAgent
{
    private readonly HttpClient _httpClient;
    private readonly GeminiOptions _options;

    public GeminiContentCourseAiAgent(
        HttpClient httpClient,
        IOptions<GeminiOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<GeneratedContentCourseResult> DraftPlanAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken cancellationToken = default)
    {
        var systemPrompt = """
            You are an expert instructional designer. Create a comprehensive course outline.
            Respond with valid JSON only, no markdown or extra text.
            """;

        var userPrompt = $"""
            Create a {desiredDurationMinutes}-minute professional development course about: {prompt}
            
            Return JSON with this structure:
            {{
                "title": "Course Title",
                "description": "2-3 sentence description",
                "modules": ["Module 1", "Module 2", ...],
                "estimatedDurationMinutes": {desiredDurationMinutes}
            }}
            
            Include 5-7 specific, actionable modules.
            """;

        var request = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = systemPrompt },
                        new { text = userPrompt }
                    }
                }
            },
            generationConfig = new
            {
                temperature = 0.7,
                maxOutputTokens = 1024
            }
        };

        var response = await _httpClient.PostAsJsonAsync(
            $"/v1beta/models/{_options.Model}:generateContent?key={_options.ApiKey}",
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>(cancellationToken);
        var text = result?.Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text 
            ?? throw new InvalidOperationException("Empty response from Gemini");

        // Extract JSON from response (Gemini sometimes wraps in markdown)
        var jsonText = text.Trim().Replace("```json", "").Replace("```", "").Trim();
        var courseData = System.Text.Json.JsonSerializer.Deserialize<CourseData>(jsonText);

        return new GeneratedContentCourseResult(
            courseData?.Title ?? $"Course: {prompt}",
            courseData?.Description ?? $"AI-generated course on {prompt}",
            courseData?.Modules ?? new List<string> { "Introduction", "Core Concepts", "Advanced Topics" },
            courseData?.EstimatedDurationMinutes ?? desiredDurationMinutes
        );
    }

    private record GeminiResponse(
        Candidate[]? Candidates,
        string? Error);

    private record Candidate(Content Content);

    private record Content(Part[] Parts);

    private record Part(string Text);

    private record CourseData(string Title, string Description, List<string> Modules, int EstimatedDurationMinutes);
}

public class GeminiOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-1.5-flash";
}
```

**Registration:**

```csharp
builder.Services.Configure<GeminiOptions>(
    builder.Configuration.GetSection("AI:Gemini"));

builder.Services.AddHttpClient<IContentCourseAiAgent, GeminiContentCourseAiAgent>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com");
});
```

**Free Tier Limits:**
- 60 requests/minute
- 1,000,000 tokens/day
- No credit card required

**Pros:**
- ✅ Real cloud AI with excellent quality
- ✅ Generous free tier
- ✅ Fast response times
- ✅ No credit card required

**Cons:**
- ❌ Requires Google account
- ❌ API key management
- ❌ Rate limits (though generous)
- ❌ Data sent to Google

---

### Option 3: Azure OpenAI (Enterprise Grade)

**Best for:** Production deployments, enterprise environments

**Setup:**
1. Create Azure OpenAI resource in Azure Portal
2. Deploy a model (e.g., GPT-4o-mini)
3. Get endpoint URL and API key
4. Add to `appsettings.json`:

```json
{
  "AI": {
    "AzureOpenAI": {
      "Endpoint": "https://your-resource.openai.azure.com/",
      "ApiKey": "your-api-key",
      "DeploymentName": "gpt-4o-mini"
    }
  }
}
```

**Implementation:**

```csharp
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace TimboLearn.Infrastructure.AI;

public class AzureOpenAiContentCourseAiAgent : IContentCourseAiAgent
{
    private readonly AzureOpenAiOptions _options;

    public AzureOpenAiContentCourseAiAgent(IOptions<AzureOpenAiOptions> options)
    {
        _options = options.Value;
    }

    public async Task<GeneratedContentCourseResult> DraftPlanAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken cancellationToken = default)
    {
        var client = new OpenAIClient(
            new Uri(_options.Endpoint),
            new AzureKeyCredential(_options.ApiKey));

        var systemMessage = new ChatRequestSystemMessage("""
            You are an expert instructional designer creating professional development courses.
            Respond with ONLY valid JSON, no markdown or additional text.
            """);

        var userMessage = new ChatRequestUserMessage($"""
            Create a {desiredDurationMinutes}-minute course about: {prompt}
            
            Return JSON with this exact structure:
            {{
                "title": "Course Title",
                "description": "2-3 sentence description",
                "modules": ["Module 1", "Module 2", "Module 3", ...],
                "estimatedDurationMinutes": {desiredDurationMinutes}
            }}
            
            Include 5-7 specific, actionable module titles.
            """);

        var response = await client.GetChatCompletionsAsync(
            new ChatCompletionsOptions(_options.DeploymentName, new[] { systemMessage, userMessage })
            {
                Temperature = 0.7f,
                MaxTokens = 1024
            },
            cancellationToken);

        var content = response.Value.Choices[0].Message.Content;
        var courseData = System.Text.Json.JsonSerializer.Deserialize<CourseData>(content);

        return new GeneratedContentCourseResult(
            courseData?.Title ?? $"Course: {prompt}",
            courseData?.Description ?? $"AI-generated course on {prompt}",
            courseData?.Modules ?? new List<string> { "Introduction", "Core Concepts", "Advanced Topics" },
            courseData?.EstimatedDurationMinutes ?? desiredDurationMinutes
        );
    }
}

public class AzureOpenAiOptions
{
    public string Endpoint { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DeploymentName { get; set; } = "gpt-4o-mini";
}
```

**Registration:**

```csharp
builder.Services.Configure<AzureOpenAiOptions>(
    builder.Configuration.GetSection("AI:AzureOpenAI"));

builder.Services.AddSingleton<IContentCourseAiAgent, AzureOpenAiContentCourseAiAgent>();
```

**Pricing:**
- GPT-4o-mini: ~$0.15 per 1M input tokens
- Very affordable for development/testing

**Pros:**
- ✅ Enterprise-grade reliability
- ✅ Best-in-class model quality
- ✅ Azure AD integration
- ✅ Private networking options
- ✅ Compliance certifications

**Cons:**
- ❌ Requires Azure subscription
- ❌ More complex setup
- ❌ Cost (though minimal for dev)
- ❌ Vendor lock-in

---

## Switching Implementations

To switch from mock to real AI:

### 1. Update Program.cs

**Current (Mock):**
```csharp
builder.Services.AddScoped<IContentCourseAiAgent, ContentCourseAiAgent>();
```

**New (e.g., Ollama):**
```csharp
builder.Services.AddHttpClient<IContentCourseAiAgent, OllamaContentCourseAiAgent>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
});
```

### 2. (Optional) Add Configuration

Add settings to `appsettings.json` for API keys, model names, etc.

### 3. Test

```bash
dotnet run --project src/TimboLearn.Api

# Test the endpoint:
curl -X POST http://localhost:5000/api/content-courses/ai-generate \
  -H "Content-Type: application/json" \
  -d '{
    "prompt": "Effective Leadership",
    "desiredDurationMinutes": 120
  }'
```

---

## Recommendation

**For Local Development:** Use **Ollama** if you have the RAM. It's free, private, and works offline.

**For Cloud Demo:** Use **Google Gemini** for its generous free tier and excellent quality.

**For Production:** Use **Azure OpenAI** or another enterprise provider for reliability and support.

---

## Future Enhancements

Once integrated, consider adding:

- [ ] **Retry logic** with Polly for transient failures
- [ ] **Caching** to reduce API calls for repeated prompts
- [ ] **Prompt templates** for consistent output quality
- [ ] **Validation** to verify AI-generated content meets requirements
- [ ] **Human review workflow** before publishing AI content
- [ ] **Multi-model support** with fallback logic
- [ ] **Token counting** for cost tracking
- [ ] **Structured logging** for AI interactions

---

**See Also:**
- [AI Integration Pattern](Architecture.md#ai-integration-pattern)
- [Content Course Generation](Testing.md#scenario-7-ai-course-generation)
