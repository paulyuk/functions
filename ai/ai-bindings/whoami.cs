using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebJobs.Extensions.OpenAI;
using WebJobs.Extensions.OpenAI.Search;
using OpenAI.ObjectModels.ResponseModels;

namespace genai2
{
    public static class WhoIs
    {
        [FunctionName(nameof(WhoIsFunction))]
        public static string WhoIsFunction(
            [HttpTrigger(AuthorizationLevel.Function, Route = "whois/{name}")] HttpRequest req,
            [WebJobs.Extensions.OpenAI.TextCompletion("Who is {name}?")] CompletionCreateResponse response)
        {
            return response.Choices[0].Text;
        }

    }

}
