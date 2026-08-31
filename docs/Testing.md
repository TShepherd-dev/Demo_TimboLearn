# Testing TimboLearn API

This guide covers all aspects of testing the TimboLearn API, from quick manual testing to automated integration tests.

---

## Manual Testing with Swagger UI

### Getting Started

1. **Start the API:**
   ```bash
   dotnet run --project src/TimboLearn.Api
   ```

2. **Open Swagger UI:**
   ```
   http://localhost:5000/swagger
   ```

### Generating a Test Token

The easiest way to test is with a test token:

1. **Find the test token endpoint:**
   - Scroll to `POST /api/test-token`
   - This endpoint requires **no authentication**

2. **Generate your token:**
   - Click "Try it out"
   - Click "Execute"
   - Copy the token from the response body:
   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
     "expiresIn": "24 hours"
   }
   ```

3. **Authorize with your token:**
   - Click the "Authorize" button (top right, lock icon)
   - In the dialog, paste: `Bearer <your-token-here>`
     - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
   - Click "Authorize"
   - Click "Close"

4. **Test any endpoint:**
   - All endpoints now show as unlocked
   - Click "Try it out" on any endpoint
   - Modify request parameters if needed
   - Click "Execute"
   - View response status, headers, and body

---

## Testing Scenarios

### Scenario 1: View Current User Profile

**Endpoint:** `GET /api/users/me`

**Purpose:** Retrieves the profile of the authenticated user

**Expected Response:**
```json
{
  "id": "test-demo@timbolearn.local",
  "email": "demo@timbolearn.local",
  "firstName": "Demo",
  "lastName": "User",
  "isActive": true,
  "teamMemberships": [
    {
      "teamId": "10000000-0000-0000-0000-000000000001",
      "teamName": "Engineering Team",
      "role": "TeamAdmin"
    }
  ]
}
```

**Test Steps:**
1. Authorize with test token
2. Execute `GET /api/users/me`
3. Verify 200 OK response
4. Check user details match token claims

---

### Scenario 2: Create a New Team

**Endpoint:** `POST /api/teams`

**Request:**
```json
{
  "name": "Sales Team",
  "code": "SLS",
  "description": "Sales and business development",
  "parentTeamId": null
}
```

**Expected Response:** `201 Created`

**Test Steps:**
1. Authorize with test token (must have `TeamAdmin` role)
2. Execute with request body above
3. Verify 201 Created status
4. Check `Location` header contains new team ID
5. Verify team appears in database

---

### Scenario 3: Add User to Team

**Endpoint:** `POST /api/teams/{id}/members`

**Request:**
```json
{
  "userId": "00000000-0000-0000-0000-000000000006",
  "role": "Member"
}
```

**Expected Response:** `200 OK`

**Test Steps:**
1. Use Engineering Team ID: `10000000-0000-0000-0000-000000000001`
2. Add Frank Miller (already in Marketing, cross-functional)
3. Verify 200 OK response
4. Execute `GET /api/teams/{id}/hierarchy` to see updated membership

---

### Scenario 4: Get Team Hierarchy

**Endpoint:** `GET /api/teams/{id}/hierarchy`

**Purpose:** Retrieves team and all sub-teams using recursive CTE

**Expected Response:**
```json
[
  {
    "id": "10000000-0000-0000-0000-000000000001",
    "name": "Engineering Team",
    "code": "ENG",
    "parentTeamId": null,
    "level": 0
  },
  {
    "id": "11000000-0000-0000-0000-000000000001",
    "name": "Frontend Team",
    "code": "FE",
    "parentTeamId": "10000000-0000-0000-0000-000000000001",
    "level": 1
  },
  {
    "id": "11000000-0000-0000-0000-000000000002",
    "name": "Backend Team",
    "code": "BE",
    "parentTeamId": "10000000-0000-0000-0000-000000000001",
    "level": 1
  }
]
```

**Test Steps:**
1. Create sub-teams under Engineering Team first
2. Execute GET request
3. Verify hierarchical structure (parent + all descendants)
4. Check `level` field indicates depth

---

### Scenario 5: Create Content Course

**Endpoint:** `POST /api/content-courses`

**Request:**
```json
{
  "title": "Advanced Excel Techniques",
  "description": "Master pivot tables, macros, and data analysis",
  "estimatedDurationMinutes": 180,
  "isPublished": false
}
```

**Expected Response:** `201 Created`

**Test Steps:**
1. Authorize with test token (must have `CanManageContentCourses`)
2. Execute with request body above
3. Verify 201 Created status
4. Course saved with `IsPublished = false` (draft mode)

---

### Scenario 6: Assign Course to Team

**Endpoint:** `POST /api/content-courses/assign`

**Request:**
```json
{
  "contentCourseId": "20000000-0000-0000-0000-000000000001",
  "targetTeamId": "10000000-0000-0000-0000-000000000001",
  "dueDateUtc": "2026-12-31T23:59:59Z"
}
```

**Expected Response:** `200 OK`

**Test Steps:**
1. Use existing Cybersecurity course ID
2. Assign to Engineering Team
3. Verify assignment created with status `NotStarted`
4. All team members now have access to course

---

### Scenario 7: AI Course Generation

**Endpoint:** `POST /api/content-courses/ai-generate`

**Request:**
```json
{
  "prompt": "Leadership Skills for New Managers",
  "desiredDurationMinutes": 120
}
```

**Expected Response:**
```json
{
  "title": "AI-Generated Course: Leadership Skills for New Managers",
  "description": "Comprehensive training course covering leadership fundamentals...",
  "modules": [
    "Introduction to Leadership",
    "Communication and Feedback",
    "Delegation and Time Management",
    "Conflict Resolution",
    "Building High-Performance Teams",
    "Assessment: Leadership Style"
  ]
}
```

**Test Steps:**
1. Execute with custom prompt
2. Review generated course structure
3. **Note:** Currently returns mock data (see [Architecture.md](Architecture.md) for AI integration details)

---

## Automated Integration Tests

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/TimboLearn.IntegrationTests

# Run with verbose output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~TeamEndpointTests"
```

