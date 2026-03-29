using System.Net.Http.Json;
using Fintazz.Web.Shared;

namespace Fintazz.Web.Modules.Users;

public class UserService(HttpClient http)
{
    public async Task<Result<UserProfileResponse>> GetProfileAsync()
    {
        var response = await http.GetAsync("api/users/me");
        if (!response.IsSuccessStatusCode)
        {
            try
            {
                var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
                return Result<UserProfileResponse>.Failure(problem?.ToApiError() ?? new ApiError("Error", "Usuário não encontrado."));
            }
            catch { return Result<UserProfileResponse>.Failure("Erro ao carregar perfil."); }
        }
        var data = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        return Result<UserProfileResponse>.Success(data!);
    }
}
