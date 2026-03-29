namespace Fintazz.Web.Shared;

public record ProblemDetails(
    string? Type,
    string? Title,
    int? Status,
    string? Detail,
    string? Instance)
{
    public ApiError ToApiError() =>
        new(Title ?? "Error", Detail ?? Title ?? "Ocorreu um erro inesperado.");
}
