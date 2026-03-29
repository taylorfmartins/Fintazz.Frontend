namespace Fintazz.Web.Modules.Auth;

public record RegisterRequest(
    string FullName,
    string NickName,
    string Email,
    DateOnly BirthDate,
    string Password);
