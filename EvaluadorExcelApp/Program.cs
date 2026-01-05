using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EvaluadorExcelApp;
using MudBlazor.Services;
using EvaluadorExcelApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddScoped<TransactionProcessor>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddSingleton<AuthService>();

await builder.Build().RunAsync();
