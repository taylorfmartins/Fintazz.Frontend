namespace Fintazz.Web.Modules.Auth;

public record AuthResponse(string AccessToken, string RefreshToken, DateTime AccessTokenExpiresAt);
