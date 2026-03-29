using System.Text.Json.Serialization;

namespace Fintazz.Web.Modules.Transactions;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TransactionType { Income, Expense, Subscription }

public record TransactionResponse(
    Guid Id,
    Guid BankAccountId,
    string? BankAccountName,
    string? Description,
    decimal Amount,
    DateTime Date,
    TransactionType Type,
    bool IsPaid,
    Guid? CategoryId,
    string? CategoryName,
    bool? AutoRenew);

public record TransactionResponsePagedResult(
    List<TransactionResponse>? Items,
    long TotalCount,
    int Page,
    int PageSize,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage);

public record AddTransactionCommand(
    Guid HouseHoldId,
    Guid BankAccountId,
    string? Description,
    decimal Amount,
    DateTime Date,
    TransactionType Type,
    bool IsPaid,
    Guid? CategoryId);
