namespace TimboLearn.Features.Users;

public record UserProfileResponse(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAtUtc
);
