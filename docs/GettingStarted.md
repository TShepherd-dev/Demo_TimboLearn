# Getting Started with TimboLearn

Welcome to the TimboLearn Enterprise Learning Platform API! This guide will get you up and running in minutes.

## Quick Start (5 Minutes)

### Prerequisites

- .NET 10 SDK (preview)
- Visual Studio 2022 / Rider / VS Code
- No database setup required!

### Step 1: Clone & Restore

```bash
git clone <your-repo-url>
cd TimboLearn
dotnet restore
```

### Step 2: Build

```bash
dotnet build
```

### Step 3: Run

```bash
dotnet run --project src/TimboLearn.Api
```

The application will:
- ✅ Create a SQLite database (`timbolearn.db`) automatically
- ✅ Apply database migrations
- ✅ Seed demo data (10 users, 2 teams, 3 courses)
- ✅ Start the API on http://localhost:5000

### Step 4: Open Swagger UI

Navigate to: **http://localhost:5000/swagger**

---

## Testing the API

### Option 1: Generate Test Token (Recommended)

**This is the easiest way to test everything!**

1. **Get a test token:**
   - In Swagger UI, find the `POST /api/test-token` endpoint
   - Click "Try it out"
   - Click "Execute"
   - Copy the returned JWT token

2. **Authorize with your token:**
   - Click the "Authorize" button (top right)
   - Paste: `Bearer <your-token-here>`
   - Click "Authorize"
   - Click "Close"

3. **Test any endpoint:**
   - All endpoints now show a lock icon (unlocked)
   - Click "Try it out" on any endpoint
   - Execute with or without modifying parameters
   - See live responses from your database!

### Option 2: Use Auth0 or Entra ID

For production-like testing with real authentication:

1. **Update `src/TimboLearn.Api/appsettings.json`:**

```json
{
  "Auth0": {
    "Authority": "https://your-tenant.auth0.com/",
    "Audience": "https://your-api-audience"
  }
}
```

2. **Obtain a token from your identity provider**

3. **Use the "Authorize" button in Swagger UI** to input your token

---

## Understanding the Seed Data

When you run the API in Development mode, it automatically seeds the database with:

### Users (10 total)

| Name | Email | Role |
|------|-------|------|
| Alice Johnson | alice.johnson@example.com | TeamAdmin |
| Bob Smith | bob.smith@example.com | Member |
| Carol Williams | carol.williams@example.com | Member |
| David Brown | david.brown@example.com | Member |
| Emma Davis | emma.davis@example.com | Member |
| Frank Miller | frank.miller@example.com | TeamAdmin |
| Grace Wilson | grace.wilson@example.com | Member |
| Henry Moore | henry.moore@example.com | Member |
| Iris Taylor | iris.taylor@example.com | Member |
| Jack Anderson | jack.anderson@example.com | Member |

### Teams (2 total)

**Engineering Team** (5 members)
- Alice Johnson (Admin)
- Bob Smith, Carol Williams, David Brown, Emma Davis (Members)

**Marketing Team** (8 members)
- Frank Miller (Admin)
- Grace Wilson, Henry Moore, Iris Taylor, Jack Anderson (Members)
- Alice Johnson, Bob Smith, Carol Williams (also in Engineering)

### Content Courses (3 total)

1. **Cybersecurity Hygiene for Remote Workers** (90 min) - Published
2. **Effective Communication in Virtual Teams** (60 min) - Published
3. **Project Management Fundamentals** (120 min) - Draft

### Course Assignments (3 total)

- Cybersecurity course → Engineering Team (Not Started)
- Communication course → Marketing Team (In Progress)
- Project Management course → Engineering Team (Not Started)

---

## Database Configuration

### Using SQLite (Default)

SQLite is perfect for local development and demos:

- **Zero setup** - database file created automatically
- **File location**: `src/TimboLearn.Api/timbolearn.db`
- **Connection string**: `Data Source=timbolearn.db`

**To reset the database:**

```bash
# Delete the database file
rm src/TimboLearn.Api/timbolearn.db

# Re-run the API (database and seed data recreated)
dotnet run --project src/TimboLearn.Api
```

### Using SQL Server (Optional)

For production-like testing:

1. **Update connection string** in `src/TimboLearn.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "TimboLearnDb": "Server=(localdb)\\mssqllocaldb;Database=TimboLearn;Trusted_Connection=True;TrustServerCertificate=true;"
  }
}
```

2. **Apply migrations:**

```bash
dotnet ef database update --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

3. **Seed data manually** (if needed):

The seed data runs automatically on startup in Development mode. For SQL Server, you may need to run the API once to trigger seeding.

---

## Managing Migrations

### Create a New Migration

```bash
dotnet ef migrations add MigrationName --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

### Remove Last Migration

```bash
dotnet ef migrations remove --project src/TimboLearn.Infrastructure
```

### Apply Migrations

```bash
dotnet ef database update --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

### Generate SQL Script

```bash
dotnet ef migrations script --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

---

## Next Steps

- **[Architecture Overview](Architecture.md)** - Learn about the system design
- **[Testing Guide](Testing.md)** - Detailed API testing strategies
- **[Troubleshooting](Troubleshooting.md)** - Common issues and solutions

---

## Development Tips

### Hot Reload (Optional)

For faster development iterations:

```bash
dotnet watch run --project src/TimboLearn.Api
```

### Enable Detailed Logging

Add to `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

### View Generated Database

Use [DB Browser for SQLite](https://sqlitebrowser.org/) to inspect `timbolearn.db` and understand the data structure.

---

**Ready to explore?** Head to [http://localhost:5000/swagger](http://localhost:5000/swagger) and start testing!
