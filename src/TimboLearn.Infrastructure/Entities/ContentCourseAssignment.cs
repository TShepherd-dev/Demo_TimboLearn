namespace TimboLearn.Infrastructure.Entities;

public enum AssignmentStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3
}

public class ContentCourseAssignment
{
    public Guid Id { get; set; }
    public Guid ContentCourseId { get; set; }
    public Guid? TargetUserId { get; set; }
    public Guid? TargetTeamId { get; set; }
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DueDateUtc { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.NotStarted;
    
    public ContentCourse ContentCourse { get; set; } = null!;
    public User? TargetUser { get; set; }
    public Team? TargetTeam { get; set; }
}
