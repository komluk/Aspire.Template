using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Aspire.Hosting;

[ExcludeFromCodeCoverage]
public static partial class DistributedApplicationBuilderExtensions
{
    public static IDistributedApplicationBuilder AddBackendProjects(this IDistributedApplicationBuilder builder)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var projectTypes = assembly.GetTypes()
            .Where(t => t.IsClass &&
                        !t.IsAbstract &&
                        !t.Name.Contains("AppHost", StringComparison.OrdinalIgnoreCase) &&
                        t.Namespace != null &&
                        t.Namespace.StartsWith("Projects"));

        foreach (var projectType in projectTypes)
        {
            var projectInstance = Activator.CreateInstance(projectType) ??
                throw new InvalidOperationException($"Unable to create instance of type {projectType.Name}.");

            var projectPath = projectInstance.GetPropertyValue(projectType);
            if (projectPath is null)
                continue;

            builder.AddProject(GetSanitisedProjectName(projectType.Name), projectPath);
        }
        return builder;
    }

    private static string GetPropertyValue(this object? Instnace, Type projectType)
    {
        var property = projectType.GetProperty("ProjectPath");
        if (property is not null && property.CanRead)
        {
            var path = property.GetValue(Instnace);
#pragma warning disable CS8603 // Possible null reference return.
            return path is null ? throw new InvalidOperationException($"Project path {projectType.Name} not found.") : path.ToString();
#pragma warning restore CS8603 // Possible null reference return.
        }
        throw new InvalidOperationException($"Property is null. Cannot retrieve ProjectPath property value from {projectType.Name} instance.");
    }

    private static string GetSanitisedProjectName(string name) => ProjectName().Replace(name.Split("_")[^1], "").ToLower();

    [GeneratedRegex("[^a-zA-Z]")]
    private static partial Regex ProjectName();
}