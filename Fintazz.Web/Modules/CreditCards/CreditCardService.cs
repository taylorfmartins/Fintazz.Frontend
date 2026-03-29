using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.CreditCards;

public class CreditCardService(HttpClient http)
{
    public async Task<Result<List<CreditCardResponse>>> GetByHouseHoldAsync(Guid houseHoldId)
    {
        var response = await http.GetAsync($"api/credit-cards/house-hold/{houseHoldId}");
        if (!response.IsSuccessStatusCode)
            return Result<List<CreditCardResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<CreditCardResponse>>();
        return Result<List<CreditCardResponse>>.Success(data ?? []);
    }

    public async Task<Result<CreditCardResponse>> GetByIdAsync(Guid id)
    {
        var response = await http.GetAsync($"api/credit-cards/{id}");
        if (!response.IsSuccessStatusCode)
            return Result<CreditCardResponse>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<CreditCardResponse>();
        return Result<CreditCardResponse>.Success(data!);
    }

    public async Task<Result<Guid>> CreateAsync(CreateCreditCardCommand command)
    {
        var response = await http.PostAsJsonAsync("api/credit-cards", command);
        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadError(response));
        var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        return Result<Guid>.Success(created!.Id);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCreditCardRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/credit-cards/{id}", request);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/credit-cards/{id}");
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> AddPurchaseAsync(AddCreditCardPurchaseCommand command)
    {
        var response = await http.PostAsJsonAsync("api/credit-cards/purchases", command);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> UpdatePurchaseAsync(Guid purchaseId, UpdateCreditCardPurchaseRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/credit-cards/purchases/{purchaseId}", request);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeletePurchaseAsync(Guid purchaseId)
    {
        var response = await http.DeleteAsync($"api/credit-cards/purchases/{purchaseId}");
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> MarkInstallmentAsPaidAsync(Guid purchaseId, Guid installmentId)
    {
        var response = await http.PostAsync($"api/credit-cards/purchases/{purchaseId}/installments/{installmentId}/pay", null);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result<CreditCardInvoiceResponse>> GetInvoiceAsync(Guid creditCardId, int month, int year)
    {
        var response = await http.GetAsync($"api/dashboards/credit-card/{creditCardId}/invoice?month={month}&year={year}");
        if (!response.IsSuccessStatusCode)
            return Result<CreditCardInvoiceResponse>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<CreditCardInvoiceResponse>();
        return Result<CreditCardInvoiceResponse>.Success(data!);
    }

    public async Task<Result> PayInvoiceAsync(Guid creditCardId, PayInvoiceRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/credit-cards/{creditCardId}/invoice/pay", request);
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
