using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.Categories;

public class CategoryService(HttpClient http)
{
    public async Task<Result<List<CategoryResponse>>> GetByHouseHoldAsync(Guid houseHoldId)
    {
        var response = await http.GetAsync($"api/categories/house-hold/{houseHoldId}");
        if (!response.IsSuccessStatusCode)
            return Result<List<CategoryResponse>>.Failure(await ReadError(response));
        var data = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
        return Result<List<CategoryResponse>>.Success(data ?? []);
    }

    public async Task<Result<Guid>> CreateAsync(CreateCategoryRequest request)
    {
        var response = await http.PostAsJsonAsync("api/categories", request);
        if (!response.IsSuccessStatusCode)
            return Result<Guid>.Failure(await ReadError(response));
        var created = await response.Content.ReadFromJsonAsync<CreatedResponse>();
        return Result<Guid>.Success(created!.Id);
    }

    public async Task<Result> UpdateAsync(Guid id, UpdateCategoryRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/categories/{id}", request);
        if (!response.IsSuccessStatusCode)
            return Result.Failure(await ReadError(response));
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var response = await http.DeleteAsync($"api/categories/{id}");
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
