using System.Diagnostics.CodeAnalysis;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;

namespace Microsoft.Extensions.Hosting;

[ExcludeFromCodeCoverage]
public static class ConfigurationExtensions
{
    public static TBuilder AddConfiguration<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var environment = builder.Environment;
        builder.Configuration.AddDefaultConfiguration(environment);

        if (environment.IsEnvironment("Test"))
            return builder;

        var appConfigurationName = builder.Configuration["AppConfigurationName"]
            ?? throw new KeyNotFoundException("The 'AppConfigurationName' configuration value is missing. Ensure it is set in the environment variables or appsettings.json.");

        var keyVaultName = builder.Configuration["KeyVaultName"]
            ?? throw new KeyNotFoundException("The 'KeyVaultName' configuration value is missing. Ensure it is set in the environment variables or appsettings.json.");

        builder.Configuration.AddAzureConfiguration(appConfigurationName, keyVaultName);
        return builder;
    }

    private static IConfigurationBuilder AddDefaultConfiguration(this IConfigurationBuilder configurationBuilder, IHostEnvironment environment)
    {
        configurationBuilder
            .SetBasePath(environment.ContentRootPath)
            .AddEnvironmentVariables()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        if (environment.IsDevelopment())
            configurationBuilder.AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        return configurationBuilder;
    }

    private static IConfigurationBuilder AddAzureConfiguration(this IConfigurationBuilder configurationBuilder, string appConfigurationName, string keyVaultName)
    {
        var credentials = new DefaultAzureCredential();
        configurationBuilder
            .AddAzureKeyVault(new Uri($"https://{keyVaultName}.vault.azure.net/"), credentials);

        configurationBuilder
            .AddAzureAppConfiguration(config =>
            {
                config.Connect(new Uri($"https://{appConfigurationName}.azconfig.io"), credentials)
                    .Select(KeyFilter.Any)
                    .ConfigureKeyVault(kv => { kv.SetCredential(credentials); });
            });
        return configurationBuilder;
    }
}
