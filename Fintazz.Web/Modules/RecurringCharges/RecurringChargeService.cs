using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.RecurringCharges;

public class RecurringChargeService(HttpClient http)
{
    public async Task<Result<List<RecurringChargeResponse>>> GetByHouseHoldAsync(Guid houseHoldId)
    {
        var response = await http.GetAsync($"api/recurring-charges/house-hold/{houseHoldId}");
        if (!response.IsSuccessStatusCode)
            return Result<List<RecurringChargeResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<RecurringChargeResponse>>();
        return Result<List<RecurringChargeResponse>>.Success(data ?? []);
    }

    public async Task<Result<Guid>> CreateAsync(CreateRecurringChargeCommand command)
    {
        var response = await http.PostAsJsonAsync("api/recurring-charges", command);
        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadError(response));
        var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        return Result<Guid>.Success(created!.Id);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateRecurringChargeRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/recurring-charges/{id}", request);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/recurring-charges/{id}");
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> ReactivateAsync(Guid id)
    {
        var response = await http.PatchAsync($"api/recurring-charges/{id}/reactivate", null);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> ApproveAsync(Guid id, ApproveRecurringChargeRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/recurring-charges/{id}/approve", request);
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
