# Troubleshooting TimboLearn

Common issues and solutions for running and testing TimboLearn.

---

## Build Issues

### Error: "The SDK 'Microsoft.Extensions.AppHost.Sdk' was not found"

**Problem:** .NET Aspire workload not installed or deprecated

**Solution:**
The AppHost project has been removed. Build and run the API directly:

```bash
dotnet run --project src/TimboLearn.Api
```

If you need Aspire for orchestration, reinstall the workload:

```bash
dotnet workload install aspire
```

---

### Error: "Package version not defined for Central Package Management"

**Problem:** Package referenced in project but version not defined in `Directory.Packages.props`

**Solution:**
Add the missing package version to `Directory.Packages.props`:

```xml
<PackageVersion Include="PackageName" Version="x.y.z" />
```

Or check for typos in package name.

---

### Error: "Build failed with 1 error(s)"

**Check:**
```bash
dotnet build --verbosity detailed
```

Common causes:
- Missing `using` directive
- Namespace mismatch after refactoring
- Entity class name doesn't match configuration

---

## Database Issues

### Error: "SQLite error: no such table: Users"

**Problem:** Migrations not applied

**Solution:**

```bash
# Ensure you're in solution root
cd C:\path\to\TimboLearn

# Apply migrations
dotnet ef database update --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

**Alternative:** Delete database and restart (Development mode only):

```bash
rm src/TimboLearn.Api/timbolearn.db
dotnet run --project src/TimboLearn.Api  # Auto-creates and seeds
```

---

### Error: "Cannot open database 'TimboLearn' requested by the login"

**Problem:** SQL Server connection failed

**Solution:**

1. **Check SQL Server is running:**
   ```bash
   # For LocalDB
   sqllocaldb start MSSQLLocalDB
   ```

2. **Verify connection string** in `appsettings.json`:
   ```json
   "ConnectionStrings": {
     "TimboLearnDb": "Server=(localdb)\\mssqllocaldb;Database=TimboLearn;Trusted_Connection=True;"
   }
   ```

3. **Switch to SQLite** (easier for local dev):
   ```json
   "ConnectionStrings": {
     "TimboLearnDb": "Data Source=timbolearn.db"
   }
   ```

---

### Error: "A migration with the same name already exists"

**Problem:** Migration file conflicts

**Solution:**

```bash
# Remove last migration
dotnet ef migrations remove --project src/TimboLearn.Infrastructure

# Create new migration with different name
dotnet ef migrations add MigrationDescription --project src/TimboLearn.Infrastructure
```

---

## Authentication Issues

### Error: "401 Unauthorized" on protected endpoints

**Problem:** Missing or invalid JWT token

**Solution:**

1. **Generate test token:**
   - Execute `POST /api/test-token` in Swagger UI
   - Copy the token from response

2. **Authorize in Swagger UI:**
   - Click "Authorize" button
   - Paste: `Bearer <your-token-here>`
   - Click "Authorize"

3. **Verify token claims:**
   - Token must include: `sub`, `email`, `name`
   - For role-based access: include `role` claim

---

### Error: "403 Forbidden" even with valid token

**Problem:** Insufficient permissions

**Solution:**

Check required policy for endpoint:

| Policy | Required Role/Permission |
|---|---|
| `CanManageTeams` | `TeamAdmin` or `TeamManager` |
| `CanAssignContentCourse` | `TeamAdmin`/`TeamManager` OR `ContentCourse.Assign` permission |
| `CanManageContentCourses` | `Admin` OR `ContentCourse.Manage` permission |

**Update test token generation** to include correct role:

```csharp
var token = TestTokenGenerator.GenerateToken(
    email: "admin@timbolearn.local",
    firstName: "Admin",
    lastName: "User",
    role: "TeamAdmin"  // Change role here
);
```

---

### Error: "Invalid signature" or "Token validation failed"

**Problem:** Token signing key mismatch

**Solution:**

Verify `appsettings.json` has correct signing key:

```json
{
  "Auth0": {
    "TestToken": {
      "SigningKey": "TimboLearnDemoSigningKey2026!WhichIsLongEnough"
    }
  }
}
```

The key must be **at least 32 characters** for HS256 algorithm.

---

## Runtime Issues

### API starts but Swagger UI returns 404

**Problem:** OpenAPI document middleware not registered

**Solution:**

Check `Program.cs` has:

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument();

// ...

app.UseOpenApi();
app.UseSwaggerUi();
```

