namespace TimboLearn.Infrastructure.Entities;

public class Team
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentTeamId { get; set; }
    
    public Team? ParentTeam { get; set; }
    public ICollection<Team> SubTeams { get; set; } = new List<Team>();
    public ICollection<TeamMembership> Memberships { get; set; } = new List<TeamMembership>();
}
