/*
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services => {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();
*/

using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using Dynatrace.OpenTelemetry;
using Dynatrace.OpenTelemetry.Instrumentation.AzureFunctions.Core;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main()
    {
        // Initialize Dynatrace logging
        DynatraceSetup.InitializeLogging();

        // Build and run the host
        var host = new HostBuilder()
            .ConfigureFunctionsWebApplication() // Use ASP.NET Core integration
            .ConfigureServices(services =>
            {
                // Add OpenTelemetry tracing
                services.AddOpenTelemetry(builder =>
                {
                    builder
                        .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("YourServiceName"))
                        .AddAzureFunctionsCoreInstrumentation()
                        .AddDynatrace()
                        .AddHttpClientInstrumentation();
                });
            })
            .Build();

        host.Run();
    }
}


