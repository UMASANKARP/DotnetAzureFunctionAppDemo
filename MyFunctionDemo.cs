using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using OpenTelemetry.Trace;
using System.Diagnostics;

public class HttpExample
{
    private readonly TracerProvider _tracerProvider;

    public HttpExample(TracerProvider tracerProvider)
    {
        _tracerProvider = tracerProvider;
    }

    [Function("HttpExample")]
    public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req, FunctionContext ctx)
    {
        var parentContext = ExtractParentContext(req, ctx);
        return AzureFunctionsCoreInstrumentation.Trace(_tracerProvider, ctx.FunctionDefinition.Name, () => RunInternal(req), parentContext);
    }

    public HttpResponseData RunInternal(HttpRequestData req)
    {
        // Your actual handler code
        var response = req.CreateResponse();
        response.WriteString("Your result");
        return response;
    }

    private static ActivityContext ExtractParentContext(HttpRequestData req, FunctionContext ctx)
    {
        ActivityContext parent = default;

        // Extract parent context from HTTP headers
        PropagationContext propagationContext = Propagators.DefaultTextMapPropagator.Extract(
            default,
            req.Headers,
            (headers, name) =>
            {
                if (headers.TryGetValues(name, out var values))
                {
                    return values;
                }
                return null;
            });

        parent = propagationContext.ActivityContext;

        // If no parent context found, try using FunctionContext.TraceContext
        if (parent == default)
        {
            parent = ctx.TraceContext.ActivityContext;
        }

        return parent;
    }
}
