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

        var users = new User[]
        {
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), ExternalIdentityId = "auth0|user1", Email = "alice.johnson@example.com", FirstName = "Alice", LastName = "Johnson", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), ExternalIdentityId = "auth0|user2", Email = "bob.smith@example.com", FirstName = "Bob", LastName = "Smith", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), ExternalIdentityId = "auth0|user3", Email = "carol.williams@example.com", FirstName = "Carol", LastName = "Williams", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), ExternalIdentityId = "auth0|user4", Email = "david.brown@example.com", FirstName = "David", LastName = "Brown", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), ExternalIdentityId = "auth0|user5", Email = "emma.davis@example.com", FirstName = "Emma", LastName = "Davis", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), ExternalIdentityId = "auth0|user6", Email = "frank.miller@example.com", FirstName = "Frank", LastName = "Miller", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), ExternalIdentityId = "auth0|user7", Email = "grace.wilson@example.com", FirstName = "Grace", LastName = "Wilson", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), ExternalIdentityId = "auth0|user8", Email = "henry.moore@example.com", FirstName = "Henry", LastName = "Moore", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), ExternalIdentityId = "auth0|user9", Email = "iris.taylor@example.com", FirstName = "Iris", LastName = "Taylor", IsActive = true, CreatedAtUtc = DateTime.UtcNow },
            new() { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), ExternalIdentityId = "auth0|user10", Email = "jack.anderson@example.com", FirstName = "Jack", LastName = "Anderson", IsActive = true, CreatedAtUtc = DateTime.UtcNow }
        };

        var teams = new Team[]
        {
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Engineering Team", Code = "ENG", Description = "Software development and engineering" },
            new() { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Marketing Team", Code = "MKT", Description = "Marketing and communications" }
        };

        var teamMemberships = new TeamMembership[]
        {
            // Engineering Team (5 users: Alice, Bob, Carol, David, Emma)
            new() { UserId = users[0].Id, TeamId = teams[0].Id, Role = TeamRole.TeamAdmin },
            new() { UserId = users[1].Id, TeamId = teams[0].Id, Role = TeamRole.Member },
            new() { UserId = users[2].Id, TeamId = teams[0].Id, Role = TeamRole.Member },
            new() { UserId = users[3].Id, TeamId = teams[0].Id, Role = TeamRole.Member },
            new() { UserId = users[4].Id, TeamId = teams[0].Id, Role = TeamRole.Member },

            // Marketing Team (8 users: Frank, Grace, Henry, Iris, Jack + Alice, Bob, Carol from Engineering)
            new() { UserId = users[5].Id, TeamId = teams[1].Id, Role = TeamRole.TeamAdmin },
            new() { UserId = users[6].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[7].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[8].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[9].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[0].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[1].Id, TeamId = teams[1].Id, Role = TeamRole.Member },
            new() { UserId = users[2].Id, TeamId = teams[1].Id, Role = TeamRole.Member }
        };

        var contentCourses = new ContentCourse[]
        {
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000001"), Title = "Cybersecurity Hygiene for Remote Workers", Description = "Comprehensive training on cybersecurity best practices for remote teams", EstimatedDurationMinutes = 90, IsPublished = true },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000002"), Title = "Effective Communication in Virtual Teams", Description = "Learn to communicate effectively in distributed work environments", EstimatedDurationMinutes = 60, IsPublished = true },
            new() { Id = Guid.Parse("20000000-0000-0000-0000-000000000003"), Title = "Project Management Fundamentals", Description = "Introduction to modern project management methodologies and tools", EstimatedDurationMinutes = 120, IsPublished = false }
        };

        var contentCourseAssignments = new ContentCourseAssignment[]
        {
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000001"), ContentCourseId = contentCourses[0].Id, TargetTeamId = teams[0].Id, AssignedAtUtc = DateTime.UtcNow, Status = AssignmentStatus.NotStarted },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000002"), ContentCourseId = contentCourses[1].Id, TargetTeamId = teams[1].Id, AssignedAtUtc = DateTime.UtcNow, Status = AssignmentStatus.InProgress },
            new() { Id = Guid.Parse("30000000-0000-0000-0000-000000000003"), ContentCourseId = contentCourses[2].Id, TargetTeamId = teams[0].Id, AssignedAtUtc = DateTime.UtcNow, Status = AssignmentStatus.NotStarted }
        };

        dbContext.Users.AddRange(users);
        dbContext.Teams.AddRange(teams);
        dbContext.TeamMemberships.AddRange(teamMemberships);
        dbContext.ContentCourses.AddRange(contentCourses);
        dbContext.ContentCourseAssignments.AddRange(contentCourseAssignments);

        await dbContext.SaveChangesAsync();
    }
}
