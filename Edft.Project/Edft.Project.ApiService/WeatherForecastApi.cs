namespace Edft.Project.ApiService;

public static class WeatherForecastApi
{
    public static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public static RouteGroupBuilder MapWeatherForecastApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/v1/weatherforecast");
        group.WithTags("weatherforecast");
        group.MapGet("/", GetpWeatherForecast)
            .Produces(StatusCodes.Status404NotFound)
            .Produces<WeatherForecast[]>();
        return group;
    }

    private static IResult GetpWeatherForecast() =>
        Results.Ok(Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            )).ToArray());
}


public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
