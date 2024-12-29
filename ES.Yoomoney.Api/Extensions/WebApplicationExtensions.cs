using ES.Yoomoney.Api.Abstractions;

namespace ES.Yoomoney.Api.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<IHasEndpoints>();
        
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoints(app);
        }
    }
}
