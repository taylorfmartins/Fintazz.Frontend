using Fintazz.Web.Auth;
using Fintazz.Web.Modules.Auth;
using Fintazz.Web.Modules.BankAccounts;
using Fintazz.Web.Modules.Categories;
using Fintazz.Web.Modules.CreditCards;
using Fintazz.Web.Modules.Dashboard;
using Fintazz.Web.Modules.HouseHolds;
using Fintazz.Web.Modules.RecurringCharges;
using Fintazz.Web.Modules.Transactions;
using Fintazz.Web.Modules.Users;
using Fintazz.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<Fintazz.Web.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
var apiBaseUri = string.IsNullOrWhiteSpace(baseUrl)
    ? new Uri(builder.HostEnvironment.BaseAddress)
    : new Uri(baseUrl);

// Auth
builder.Services.AddScoped<TokenStorageService>();
builder.Services.AddScoped<FintazzAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<FintazzAuthStateProvider>());
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthorizationMessageHandler>();

// HttpClient
builder.Services.AddHttpClient("FintazzApi", client =>
    client.BaseAddress = apiBaseUri)
    .AddHttpMessageHandler<AuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("FintazzApi"));

// App state
builder.Services.AddScoped<AppState>();

// Serviços de domínio
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<HouseHoldService>();
builder.Services.AddScoped<BankAccountService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CreditCardService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddScoped<RecurringChargeService>();
builder.Services.AddScoped<DashboardService>();
builder.Services.AddScoped<UserService>();

// MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
