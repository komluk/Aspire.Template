using Edft.Project.ApiService;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfiguration();
builder.AddServiceDefaults();
builder.AddSwagger(builder.Environment.ApplicationName);

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwaggerDefaults();
app.MapDefaultEndpoints();
app.MapWeatherForecastApi();

app.Run();
