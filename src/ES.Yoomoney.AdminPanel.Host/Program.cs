using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var postgresInstance = builder
    .AddPostgres("postgres")
    .WithHttpEndpoint(port: 7001, targetPort: 5432)
    .WithPgAdmin()
    .AddDatabase("postgresdb", "postgres");

var kafkaInstance = builder
    .AddKafka("kafka")
    .WithKafkaUI();

_ = builder
    .AddProject<ES_Yoomoney_Api>("EsYoomoneyApi")
    .WithReference(postgresInstance)
    .WaitFor(postgresInstance)
    .WithReference(kafkaInstance)
    .WaitFor(kafkaInstance);

_ = builder
    .AddProject<ES_Yoomoney_AdminPanel>("EsYoomoneyAdmin")
    .WithReference(postgresInstance)
    .WaitFor(postgresInstance)
    .WithReference(kafkaInstance)
    .WithEnvironment("kafka-endpoint", () => $"{kafkaInstance.Resource.ConnectionStringExpression.ValueProviders.First().GetValueAsync().GetAwaiter().GetResult()}:{kafkaInstance.Resource.ConnectionStringExpression.ValueProviders.Skip(1).First().GetValueAsync().GetAwaiter().GetResult()}")
    .WaitFor(kafkaInstance);

builder.Build().Run();