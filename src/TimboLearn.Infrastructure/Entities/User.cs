namespace TimboLearn.Infrastructure.Entities;

public class User
{
    public Guid Id { get; set; }
    public string ExternalIdentityId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public ICollection<TeamMembership> GroupMemberships { get; set; } = new List<TeamMembership>();
}
