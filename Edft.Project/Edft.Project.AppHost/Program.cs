var builder = DistributedApplication.CreateBuilder(args);

//var apiService = builder.AddProject<Projects.Edft_Project_ApiService>("apiservice");
builder.AddBackendProjects();
//builder.AddProject<Projects.Edft_Project_Web>("webfrontend")
//    .WithExternalHttpEndpoints()
//    .WithReference(apiService)
//    .WaitFor(apiService);

builder.Build().Run();
