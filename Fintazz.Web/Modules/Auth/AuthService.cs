using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.Auth;

public class AuthService(HttpClient httpClient)
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            return Result<AuthResponse>.Failure(error ?? new ApiError("Error", "Erro ao fazer login."));
        }
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return Result<AuthResponse>.Success(auth!);
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/register", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            return Result.Failure(error ?? new ApiError("Error", "Erro ao cadastrar usuário."));
        }
        return Result.Success();
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshTokenRequest request)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/refresh", request);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiError>();
            return Result<AuthResponse>.Failure(error ?? new ApiError("Error", "Sessão expirada."));
        }
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return Result<AuthResponse>.Success(auth!);
    }
}
