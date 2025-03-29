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

builder.Build().Run();