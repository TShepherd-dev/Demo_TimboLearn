using TimboLearn.Infrastructure.Entities;
using TimboLearn.Infrastructure.Persistence;

namespace TimboLearn.Infrastructure.SeedData;

public static class Seeder
{
    public static async Task SeedAsync(TimboLearnDbContext dbContext)
    {
        if (dbContext.Users.Any())
        {
            return;
        }

        var users = new List<User>
        {
            new() { ExternalIdentityId = "auth0|user1", Email = "alice.johnson@example.com", FirstName = "Alice", LastName = "Johnson", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user2", Email = "bob.smith@example.com", FirstName = "Bob", LastName = "Smith", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user3", Email = "carol.williams@example.com", FirstName = "Carol", LastName = "Williams", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user4", Email = "david.brown@example.com", FirstName = "David", LastName = "Brown", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user5", Email = "emma.davis@example.com", FirstName = "Emma", LastName = "Davis", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user6", Email = "frank.miller@example.com", FirstName = "Frank", LastName = "Miller", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user7", Email = "grace.wilson@example.com", FirstName = "Grace", LastName = "Wilson", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user8", Email = "henry.moore@example.com", FirstName = "Henry", LastName = "Moore", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user9", Email = "iris.taylor@example.com", FirstName = "Iris", LastName = "Taylor", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { ExternalIdentityId = "auth0|user10", Email = "jack.anderson@example.com", FirstName = "Jack", LastName = "Anderson", IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        };

        var teams = new List<Team>
        {
            new() { Name = "Engineering Team", Code = "ENG", Description = "Software development and engineering" },
            new() { Name = "Marketing Team", Code = "MKT", Description = "Marketing and communications" }
        };

        var contentCourses = new List<ContentCourse>
        {
            new() { Title = "Cybersecurity Hygiene for Remote Workers", Description = "Comprehensive training on cybersecurity best practices for remote teams", EstimatedDurationMinutes = 90, IsPublished = true },
            new() { Title = "Effective Communication in Virtual Teams", Description = "Learn to communicate effectively in distributed work environments", EstimatedDurationMinutes = 60, IsPublished = true },
            new() { Title = "Project Management Fundamentals", Description = "Introduction to modern project management methodologies and tools", EstimatedDurationMinutes = 120, IsPublished = false }
        };

        dbContext.Users.AddRange(users);
        dbContext.Teams.AddRange(teams);
        dbContext.ContentCourses.AddRange(contentCourses);

        await dbContext.SaveChangesAsync();

        var teamMemberships = new List<TeamMembership>
        {
            new() { UserId = users[0].Id, TeamId = teams[0].Id, Role = TeamRole.TeamAdmin },
            new() { UserId = users[1].Id, TeamId = teams[0].Id, Role = TeamRole.Member },
            new() { UserId = users[2].Id, TeamId = teams[0].Id, Role = TeamRole.Member },
            new() { UserId = users[3].Id, TeamId = teams[0].Id, Role = TeamRole.Member },
            new() { UserId = users[4].Id, TeamId = teams[0].Id, Role = TeamRole.Member },

            new() { UserId = users[5].Id, TeamId = teams[1].Id, Role = TeamRole.TeamAdmin },
            new() { UserId = users[6].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[7].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[8].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[9].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[0].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[1].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[2].Id, TeamId = teams[1].Id, Role = TeamRole.Member }
        };

        var contentCourseAssignments = new List<ContentCourseAssignment>
        {
            new() { ContentCourseId = contentCourses[0].Id, TargetTeamId = teams[0].Id, AssignedAtUtc = DateTime.UtcNow, Status = AssignmentStatus.NotStarted },
            new() { ContentCourseId = contentCourses[1].Id, TargetTeamId = teams[1].Id, AssignedAtUtc = DateTime.UtcNow, Status = AssignmentStatus.InProgress },
            new() { ContentCourseId = contentCourses[2].Id, TargetTeamId = teams[0].Id, AssignedAtUtc = DateTime.UtcNow, Status = AssignmentStatus.NotStarted }
        };

        dbContext.TeamMemberships.AddRange(teamMemberships);
        dbContext.ContentCourseAssignments.AddRange(contentCourseAssignments);

        await dbContext.SaveChangesAsync();
    }
}
