using Npgsql;

namespace ES.Yoomoney.Infrastructure.Persistence.PostgreSql;

public sealed class PostgresqlConnectionFactory(string connectionString)
{
    public NpgsqlConnection Create() => new NpgsqlConnection(connectionString);
}