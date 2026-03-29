using Microsoft.JSInterop;

namespace Fintazz.Web.Auth;

public class TokenStorageService(IJSRuntime js)
{
    private const string AccessTokenKey = "fintazz_access_token";
    private const string RefreshTokenKey = "fintazz_refresh_token";

    public async Task<string?> GetAccessTokenAsync()
        => await js.InvokeAsync<string?>("localStorage.getItem", AccessTokenKey);

    public async Task<string?> GetRefreshTokenAsync()
        => await js.InvokeAsync<string?>("localStorage.getItem", RefreshTokenKey);

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await js.InvokeVoidAsync("localStorage.setItem", AccessTokenKey, accessToken);
        await js.InvokeVoidAsync("localStorage.setItem", RefreshTokenKey, refreshToken);
    }

    public async Task ClearAsync()
    {
        await js.InvokeVoidAsync("localStorage.removeItem", AccessTokenKey);
        await js.InvokeVoidAsync("localStorage.removeItem", RefreshTokenKey);
    }
}
