using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.HouseHolds;

public class HouseHoldService(HttpClient http)
{
    public async Task<Result<List<HouseHoldResponse>>> GetAllAsync()
    {
        var response = await http.GetAsync("api/house-holds");
        if (!response.IsSuccessStatusCode)
            return Result<List<HouseHoldResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<HouseHoldResponse>>();
        return Result<List<HouseHoldResponse>>.Success(data ?? []);
    }

    public async Task<Result<Guid>> CreateAsync(CreateHouseHoldRequest request)
    {
        var response = await http.PostAsJsonAsync("api/house-holds", request);
        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadError(response));
        var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        return Result<Guid>.Success(created!.Id);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateHouseHoldRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/house-holds/{id}", request);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/house-holds/{id}");
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result<List<HouseHoldMemberResponse>>> GetMembersAsync(Guid id)
    {
        var response = await http.GetAsync($"api/house-holds/{id}/members");
        if (!response.IsSuccessStatusCode)
            return Result<List<HouseHoldMemberResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<HouseHoldMemberResponse>>();
        return Result<List<HouseHoldMemberResponse>>.Success(data ?? []);
    }

    public async Task<Result> RemoveMemberAsync(Guid id, Guid userId)
    {
        var response = await http.DeleteAsync($"api/house-holds/{id}/members/{userId}");
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result<Guid>> SendInviteAsync(Guid id, SendInviteRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/house-holds/{id}/invites", request);
        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadError(response));
        var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        return Result<Guid>.Success(created!.Id);
    }

    public async Task<Result> AcceptInviteAsync(string token)
    {
        var response = await http.PostAsync($"api/house-holds/invites/{token}/accept", null);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
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
}
