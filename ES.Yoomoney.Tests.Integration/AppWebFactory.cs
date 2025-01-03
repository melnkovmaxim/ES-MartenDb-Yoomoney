using System.Reflection;
using Aspire.Npgsql;
using ES.Yoomoney.Api;
using Marten;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace ES.Yoomoney.Tests.Integration;

public sealed class AppWebFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    public T Resolve<T>() where T : notnull => Services.GetRequiredService<IServiceScopeFactory>()
        .CreateScope().ServiceProvider.GetRequiredService<T>();
    
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithPassword("Strong_password_123!")
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConnectionStrings:postgresdb", _dbContainer.GetConnectionString());
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}