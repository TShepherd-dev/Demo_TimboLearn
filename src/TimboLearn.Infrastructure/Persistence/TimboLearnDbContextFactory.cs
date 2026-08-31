using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using TimboLearn.Infrastructure.Persistence;

namespace TimboLearn.Infrastructure.Persistence.Design;

public class TimboLearnDbContextFactory : IDesignTimeDbContextFactory<TimboLearnDbContext>
{
    public TimboLearnDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TimboLearnDbContext>();
        optionsBuilder.UseSqlite("Data Source=timbolearn.db");
        
        return new TimboLearnDbContext(optionsBuilder.Options);
    }
}
