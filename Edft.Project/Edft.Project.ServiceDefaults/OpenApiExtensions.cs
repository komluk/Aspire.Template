using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.Hosting;

[ExcludeFromCodeCoverage]
public static class OpenApiExtensions
{

    public static TBuilder AddSwagger<TBuilder>(this TBuilder builder, string componentName) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = $"Edft.Project.{componentName}",
                Version = "v1"
            });
        });
        builder.Services.AddOpenApi();
        return builder;
    }

    public static WebApplication UseSwaggerDefaults(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        return app;
    }

}
