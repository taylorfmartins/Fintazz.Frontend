using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.BankAccounts;

public class BankAccountService(HttpClient http)
{
    public async Task<Result<List<BankAccountResponse>>> GetByHouseHoldAsync(Guid houseHoldId)
    {
        var response = await http.GetAsync($"api/bank-accounts/house-hold/{houseHoldId}");
        if (!response.IsSuccessStatusCode)
            return Result<List<BankAccountResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<BankAccountResponse>>();
        return Result<List<BankAccountResponse>>.Success(data ?? []);
    }

    public async Task<Result> CreateAsync(CreateBankAccountCommand command)
    {
        var response = await http.PostAsJsonAsync("api/bank-accounts", command);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateBankAccountRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/bank-accounts/{id}", request);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/bank-accounts/{id}");
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
