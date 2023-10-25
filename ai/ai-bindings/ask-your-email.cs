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
    public static class AskYourEmail
    {
        public record EmbeddingsRequest(string RawText, string FilePath);
        public record SemanticSearchRequest(string Prompt);

        // REVIEW: There are several assumptions about how the Embeddings binding and the SemanticSearch bindings
        //         work together. We should consider creating a higher-level of abstraction for this.
        [FunctionName("IngestEmail")]
        public static async Task<IActionResult> IngestEmail(
            [HttpTrigger(AuthorizationLevel.Function, "post")] EmbeddingsRequest req,
            [Embeddings("{FilePath}", InputType.FilePath, Model = "py-embedding-ada-002")] EmbeddingsContext embeddings,
            [SemanticSearch("KustoConnectionString", "Documents")] IAsyncCollector<SearchableDocument> output)
        {
            string title = Path.GetFileNameWithoutExtension(req.FilePath);
            await output.AddAsync(new SearchableDocument(title, embeddings));
            return new OkObjectResult(new { status = "success", title, chunks = embeddings.Count });
        }

        [FunctionName("PromptEmail")]
        public static IActionResult PromptEmail(
            [HttpTrigger(AuthorizationLevel.Function, "post")] SemanticSearchRequest unused,
            [SemanticSearch("KustoConnectionString", "Documents", Query = "{Prompt}", EmbeddingsModel = "py-embedding-ada-002", ChatModel = "py-gpt35turbo")] SemanticSearchContext result)
        {
            return new ContentResult { Content = result.Response, ContentType = "text/plain" };
        }

    }

}