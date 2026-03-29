using System.Text.Json.Serialization;

namespace Fintazz.Web.Modules.Categories;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryType { Income, Expense }

public record CategoryResponse(Guid Id, string? Name, CategoryType Type, Guid CreatedByUserId, bool IsSystem, Guid? ParentCategoryId);

public record CreateCategoryRequest(Guid HouseHoldId, string Name, CategoryType Type, Guid? ParentCategoryId = null);

public record UpdateCategoryRequest(string Name);
