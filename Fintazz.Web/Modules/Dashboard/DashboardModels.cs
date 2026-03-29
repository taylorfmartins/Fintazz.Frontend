namespace Fintazz.Web.Modules.Dashboard;

public record MonthlyBalanceResponse(
    decimal TotalIncome,
    decimal TotalExpense,
    decimal Balance,
    decimal TotalCreditCardInvoices,
    decimal BankAccountsTotalBalance);
