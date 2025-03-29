using Npgsql;

namespace ES.Yoomoney.Infrastructure.Persistence.PostgreSql;

#pragma warning disable S125
#pragma warning disable CA1812

// ReSharper disable once UnusedType.Global
internal sealed class OutboxRepository(NpgsqlDataSource dataSource)
{
    // https://www.milanjovanovic.tech/blog/scaling-the-outbox-pattern
    // public Task AddMessageAsync(OutboxMessage message)
    // {
    //     
    // }
    //
    // public async Task GetUnprocessedMessagesAsync(CancellationToken ct)
    // {
    //     const int batchSize = 50;
    //     
    //     await using var connection = await dataSource.OpenConnectionAsync(ct);
    //
    //     var messages = await connection.QueryAsync<OutboxMessage>(
    //         $@"""
    //             SELECT *
    //             FROM outbox_messages
    //             WHERE 
    //             """);
    // }
    //
    // public Task MarkAsProcessedAsync(Guid messageId)
    // {
    //     
    // }
}
#pragma warning restore S125
#pragma warning disable CA1812