**Also check:** Request is to correct URL: `http://localhost:5000/swagger` (not HTTPS)

---

### Seed data not appearing in database

**Problem:** Seeding only runs in Development mode

**Solution:**

1. **Set Development environment:**
   ```bash
   $env:ASPNETCORE_ENVIRONMENT="Development"  # PowerShell
   set ASPNETCORE_ENVIRONMENT=Development      # CMD
   ```

2. **Check database exists:**
   ```bash
   dotnet ef database update --project src/TimboLearn.Infrastructure
   ```

3. **Manually trigger seed** (if needed) by running API once

---

### Recursive CTE returns incomplete hierarchy

**Problem:** SQLite vs SQL Server syntax differences

**Solution:**

Check `TeamQueries.cs` uses SQLite-compatible syntax:

```sql
-- SQLite (correct)
SELECT * FROM TeamTree ORDER BY Level, Name;

-- SQL Server (wrong for SQLite)
SELECT * FROM dbo.TeamTree ORDER BY Level, Name;
```

Remove `dbo.` schema prefix for SQLite compatibility.

---

## Testcontainers Issues

### Error: "Docker is not running"

**Problem:** Testcontainers requires Docker Desktop

**Solution:**

1. **Start Docker Desktop**
2. **Verify Docker is running:**
   ```bash
   docker ps
   ```
3. **Run tests again**

**Alternative:** Use SQLite for tests instead of Testcontainers

---

### Error: "Container failed to start"

**Problem:** Docker resource issues or port conflicts

**Solution:**

```bash
# Clean up Docker containers
docker container prune -f

# Restart Docker Desktop
# Try tests again
```

---

## Performance Issues

### Slow API responses (> 1 second)

**Diagnosis:**

1. **Check database queries:**
   - Enable EF Core logging to see SQL
   - Look for N+1 query patterns

2. **Check recursive CTE:**
   - Large hierarchies can be slow
   - Add indexes on `ParentTeamId`

3. **Check connection pooling:**
   - SQLite: Limit concurrent connections
   - SQL Server: Increase pool size in connection string

**Solutions:**

- Add database indexes
- Use Dapper for complex read queries
- Implement caching for frequently accessed data

---

## Migration Tips

### Viewing Generated Migration SQL

```bash
dotnet ef migrations script --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

### Rolling Back Database

```bash
# Roll back to specific migration
dotnet ef database update 00000000000000_InitialCreate --project src/TimboLearn.Infrastructure

# Or delete database entirely (SQLite)
rm src/TimboLearn.Api/timbolearn.db
```

---

## Getting Help

### Enable Detailed Logging

Add to `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
    }
  }
}
```

### Check Application Logs

Look in console output or log files for:
- Database connection errors
- Authentication failures
- Migration application status

### Useful Commands

```bash
# Check .NET version
dotnet --version

# List installed EF Core tools
dotnet ef --version

# Verify database connection
dotnet ef database update --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api --verbose

# Clean and rebuild
dotnet clean && dotnet build
```

---

## Common Gotchas

### 1. Forgetting to Authorize in Swagger UI

**Symptom:** 401 on all endpoints

**Fix:** Click "Authorize" button and paste test token

---

### 2. Running with Wrong Connection String

**Symptom:** Database errors or wrong database type

**Fix:** Check `appsettings.json` connection string matches intended database (SQLite vs SQL Server)

---

### 3. Case-Sensitive Entity Names in SQLite

**Symptom:** "no such table" errors

**Fix:** Use exact entity names as defined in configurations (case-sensitive in SQLite)

---

### 4. Test Token Expired

**Symptom:** 401 after working previously

**Fix:** Tokens expire after 24 hours - generate new one with `POST /api/test-token`

---

## Still Having Issues?

1. **Clean rebuild:**
   ```bash
   dotnet clean
   dotnet restore
   dotnet build
   ```

2. **Reset database:**
   ```bash
   rm src/TimboLearn.Api/timbolearn.db
   dotnet ef database update --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
   ```

3. **Check GitHub Issues** for known problems

4. **Verify prerequisites:**
   - .NET 10 SDK installed
   - Docker Desktop running (for Testcontainers)
   - SQLite driver installed (usually included with .NET)

---

## Next Steps

- **[Getting Started](GettingStarted.md)** - Initial setup guide
- **[Testing Guide](Testing.md)** - How to test endpoints
- **[Architecture](Architecture.md)** - System design details
