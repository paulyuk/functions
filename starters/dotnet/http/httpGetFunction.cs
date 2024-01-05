using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class httpGetFunction
    {
        private readonly ILogger _logger;

        public httpGetFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<httpGetFunction>();
        }

        [Function("httpget")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }

}
