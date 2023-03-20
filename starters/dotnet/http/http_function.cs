using System.Net;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class HttpTrigger1
    {
        private readonly ILogger _logger;

        public HttpTrigger1(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HttpTrigger1>();
        }

        [Function("http")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string? name = HttpUtility.ParseQueryString(req.Url.Query)["name"];

            if (req.Body.Length > 0) {
                RequestPayload? payload = await req.ReadFromJsonAsync<RequestPayload>();
                name = payload?.Name;
            }

            string helloMsg = name is null
                ? "Welcome to Azure Functions!"
                : $"Welcome to Azure Functions, {name}!";

            _logger.LogInformation(helloMsg);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString(helloMsg);  

            return response;
        }
    }

    public record RequestPayload([property: JsonPropertyName("name")] string Name);
}
