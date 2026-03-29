using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.Transactions;

public class TransactionService(HttpClient http)
{
    public async Task<Result<TransactionResponsePagedResult>> GetByHouseHoldAsync(
        Guid houseHoldId,
        int page = 1,
        int pageSize = 20,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        var query = new List<string> { $"page={page}", $"pageSize={pageSize}" };
        if (startDate.HasValue) query.Add($"startDate={startDate.Value:O}");
        if (endDate.HasValue) query.Add($"endDate={endDate.Value:O}");
        var url = $"api/transactions/house-hold/{houseHoldId}?{string.Join("&", query)}";

        var response = await http.GetAsync(url);
        if (!response.IsSuccessStatusCode)
            return Result<TransactionResponsePagedResult>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<TransactionResponsePagedResult>();
        return Result<TransactionResponsePagedResult>.Success(data!);
    }

    public async Task<Result> CreateAsync(AddTransactionCommand command)
    {
        var response = await http.PostAsJsonAsync("api/transactions", command);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> PayAsync(Guid id)
    {
        var response = await http.PatchAsync($"api/transactions/{id}/pay", null);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/transactions/{id}");
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
