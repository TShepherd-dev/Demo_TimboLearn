using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimboLearn.Infrastructure.Entities;

namespace TimboLearn.Infrastructure.Persistence;

public class ContentCourseConfiguration : IEntityTypeConfiguration<ContentCourse>
{
    public void Configure(EntityTypeBuilder<ContentCourse> builder)
    {
        builder.ToTable("ContentCourses");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(4000);

        builder.HasMany(p => p.Assignments)
            .WithOne(a => a.ContentCourse)
            .HasForeignKey(a => a.ContentCourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.IsPublished);
    }
}
