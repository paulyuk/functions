# Azure Functions
## Chat Your Data Data using AI Bindings for Functions (C#-InProc, C#-Isolated coming soon!)

This sample shows how to leverage new AI Functions Bindings: `TextCompletion`, `Embeddings`, and `SemanticSearch` to prompt and asks questions using AI.  These are part of these new [Bindings extension Nuget packages](https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Extensions.OpenAI)
- `TextCompletions` enables simple prompting using OpenAI.  
- `Embeddings` enables creating embeddings for raw text or entire files; the embedding value can then be stored in a vector database.  
- `SemanticSearch` does the actual upload of embeddings to a vector database, and then the consequent queries of it.  Currently the binding only supports using [Azure Data Explorer](https://techcommunity.microsoft.com/t5/azure-data-explorer-blog/azure-data-explorer-for-vector-similarity-search/ba-p/3819626) as the vector Database for similarity search.

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869) [![Open in Codeanywhere](https://codeanywhere.com/img/open-in-codeanywhere-btn.svg)](https://app.codeanywhere.com/#https://github.com/paulyuk/functions/ai/ai-bindings)

## Run on your local environment

### Pre-reqs
1) [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or higher required *and [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [VS Code](https://code.visualstudio.com/) is strongly recommended*
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) [Azurite](https://github.com/Azure/Azurite)

The easiest way to install Azurite is using a Docker container or the support built into Visual Studio:
```bash
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

4) Once you have your Azure subscription, [create an Azure Data Explorer instance](https://learn.microsoft.com/en-us/azure/data-explorer/create-cluster-and-database?tabs=free) in the Azure portal to get your key and endpoint. After it deploys, click Go to resource.  Note: if you perform `azd provision` or `azd up` per the section at the end of the tutorial, this resource will already be created.  
You will need the connection string for your Kusto database. It should look something like this with your resource name at the beginning and the default DB name replaced at the end:
```
"KustoConnectionString": "https://YOUR_RESOURCE.eastus2.kusto.windows.net/DEFAULTDATABASE; Fed=true; Accept=true"
``` 
You will need to create a table in it called `Documents` with the right schema. 

Create the `Documents` table with desired schema using this query in Azure Data Explorer:
```
.create table Documents (Id:string, Title:string, Text:string, Embeddings:dynamic, Timestamp:datetime)
```

5) Add this local.settings.json file to this folder to simplify local development.  Fill in the empty values.  This file will be gitignored to protect secrets from committing to your repo.  
```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet",
        "AZURE_OPENAI_KEY": "***",
        "AZURE_OPENAI_ENDPOINT": "https://***.openai.azure.com/",
        "AZURE_OPENAI_SERVICE": "***",
        "AZURE_OPENAI_CHATGPT_DEPLOYMENT": "***",
        "AZURE_OPENAI_EMBEDDINGS_DEPLOYMENT": "***",       
        "KustoConnectionString": "https://***.eastus2.kusto.windows.net/your-database-here; Fed=true; Accept=true"
    }
}
```

### Using Visual Studio
1) Open `ai-bindings.sln` using Visual Studio 2022 or later.
2) Press Run/F5 to run in the debugger

Use `test.http` along with your favorite REST client or extension to test.

### Using Functions CLI
1) Open a new terminal and do the following:

```bash
func start
```

Use `test.http` along with your favorite REST client or extension to test.

### Azure Data Explorer (Kusto)

Use this query to see all data/documents uploaded and the embeddings/vectors:
```
Documents
```

Use this query to reset all data/embeddings/data:
```
.clear table Documents data
```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```

`Note: azd provision + test.http currently works but we're still working on enabling the deployed function calling Kusto with right access rules`

## How it works

Looking at [ask-your-data.cs](src/ask-your-data.cs) in particular, we see two functions: `IngestData` and `PromptData`.

`IngestData` function is responsible for uploading your Data file or raw text, and converting it into embeddings using `Embeddings` binding attribute that will work later with a vector search.
Once the file is converted into embeddings, the embeddings are uploaded to the vector database using the `SemanticSearch` binding attribute.  

`PromptData` function is responsible for searching over the vector database, in this case Azure Data Explorer (Kusto), using a customizable `Prompt` as the query.  `SemanticSearch` binding attribute
is used again but this time we specify the embeddings model/deployment, the ChatGPT model/deployment, and the Query.  This connects the ChatGPT search to the vector database to search with your prompt. 

```csharp
[FunctionName("IngestData")]
public static async Task<IActionResult> IngestData(
    [HttpTrigger(AuthorizationLevel.Function, "post")] EmbeddingsRequest req,
    [Embeddings("{FilePath}", inputType: InputType.FilePath, 
    Model = "%AZURE_OPENAI_EMBEDDINGS_DEPLOYMENT%")] EmbeddingsContext embeddings,
    [SemanticSearch("KustoConnectionString", "Documents")] IAsyncCollector<SearchableDocument> output)
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
```

Learn more about the AI bindings in their respective NuGet pages:
[Learn more about Azure Functions AI Bindings](https://www.nuget.org/packages/Microsoft.Azure.WebJobs.Extensions.OpenAI)



