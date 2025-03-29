using ES.Yoomoney.Common.Presentation.MinimapApi.Abstractions;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ES.Yoomoney.Common.Presentation.MinimapApi.Extensions;

public static class WebApplicationExtensions
{
    public static void MapApplicationEndpoints(this WebApplication app)
    {
        IEnumerable<IHasEndpoints> endpoints = app.Services.GetServices<IHasEndpoints>();

        foreach (IHasEndpoints endpoint in endpoints)
        {
            endpoint.MapEndpoints(app);
        }
    }
}