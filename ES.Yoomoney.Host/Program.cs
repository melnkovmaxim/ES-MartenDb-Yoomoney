using Projects;

var builder = DistributedApplication.CreateBuilder(args);
var postgresInstance = builder
    .AddPostgres("postgres")
    .WithHttpEndpoint(port: 7001, targetPort: 5432)
    .WithPgAdmin()
    .AddDatabase("postgresdb", "postgres");

_ = builder
    .AddProject<ES_Yoomoney_Api>("EsYoomoneyApi")
    .WithReference(postgresInstance)
    .WaitFor(postgresInstance);

_ = builder
    .AddProject<ES_Yoomoney_Admin_Web>("EsYoomoneyAdminWeb")
    .WithReference(postgresInstance)
    .WaitFor(postgresInstance);

builder.Build().Run();