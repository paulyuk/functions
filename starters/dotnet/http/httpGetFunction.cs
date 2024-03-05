using System.Net;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
            response.WriteString("Welcome to Azure Functions!");  

            return response;
        }
    }

}
