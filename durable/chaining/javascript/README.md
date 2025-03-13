# Azure Functions
## Chaining with Durable Functions (JavaScript v4 Function)

This sample template provides a working example of chaining 3 functions in a reliable way using Durable Functions.  This is effectively the output of following this [tutorial](https://learn.microsoft.com/en-us/azure/azure-functions/durable/quickstart-js-vscode?pivots=nodejs-model-v4).  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)

## Run on your local environment

### Pre-reqs
1) [Node.js 20 or 18](https://www.nodejs.org/) 
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) Add this local.settings.json file to the text_summarize folder to simplify local development and include Key from step 3
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "node",
    "AzureWebJobsFeatureFlags": "EnableWorkerIndexing"
  }
}
```
4) Start Azurite storage emulator
```bash
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

### Using Functions CLI
1. Open a new terminal and do the following:

```bash
npm install
func start
```

### Test from Console client app

The `concept of a plan` here is to drive the orchestration using a separate console app.  The console app does indeed drive the orchestartion, show status async, and complete. What I wish it did is allow the orchestration/activities to grap the http context and write events to the output during activities.  

2. Open a new terminal and do the following:

```bash
cd client-console
npm install
npm start
```


### Test from Browser/HTTP
2. Test a Web hook or GET using the browser to open http://localhost:7071/api/http

3. Test a POST using your favorite REST client, e.g. [RestClient in VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client), PostMan, curl.  `test.http` has been provided to run this quickly.   

Terminal:
```bash
curl -i -X POST http://localhost:7071/api/orchestrators/chainingOrchestrator \
  -H "Content-Type: text/json" 
```


test.http
```bash

POST http://localhost:7071/api/orchestrators/chainingOrchestrator HTTP/1.1
content-type: application/json

{
}
```

### Using Visual Studio Code
1) Open this folder in a new terminal
2) Open VS Code by entering `code .` in the terminal
3) Press Run/Debug (F5) to run in the debugger
4) Use same approach above to test using an HTTP REST client

## Source Code

The key code that makes this work is as follows in [src/functions/chaining.js](src/functions/chaining.js).  

```javascript
const { app } = require('@azure/functions');
const df = require('durable-functions');
const activityName = 'chaining';

df.app.orchestration('chainingOrchestrator', function* (context) {
    const outputs = [];
    outputs.push(yield context.df.callActivity(activityName, 'Tokyo'));
    outputs.push(yield context.df.callActivity(activityName, 'Seattle'));
    outputs.push(yield context.df.callActivity(activityName, 'Cairo'));

    return outputs;
});

df.app.activity(activityName, {
    handler: (input) => {
        return `Hello, ${input}`;
    },
});

```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```
