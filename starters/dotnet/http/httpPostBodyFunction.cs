using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class HttpPostBody
    {
        private readonly ILogger _logger;

        public HttpPostBody(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpPostBody>();
        }

        [Function("httppostbody")]        
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
            [FromBody] Person person)
        {
            return new OkObjectResult(person);
        }
    }

    public record Person([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("age")] int Age);
}
