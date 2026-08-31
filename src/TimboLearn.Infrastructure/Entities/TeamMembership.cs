namespace TimboLearn.Infrastructure.Entities;

public enum TeamRole
{
    Member = 0,
    TeamManager = 1,
    TeamAdmin = 2
}

public class TeamMembership
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public TeamRole Role { get; set; }
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    
    public User User { get; set; } = null!;
    public Team Team { get; set; } = null!;
}
