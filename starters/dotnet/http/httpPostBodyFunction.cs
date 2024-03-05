using System.Net;
using System.Text.Json.Serialization;
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
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {

            Person? person = null;
            
            if (req.Body.Length > 0) {
                person = await req.ReadFromJsonAsync<Person>();
                _logger.LogInformation($"Received Person with name: {person?.Name}");
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            if (person == null) {
                await response.WriteAsJsonAsync(new {});
            } else {
                await response.WriteAsJsonAsync(person);        
            }

            return response;
        }
    }

    public record Person([property: JsonPropertyName("name")] string Name, [property: JsonPropertyName("age")] int Age);
}
