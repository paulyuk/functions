using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenAI;
using Microsoft.Azure.WebJobs.Extensions.OpenAI.Search;
using Microsoft.Azure.WebJobs.Extensions.OpenAI.Models;

namespace genai2
{
    public static class AskYourData
    {
        public record EmbeddingsRequest(string RawText, string FilePath);
        public record SemanticSearchRequest(string Prompt);

        [FunctionName("IngestData")]
        public static async Task<IActionResult> IngestData(
            [HttpTrigger(AuthorizationLevel.Function, "post")] EmbeddingsRequest req,
            [Embeddings("{FilePath}", inputType: InputType.FilePath, 
            Model = "%AZURE_OPENAI_EMBEDDINGS_DEPLOYMENT%")] EmbeddingsContext embeddings,
            [SemanticSearch("KustoConnectionString2", "Documents")] IAsyncCollector<SearchableDocument> output)
        {
            string title = Path.GetFileNameWithoutExtension(req.FilePath);
            await output.AddAsync(new SearchableDocument(title, embeddings));
            return new OkObjectResult(new { status = "success", title, chunks = embeddings.Count });
        }

        [FunctionName("PromptData")]
        public static IActionResult PromptData(
            [HttpTrigger(AuthorizationLevel.Function, "post")] SemanticSearchRequest unused,
            [SemanticSearch("KustoConnectionString", "Documents", 
            Query = "{Prompt}", 
            EmbeddingsModel = "%AZURE_OPENAI_EMBEDDINGS_DEPLOYMENT%", 
            ChatModel = "%AZURE_OPENAI_CHATGPT_DEPLOYMENT%")] 
            SemanticSearchContext result)
        {
            return new ContentResult { Content = result.Response, ContentType = "text/plain" };
        }

    }

}
