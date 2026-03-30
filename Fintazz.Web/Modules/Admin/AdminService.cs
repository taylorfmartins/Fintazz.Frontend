using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.Admin;

public class AdminService(HttpClient http)
{
    public async Task<Result<JobStatsResponse>> GetJobStatsAsync()
    {
        var response = await http.GetAsync("api/admin/jobs/stats");
        if (!response.IsSuccessStatusCode)
            return Result<JobStatsResponse>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<JobStatsResponse>();
        return Result<JobStatsResponse>.Success(data!);
    }

    /// <summary>
    /// Cria um ticket de entrada e retorna a URL completa para o Hangfire Dashboard.
    /// O browser deve navegar para essa URL (nova aba) — o ticket é trocado por um session cookie e
    /// o usuário é redirecionado para /hangfire automaticamente.
    /// </summary>
    public async Task<Result<string>> GetHangfireEntryUrlAsync()
    {
        var response = await http.PostAsync("api/admin/hangfire-ticket", null);
        if (!response.IsSuccessStatusCode)
            return Result<string>.Failure(await ReadError(response));

        var data = await response.Content.ReadFromJsonAsync<TicketResponse>();
        var baseUrl = http.BaseAddress!.ToString().TrimEnd('/');
        return Result<string>.Success($"{baseUrl}/api/admin/hangfire-entry/{data!.Ticket}");
    }

    private static async Task<ApiError> ReadError(HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            return problem?.ToApiError() ?? new ApiError("Error", "Erro inesperado.");
        }
        catch { return new ApiError("Error", "Erro ao processar resposta."); }
    }

    public async Task<Result<List<UserSummaryResponse>>> GetUsersAsync()
    {
        var response = await http.GetAsync("api/admin/users");
        if (!response.IsSuccessStatusCode)
            return Result<List<UserSummaryResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<UserSummaryResponse>>();
        return Result<List<UserSummaryResponse>>.Success(data ?? []);
    }

    public async Task<Result> GrantAdminAsync(Guid userId)
    {
        var response = await http.PostAsync($"api/admin/users/{userId}/grant-admin", null);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> RevokeAdminAsync(Guid userId)
    {
        var response = await http.DeleteAsync($"api/admin/users/{userId}/revoke-admin");
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    private record TicketResponse(string Ticket);
}
