namespace Fintazz.Web.Modules.CreditCards;

public record CreditCardResponse(
    Guid Id,
    string? Name,
    decimal TotalLimit,
    decimal AvailableLimit,
    int ClosingDay,
    int DueDay);

public record CreateCreditCardCommand(
    Guid HouseHoldId,
    string Name,
    decimal TotalLimit,
    int ClosingDay,
    int DueDay);

public record UpdateCreditCardRequest(
    string Name,
    decimal TotalLimit,
    int ClosingDay,
    int DueDay);

public record AddCreditCardPurchaseCommand(
    Guid CreditCardId,
    string? Description,
    decimal TotalAmount,
    DateTime PurchaseDate,
    int TotalInstallments,
    Guid? CategoryId = null);

public record UpdateCreditCardPurchaseRequest(string? Description, Guid? CategoryId);

public record CreditCardInvoiceResponse(
    decimal TotalAmount,
    int Month,
    int Year,
    List<InvoiceItemResponse>? Items);

public record InvoiceItemResponse(
    Guid PurchaseId,
    Guid InstallmentId,
    string? Description,
    DateTime PurchaseDate,
    decimal TotalPurchaseAmount,
    int CurrentInstallment,
    int TotalInstallments,
    decimal InstallmentAmount,
    DateTime DueDate,
    bool IsPaid);

public record PayInvoiceRequest(Guid BankAccountId, int Month, int Year);
