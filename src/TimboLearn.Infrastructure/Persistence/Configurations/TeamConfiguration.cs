using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimboLearn.Infrastructure.Entities;

namespace TimboLearn.Infrastructure.Persistence;

public class TeamConfiguration : IEntityTypeConfiguration<Team>
{
    public void Configure(EntityTypeBuilder<Team> builder)
    {
        builder.ToTable("Teams");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(g => g.Code)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.HasOne(g => g.ParentTeam)
            .WithMany(g => g.SubTeams)
            .HasForeignKey(g => g.ParentTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(g => g.Memberships)
            .WithOne(m => m.Team)
            .HasForeignKey(m => m.TeamId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => g.Code)
            .IsUnique();
    }
}
