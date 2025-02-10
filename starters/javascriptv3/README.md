# Azure Functions
## Starter template (JavaScript v3 Function)

This sample template provides an "empty starting point" function that is ready to run and deploy Azure easily.  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869) [![Open in Codeanywhere](https://codeanywhere.com/img/open-in-codeanywhere-btn.svg)](https://app.codeanywhere.com/#https://github.com/paulyuk/functions/starters/javascriptv3)

## Run on your local environment

### Pre-reqs
1) [Node.js 16 or 18](https://www.nodejs.org/) 
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) Add this local.settings.json file to the text_summarize folder to simplify local development and include Key from step 3
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "node"
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
npm install
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

You will see chat happen in the Terminal standard out, the HTTP response, and saved off to a Blob for state management in the `samples-chatgpt-output` container.  

### Using Visual Studio Code
1) Open this folder in a new terminal
2) Open VS Code by entering `code .` in the terminal
3) Press Run/Debug (F5) to run in the debugger
4) Use same approach above to test using an HTTP REST client

## Source Code

The key code that makes this work is as follows in [http/index.js](http/index.js).  

```javascript
module.exports = async function (context, req) {
    context.log('JavaScript HTTP trigger function processed a request.');

    const name = (req.query.name || (req.body && req.body.name));
    const responseMessage = name
        ? "Hello, " + name + ". This HTTP triggered function executed successfully."
        : "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.";

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: responseMessage
    };
}
```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```
