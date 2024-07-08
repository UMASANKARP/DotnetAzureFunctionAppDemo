using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MyFunctionNamespace
{
    public class MyFunctionDemo
    {
        private readonly ILogger<MyFunctionDemo> _logger;

        public MyFunctionDemo(ILogger<MyFunctionDemo> logger)
        {
            _logger = logger;
        }

        [Function("MyFunctionDemo")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
