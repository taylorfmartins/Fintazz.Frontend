using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.Dashboard;

public class DashboardService(HttpClient http)
{
    public async Task<Result<MonthlyBalanceResponse>> GetMonthlyBalanceAsync(Guid houseHoldId, int month, int year)
    {
        var response = await http.GetAsync(
            $"api/dashboards/monthly-balance/{houseHoldId}?month={month}&year={year}");
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                return Result<MonthlyBalanceResponse>.Failure(problem?.ToApiError() ?? new ApiError("Error", "Erro ao carregar dashboard."));
            }
            catch { return Result<MonthlyBalanceResponse>.Failure("Erro ao carregar dashboard."); }
        }
        var data = await response.Content.ReadFromJsonAsync<MonthlyBalanceResponse>();
        return Result<MonthlyBalanceResponse>.Success(data!);
    }
}
