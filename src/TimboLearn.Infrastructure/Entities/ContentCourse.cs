namespace TimboLearn.Infrastructure.Entities;

public class ContentCourse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public ICollection<ContentCourseAssignment> Assignments { get; set; } = new List<ContentCourseAssignment>();
}
