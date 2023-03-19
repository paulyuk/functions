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

            string name;
            //string reqstring = req.ReadAsString();
            string reqstring = "{ }";
            if ((req != null) && (reqstring?.Length > 0)) {
                RequestPayload payload = await req.ReadFromJsonAsync<RequestPayload>();
                name = payload?.Name ?? "";

                if (name != "") {
                    _logger.LogInformation($"Received: \n{name}\n");
                    response.WriteString($"Welcome to Azure Functions, {name}!.");  
                } else {
                    response.WriteString("Welcome to Azure Functions!"); 
                }
            } else {
                response.WriteString("Welcome to Azure Functions!"); 
            }

            return response;
        }
    }

    public record RequestPayload([property: JsonPropertyName("name")] string Name);
}