### Test Structure

```
TimboLearn.IntegrationTests/
├── IntegrationTestFactory.cs      # WebApplicationFactory setup
├── TeamEndpointTests.cs           # Team CRUD tests
├── UserEndpointTests.cs           # User profile tests
├── ContentCourseEndpointTests.cs  # Course management tests
└── AuthorizationTests.cs          # Policy enforcement tests
```

### Example Test

```csharp
public class TeamEndpointTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    private readonly string _authToken;
    
    public TeamEndpointTests(IntegrationTestFactory factory)
    {
        _client = factory.CreateClient();
        _authToken = factory.GenerateTestToken();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _authToken);
    }
    
    [Fact]
    public async Task CreateTeam_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = "Test Team",
            Code = "TEST"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", request);
        
        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);
        response.Headers.Location.Should().NotBeNull();
    }
}
```

### Testcontainers Setup

Tests use real SQL Server in Docker containers:

```csharp
public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    private readonly IContainer _container;
    private readonly string _connectionString;
    
    public IntegrationTestFactory()
    {
        _container = new MsSqlBuilder().Build();
        _container.Start();
        _connectionString = _container.GetConnectionString();
    }
    
    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<DbContextOptions<TimboLearnDbContext>>(options =>
                options.UseSqlServer(_connectionString));
        });
        
        return base.CreateHost(builder);
    }
}
```

---

## Testing with curl (Command Line)

### Get Test Token

```bash
curl -X POST http://localhost:5000/api/test-token \
  -H "Content-Type: application/json"
```

### Get User Profile

```bash
TOKEN="eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."

curl -X GET http://localhost:5000/api/users/me \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
```

### Create Team

```bash
curl -X POST http://localhost:5000/api/teams \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "DevOps Team",
    "code": "DEVOPS",
    "description": "Infrastructure and deployment automation"
  }'
```

---

## Testing Checklist

### Authentication & Authorization

- [ ] Test token generation works
- [ ] Unauthenticated requests return 401
- [ ] Invalid token returns 401
- [ ] Expired token returns 401
- [ ] TeamAdmin can manage teams
- [ ] TeamManager can manage teams
- [ ] Regular Member cannot manage teams
- [ ] Policy enforcement works correctly

### Team Management

- [ ] Create team (valid data)
- [ ] Create sub-team (with parentTeamId)
- [ ] Add user to team
- [ ] Get team hierarchy (recursive)
- [ ] Cross-functional membership (user in multiple teams)

### Content Course Management

- [ ] Create course (published and draft)
- [ ] Assign course to team
- [ ] Assign course to individual user
- [ ] AI course generation
- [ ] Update course details

### Edge Cases

- [ ] Duplicate team codes
- [ ] Circular team hierarchy (should be prevented)
- [ ] Assign course to non-existent team
- [ ] Add non-existent user to team
- [ ] SQL injection attempts (parameterized queries prevent this)

---

## Debugging Tips

### Enable Detailed Logging

Add to `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.EntityFrameworkCore": "Debug"
    }
  }
}
```

### View Database Contents

Install [DB Browser for SQLite](https://sqlitebrowser.org/) and open `src/TimboLearn.Api/timbolearn.db`.

### Capture Request/Response

Use browser DevTools Network tab or tools like:
- [Postman](https://www.postman.com/)
- [Insomnia](https://insomnia.rest/)
- [httpie](https://httpie.io/) (CLI tool)

---

## Next Steps

- **[Getting Started](GettingStarted.md)** - Initial setup and run
- **[Architecture](Architecture.md)** - Deep dive into design decisions
- **[Troubleshooting](Troubleshooting.md)** - Common issues and solutions
