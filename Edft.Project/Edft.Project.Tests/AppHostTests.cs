using Edft.Project.Tests.Extensions;
using Xunit;
using Xunit.Abstractions;

namespace Edft.Project.Tests;

public class AppHostTests(ITestOutputHelper testOutput)
{
    [Theory]
    [MemberData(nameof(AppHostAssemblies))]
    public async Task AppHostRunsCleanly(string appHostPath)
    {
        var appHost = await DistributedApplicationTestFactory.CreateAsync(appHostPath, testOutput);
        await using var app = await appHost.BuildAsync().WaitAsync(TimeSpan.FromSeconds(15));

        await app.StartAsync().WaitAsync(TimeSpan.FromSeconds(120));
        await app.WaitForResourcesAsync().WaitAsync(TimeSpan.FromSeconds(120));

        app.EnsureNoErrorsLogged();

        await app.StopAsync().WaitAsync(TimeSpan.FromSeconds(15));
    }

    public static TheoryData<string> AppHostAssemblies()
    {
        var appHostAssemblies = GetSamplesAppHostAssemblyPaths();
        var theoryData = new TheoryData<string, bool>();
        return new(appHostAssemblies.Select(p => Path.GetRelativePath(AppContext.BaseDirectory, p)));
    }

    private static IEnumerable<string> GetSamplesAppHostAssemblyPaths() =>
        Directory.GetFiles(AppContext.BaseDirectory, "*.AppHost.dll").Where(fileName => !fileName.EndsWith("Aspire.Hosting.AppHost.dll", StringComparison.OrdinalIgnoreCase));
}
