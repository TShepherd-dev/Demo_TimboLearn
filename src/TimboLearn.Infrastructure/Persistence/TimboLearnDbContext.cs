namespace TimboLearn.Infrastructure.Persistence;

public class TimboLearnDbContext : DbContext
{
    public TimboLearnDbContext(DbContextOptions<TimboLearnDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<TeamMembership> TeamMemberships { get; set; }
    public DbSet<ContentCourse> ContentCourses { get; set; }
    public DbSet<ContentCourseAssignment> ContentCourseAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new TeamMembershipConfiguration());
        modelBuilder.ApplyConfiguration(new ContentCourseConfiguration());
        modelBuilder.ApplyConfiguration(new ContentCourseAssignmentConfiguration());
    }
}
