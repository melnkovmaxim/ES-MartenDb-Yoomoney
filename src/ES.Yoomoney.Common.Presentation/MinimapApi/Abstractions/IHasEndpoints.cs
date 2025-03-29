using Microsoft.AspNetCore.Routing;

namespace ES.Yoomoney.Common.Presentation.MinimapApi.Abstractions;

public interface IHasEndpoints
{
    void MapEndpoints(IEndpointRouteBuilder builder);
}