namespace ES.Yoomoney.Api.Abstractions;

internal interface IHasEndpoints
{
    void MapEndpoints(IEndpointRouteBuilder builder);
}