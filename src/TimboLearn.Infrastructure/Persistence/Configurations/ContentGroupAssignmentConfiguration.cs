using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimboLearn.Infrastructure.Entities;

namespace TimboLearn.Infrastructure.Persistence;

public class ContentCourseAssignmentConfiguration : IEntityTypeConfiguration<ContentCourseAssignment>
{
    public void Configure(EntityTypeBuilder<ContentCourseAssignment> builder)
    {
        builder.ToTable("ContentCourseAssignments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(a => a.ContentCourse)
            .WithMany(p => p.Assignments)
            .HasForeignKey(a => a.ContentCourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TargetUser)
            .WithMany()
            .HasForeignKey(a => a.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.TargetTeam)
            .WithMany()
            .HasForeignKey(a => a.TargetTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(a => a.TargetUserId);
        builder.HasIndex(a => a.TargetTeamId);
        builder.HasIndex(a => a.Status);
    }
}
