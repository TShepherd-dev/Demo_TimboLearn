namespace TimboLearn.Infrastructure.Entities;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentTeamId { get; set; }
    
    public Team? ParentTeam { get; set; }
    public ICollection<Team> SubTeams { get; set; } = new List<Team>();
    public ICollection<TeamMembership> Memberships { get; set; } = new List<TeamMembership>();
}
