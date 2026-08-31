using TimboLearn.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TimboLearn.Features.Users;

public interface IUserProfileService
{
    Task<UserProfileResponse> GetOrProvisionUserAsync(
        string externalId, 
        string email, 
        string name, 
        CancellationToken cancellationToken = default);
}

public class UserProfileService : IUserProfileService
{
    private readonly TimboLearnDbContext _dbContext;

    public UserProfileService(TimboLearnDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserProfileResponse> GetOrProvisionUserAsync(
        string externalId, 
        string email, 
        string name, 
        CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.ExternalIdentityId == externalId, cancellationToken);

        if (user == null)
        {
            var nameParts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.FirstOrDefault() ?? email.Split('@').First();
            var lastName = nameParts.Skip(1).LastOrDefault() ?? string.Empty;

            user = new User
            {
                Id = Guid.NewGuid(),
                ExternalIdentityId = externalId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new UserProfileResponse(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.CreatedAtUtc
        );
    }
}
