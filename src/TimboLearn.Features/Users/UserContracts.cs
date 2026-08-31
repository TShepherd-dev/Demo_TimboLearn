namespace TimboLearn.Features.Users;

public record UserProfileResponse(
    int Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAtUtc
);
