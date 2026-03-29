namespace Fintazz.Web.Modules.Users;

public record UserProfileResponse(
    Guid Id,
    string? FullName,
    string? NickName,
    string? Email,
    DateOnly BirthDate);
