using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimboLearn.Infrastructure.Entities;

namespace TimboLearn.Infrastructure.Persistence;

public class TeamMembershipConfiguration : IEntityTypeConfiguration<TeamMembership>
{
    public void Configure(EntityTypeBuilder<TeamMembership> builder)
    {
        builder.ToTable("TeamMemberships");

        builder.HasKey(m => new { m.UserId, m.TeamId });

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(m => m.User)
            .WithMany(u => u.GroupMemberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Team)
            .WithMany(g => g.Memberships)
            .HasForeignKey(m => m.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.TeamId, m.Role });
    }
}
