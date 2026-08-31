# TimboLearn - Enterprise Learning Platform API

## Portfolio Showcase Project

**TimboLearn** is an enterprise-grade demonstration application built with **.NET 10** showcasing modern C# backend architecture patterns for a n-layered/onion application - in this case, a very basic learning platform.

**Purpose:** This repository demonstrates what a modern enterprise learning platform backend looks like when built from scratch using industry best practices.

> **📚 Full Documentation:** See the **Wiki** for detailed guides on [getting started](https://github.com/TimboLearn/TimboLearn/wiki/Getting-Started), [architecture](https://github.com/TimboLearn/TimboLearn/wiki/Architecture), [testing](https://github.com/TimboLearn/TimboLearn/wiki/Testing), and [troubleshooting](https://github.com/TimboLearn/TimboLearn/wiki/Troubleshooting).

---

## What This Is

✅ **A portfolio showcase** for demonstrating modern .NET enterprise architecture  
✅ **A reference implementation** of Vertical Slice Architecture  
✅ **A learning resource** for .NET developers exploring modern patterns  
✅ **A complete, runnable API** with SQLite database and seed data  
✅ **Built with AI assistance** (OpenCode and Qwen3.5)

## What This Is Not

❌ **Not a production-ready application** - missing logging, monitoring, rate limiting  
❌ **Not a tutorial** - assumes .NET knowledge, focuses on architecture demonstration  
❌ **Not a template** - opinionated demo, not a generic starter template  
❌ **Not actively maintained** - snapshot portfolio piece, not a long-term project  

---

## Architecture Highlights

```
┌─────────────────────────┐         ┌─────────────────────────┐
│   Auth0 / Entra ID      │         │    TimboLearn API       │
│   (Authentication)      │  ───▶   │   (Authorization)       │
│   - JWT Tokens          │         │   - Policy-based AuthZ  │
│   - OIDC/OAuth2         │         │   - Team permissions    │
└─────────────────────────┘         └─────────────────────────┘
```

**Key Patterns:**
- **Hybrid AuthN/AuthZ** - External OIDC providers + ASP.NET Core policies
- **Vertical Slice Architecture** - Features organized by business capability
- **EF Core + Dapper** - Best tool for each job (writes vs reads)
- **FastEndpoints** - Lightweight REPR pattern on Minimal APIs
- **AI Integration** - Custom agent pattern for content generation

---

## Technology Stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 10 (preview) |
| **API Framework** | FastEndpoints |
| **Data** | EF Core 10 (writes) + Dapper (reads) |
| **Database** | SQLite (dev) / SQL Server (production) |
| **Auth** | JWT Bearer (Auth0/Entra ID or test tokens) |
| **OpenAPI** | NSwag + FastEndpoints.Swagger |
| **Testing** | xUnit + Testcontainers |

---

## Quick Start

```bash
# Clone and build
git clone <repo-url>
cd TimboLearn
dotnet build

# Run the API (SQLite + seed data auto-created)
dotnet run --project src/TimboLearn.Api

# Open Swagger UI
# Navigate to: http://localhost:5000/swagger
```

**Testing the API:**
1. Call `POST /api/test-token` to get a JWT token
2. Click **Authorize** button, enter: `Bearer <your-token>`
3. Test any endpoint!

---

## Documentation

| Document | Description |
|---|---|
| **[Getting Started](https://github.com/TimboLearn/TimboLearn/wiki/Getting-Started)** | Complete setup guide, test tokens, database config |
| **[Architecture](https://github.com/TimboLearn/TimboLearn/wiki/Architecture)** | Technical deep dive, technology choices, design patterns |
| **[Testing Guide](https://github.com/TimboLearn/TimboLearn/wiki/Testing)** | Manual testing scenarios, integration tests, curl examples |
| **[Troubleshooting](https://github.com/TimboLearn/TimboLearn/wiki/Troubleshooting)** | Common issues and solutions |

---

## Project Structure

```
TimboLearn.sln
├── src/
│   ├── TimboLearn.Api/              # API host (FastEndpoints, Swagger)
│   ├── TimboLearn.Features/         # Vertical slices (Users, Teams, ContentCourses)
│   ├── TimboLearn.Infrastructure/   # EF Core, Dapper, AI agents
│   └── TimboLearn.ServiceDefaults/  # Shared services
└── tests/
    └── TimboLearn.IntegrationTests/ # xUnit + Testcontainers
```

---

## About This Demo

**Built to demonstrate:**
- Modern .NET enterprise architecture patterns
- Vertical Slice Architecture with FastEndpoints
- Hybrid authentication/authorization
- High-performance data access (EF Core + Dapper)
- AI integration patterns
- Developer-friendly local setup (SQLite, seed data, test tokens)

**Note:** This is a demonstration project. Production use would require additional considerations (logging, monitoring, rate limiting, etc.).

---

**Ready to explore?** Start with [Getting Started](docs/GettingStarted.md) and head to http://localhost:5000/swagger!

---

*TimboLearn - Modern Enterprise Learning Platform Reference Architecture*
