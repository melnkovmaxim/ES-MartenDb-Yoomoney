using ES.Yoomoney.Admin.Web.Components;
using ES.Yoomoney.Application.Extensions;
using ES.Yoomoney.Infrastructure.Clients.Extensions;
using ES.Yoomoney.Infrastructure.Persistence.Extensions;
using ES.Yoomoney.Infrastructure.Workers.Extensions;
using ES.Yoomoney.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDataSource("postgresdb");
builder.Services
    .AddLayerApplication()
    .AddLayerInfrastructurePersistence()
    .AddLayerInfrastructureWorkers()
    .AddLayerInfrastructureClients();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();