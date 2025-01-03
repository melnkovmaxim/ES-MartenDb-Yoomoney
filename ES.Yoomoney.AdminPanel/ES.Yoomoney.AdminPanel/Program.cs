using ES.Yoomoney.AdminPanel.Client.Pages;
using ES.Yoomoney.AdminPanel.Components;

var builder = WebApplication.CreateBuilder(args);

// https://www.youtube.com/watch?v=O7oaxFgNuYo
// builder.Services.AddSingleton<IProducer<string, string>>(new ProducerBuilder<string, string>(new ProducerConfig()
// //     {
// //         BootstrapServers = "kafka:9092",
// //         Acks = Acks.All,
// //         MessageTimeoutMs = 5000
// //     })
// //     .Build());
// //
// // builder.Services
// //     .AddLayerApplication()
// //     .AddLayerInfrastructureClients()
// //     .AddLayerInfrastructurePersistenceEventSourcing()
// //     .AddInfrastructureMessagingLayer();
// 
// // Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ES.Yoomoney.AdminPanel.Client._Imports).Assembly);

app.Run();