var builder = DistributedApplication.CreateBuilder(args);

var azureSecret = builder.Configuration["AZURE_CLIENT_SECRET"];

if (!string.IsNullOrEmpty(azureSecret))
{
    Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", azureSecret);
    Environment.SetEnvironmentVariable("AZURE_TENANT_ID", builder.Configuration["AZURE_TENANT_ID"]);
    Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", builder.Configuration["AZURE_CLIENT_ID"]);
}

var azureStorage = builder.AddAzureStorage("peakystorage")
    .AddTables("peakytables");

var server = builder.AddProject<Projects.PeakyFlow_Server>("server")
    .WithReference(azureStorage);

builder.AddProject<Projects.PeakyFlow_Aspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(server);

builder.Build().Run();
