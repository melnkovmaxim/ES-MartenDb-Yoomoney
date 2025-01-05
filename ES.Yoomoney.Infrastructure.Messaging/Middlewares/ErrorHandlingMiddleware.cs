using KafkaFlow;

namespace ES.Yoomoney.Infrastructure.Messaging.Middlewares;

internal sealed class ErrorHandlingMiddleware: IMessageMiddleware
{
    public async Task Invoke(IMessageContext context, MiddlewareDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error processing message {ex.Message}");
        }
    }
}