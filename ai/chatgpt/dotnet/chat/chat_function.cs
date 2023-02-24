using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;

namespace AI_Functions
{
    public class chat_function
    {
        private readonly ILogger _logger;

        // must export and set these Env vars with your AI Cognitive Language resource values
        private static readonly string OPENAI_API_URL = Environment.GetEnvironmentVariable("OPENAI_API_URL") ?? "https://api.openai.com";
        private static readonly string OPENAI_API_KEY = Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? "SETENVVAR!";

        public chat_function(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<chat_function>();
        }
        

        [Function("chat")]
        public static async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("chat_function");
            logger.LogInformation("message logged");

            HttpResponseData response;

            try {
                var requestBodyJson = await new StreamReader(req.Body).ReadToEndAsync();

                var requestBody = JsonSerializer.Deserialize<PromptRequestBody>(requestBodyJson);

                string prompt = ""; 

                if ((requestBody == null)||(requestBody.prompt == null)) {
                    logger.LogError($"Missing value for prompt in request body.");
                    response = req.CreateResponse(HttpStatusCode.PartialContent);

                    return response;
                } else {
                    prompt = requestBody.prompt;
                }

            // return name != null
            //     ? (ActionResult)new OkObjectResult($"Hello, {name}")
            //     : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
            
                    var completions = await CreateCompletions(prompt, logger);
                    var choice = completions.choices[0];
                    logger.LogInformation($"Completions result: {choice}");

                    response = req.CreateResponse(HttpStatusCode.OK);
                    await response.WriteAsJsonAsync<Choice>(choice);
                }   
            catch (Exception ex)
            {
                logger.LogError($"Exception thrown: {ex.Message}");
                response = req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            return response;
        }

        static async Task<CompletionResponse> CreateCompletions(string prompt, ILogger logger)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            // Adding app id as part of the header
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {OPENAI_API_KEY}");

            var completion = CompletionRequest.CreateDefaultCompletionRequest(prompt);

            var completionJson = JsonSerializer.Serialize<CompletionRequest>(completion);
            var content = new StringContent(completionJson, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{OPENAI_API_URL}/v1/completions", content);
            //Console.WriteLine("Order passed: " + order);

            var responseContent = response.Content.ToString();

            logger.LogInformation("Response code: \n" + response.StatusCode.ToString());       
            logger.LogInformation("Response: \n" + responseContent);  

            var completionResponse = JsonSerializer.Deserialize<CompletionResponse>(responseContent??"");

            return completionResponse;     
        }


    }

    public record PromptRequestBody(string prompt);

    public record CompletionRequest(string model, 
                                    string prompt,
                                    double temperature,
                                    double max_tokens,
                                    double top_p,
                                    double frequency_penalty,
                                    double presence_penalty
                                    )
    {
        public static CompletionRequest CreateDefaultCompletionRequest(string prompt) {
            return new CompletionRequest("text-davinci-003", prompt, 0.9, 64, 1.0, 0.0, 0.0);
        }

        public static CompletionRequest CreateDefaultCompletionRequest() {
            return CompletionRequest.CreateDefaultCompletionRequest(prompt: "");
        }
    }

    public record CompletionResponse(string id, 
                                    [property: JsonPropertyName("object")] string _object,
                                    int created,    
                                    string model,
                                    Choice[] choices,
                                    Usage usage
                                    );

    public record Choice(string text, int index, string logprobs, string finish_reason);

    //public record Choices(Choice[] choices);

    public record Usage(int prompt_tokens, int completion_tokens, int total_tokens);


// curl https://api.openai.com/v1/completions \
//   -H "Content-Type: application/json" \
//   -H "Authorization: Bearer $OPENAI_API_KEY" \
//   -d '{
//   "model": "text-davinci-003",
//   "prompt": "Summarize this for a second-grade student:\n\nJupiter is the fifth planet from the Sun and the largest in the Solar System. It is a gas giant with a mass one-thousandth that of the Sun, but two-and-a-half times that of all the other planets in the Solar System combined. Jupiter is one of the brightest objects visible to the naked eye in the night sky, and has been known to ancient civilizations since before recorded history. It is named after the Roman god Jupiter.[19] When viewed from Earth, Jupiter can be bright enough for its reflected light to cast visible shadows,[20] and is on average the third-brightest natural object in the night sky after the Moon and Venus.",
//   "temperature": 0.7,
//   "max_tokens": 64,
//   "top_p": 1.0,
//   "frequency_penalty": 0.0,
//   "presence_penalty": 0.0
// }'
//
//{"id":"cmpl-6nORpx9l54RDOD25zbo9b0fTmi2Ri","object":"text_completion","created":1677230069,"model":"text-davinci-003","choices":[{"text":"\n\nJupiter is the fifth planet from the Sun and the biggest in our Solar System. It is very bright and can be seen in the night sky. It is named after the Roman god Jupiter. It is the third brightest object in the night sky after the Moon and Venus.","index":0,"logprobs":null,"finish_reason":"stop"}],"usage":{"prompt_tokens":151,"completion_tokens":57,"total_tokens":208}}

}
