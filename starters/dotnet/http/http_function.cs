using System.Net;
using System.Text.Json.Serialization;
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

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            string name = query["name"] ?? "";

            if (req.Body.Length > 0) {
                RequestPayload? payload = await req.ReadFromJsonAsync<RequestPayload>();
                name = payload?.Name ?? name;
            }

            string helloMsg = "Welcome to Azure Functions!";

            if (name != "") {
                helloMsg = $"Welcome to Azure Functions, {name}!";
                _logger.LogInformation(helloMsg);
            }

            _logger.LogInformation(helloMsg);
            response.WriteString(helloMsg);  

            return response;
        }
    }

    public record RequestPayload([property: JsonPropertyName("name")] string Name);
}
