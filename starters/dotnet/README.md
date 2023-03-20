# Azure Functions
## Starter template (.NET 7 C# Function)

This sample template provides an "empty starting point" function that is ready to run and deploy Azure easily.  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)

## Run on your local environment

### Pre-reqs
1) [.NET 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) required *and [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) is strongly recommended*
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) Add this local.settings.json file to the http folder to simplify local development
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  }
}
```
4) Start Azurite storage emulator
```bash
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

### Using Functions CLI
1) Open a new terminal and do the following:

```bash
cd http
func start
```

2) Test a Web hook or GET using the browser to open http://localhost:7071/api/http

3) Test a POST using your favorite REST client, e.g. [RestClient in VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client), PostMan, curl.  `test.http` has been provided to run this quickly.   

Terminal:
```bash
curl -i -X POST http://localhost:7071/api/chat/ \
  -H "Content-Type: text/json" \
  --data-binary "@testdata.json"
```

testdata.json
```json
{
  "name": "Awesome Developer"
}
```

test.http
```bash

POST http://localhost:7071/api/chat HTTP/1.1
content-type: application/json

{
  "name": "Awesome Developer"
}
```

### Using Visual Studio
1) Open `starter.sln` using Visual Studio 2022 or later.
2) Press Run/F5 to run in the debugger
3) Use same approach above to test using an HTTP REST client

### Using Visual Studio Code
1) Open this folder in a new terminal
2) Open VS Code by entering `code .` in the terminal
3) Press Run/Debug (F5) to run in the debugger
4) Use same approach above to test using an HTTP REST client

## Source Code

The key code that makes this work is as follows in `./http/http.cs`.  The async Run function is marked as an Azure Function using the Function attribute and naming `http`.  This code shows how to handle an ordinary Web hook GET or a POST that sends
a `name` value in the request body as JSON.  

```csharp
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
```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```
