using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using TimboLearn.Infrastructure.Persistence;

namespace TimboLearn.Infrastructure.Persistence.Design;

public class TimboLearnDbContextFactory : IDesignTimeDbContextFactory<TimboLearnDbContext>
{
    public TimboLearnDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TimboLearnDbContext>();
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TimboLearn;Trusted_Connection=True;TrustServerCertificate=true;");
        
        return new TimboLearnDbContext(optionsBuilder.Options);
    }
}
