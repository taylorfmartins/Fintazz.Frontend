namespace Fintazz.Web.Modules.BankAccounts;

public record BankAccountResponse(Guid Id, Guid HouseHoldId, string? Name, decimal InitialBalance, decimal CurrentBalance);

public record CreateBankAccountCommand(Guid HouseHoldId, string Name, decimal InitialBalance);

public record UpdateBankAccountRequest(string? Name, decimal? InitialBalance, decimal? CurrentBalance);
