using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fintazz.Web.Modules.Auth;
using Microsoft.AspNetCore.Components;

namespace Fintazz.Web.Auth;

public class AuthorizationMessageHandler(
    TokenStorageService tokenStorage,
    FintazzAuthStateProvider authStateProvider,
    NavigationManager navigation) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await tokenStorage.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var refreshed = await TryRefreshAsync(cancellationToken);
            if (refreshed)
            {
                var newToken = await tokenStorage.GetAccessTokenAsync();
                var retry = new HttpRequestMessage(request.Method, request.RequestUri);
                foreach (var header in request.Headers)
                    retry.Headers.TryAddWithoutValidation(header.Key, header.Value);
                if (request.Content is not null)
                    retry.Content = request.Content;
                retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                return await base.SendAsync(retry, cancellationToken);
            }

            await tokenStorage.ClearAsync();
            authStateProvider.NotifyUserLoggedOut();
            navigation.NavigateTo("/login");
        }

        return response;
    }

    private async Task<bool> TryRefreshAsync(CancellationToken cancellationToken)
    {
        var refreshToken = await tokenStorage.GetRefreshTokenAsync();
        if (string.IsNullOrWhiteSpace(refreshToken))
            return false;

        try
        {
            var inner = InnerHandler;
            using var client = new HttpClient(inner!, false);
            var response = await client.PostAsJsonAsync(
                "api/auth/refresh",
                new RefreshTokenRequest(refreshToken),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
                return false;

            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
            if (auth is null)
                return false;

            await tokenStorage.SaveTokensAsync(auth.AccessToken, auth.RefreshToken);
            authStateProvider.NotifyUserLoggedIn(auth.AccessToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
