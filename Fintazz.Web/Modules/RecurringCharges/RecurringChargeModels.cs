namespace Fintazz.Web.Modules.RecurringCharges;

public record RecurringChargeResponse(
    Guid Id,
    Guid HouseHoldId,
    string? Description,
    decimal Amount,
    int BillingDay,
    Guid? CategoryId,
    string? CategoryName,
    Guid? BankAccountId,
    string? BankAccountName,
    Guid? CreditCardId,
    string? CreditCardName,
    bool IsVariableAmount,
    bool AutoApprove,
    bool IsActive);

public record CreateRecurringChargeCommand(
    Guid HouseHoldId,
    string? Description,
    decimal Amount,
    int BillingDay,
    Guid? CategoryId,
    Guid? BankAccountId,
    Guid? CreditCardId,
    bool IsVariableAmount,
    bool AutoApprove);

public record UpdateRecurringChargeRequest(string? Description, decimal Amount, Guid? CategoryId);

public record ApproveRecurringChargeRequest(decimal? Amount);
