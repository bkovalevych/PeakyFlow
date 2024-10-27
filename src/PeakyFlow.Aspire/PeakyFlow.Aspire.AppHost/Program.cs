var builder = DistributedApplication.CreateBuilder(args);

var azureStorage = builder.AddAzureStorage("peakystorage")
    .AddTables("peakytables");

var server = builder.AddProject<Projects.PeakyFlow_Server>("server")
    .WithReference(azureStorage);

builder.AddProject<Projects.PeakyFlow_Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(server);

builder.Build().Run();
