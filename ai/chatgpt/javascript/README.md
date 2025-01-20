# Azure Functions
## Chat using ChatGPT (Node.js JavaScript Function)

This sample shows how to take a ChatGPT prompt as HTTP Get or Post input, calculates the completions using OpenAI ChatGPT service, and then returns the output plus caches in a Blob state store.  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)
[![Open in Codeanywhere](https://codeanywhere.com/img/open-in-codeanywhere-btn.svg)](https://app.codeanywhere.com/#https://github.com/paulyuk/functions/ai/chatgpt/javascript)

## Run on your local environment

### Pre-reqs
1) [Node.js 18 or higher](https://www.nodejs.org/) 
2) [Azure Functions Core Tools 4.0.5198 or higher](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) [OpenAPI API key](https://platform.openai.com/account/api-keys) 
4) Export these secrets as Env Vars using values from Step 3.

Mac/Linux
```bash
export OPENAI_API_KEY=*Paste from step 3*
```

Windows

Search for Environment Variables in Settings, create new System Variables similarly to [these instructions](https://docs.oracle.com/en/database/oracle/machine-learning/oml4r/1.5.1/oread/creating-and-modifying-environment-variables-on-windows.html#GUID-DD6F9982-60D5-48F6-8270-A27EC53807D0):

| Variable | Value |
| -------- | ----- |
| OPENAI_API_KEY | *Paste from step 3* |

5) Add this local.settings.json file to the text_summarize folder to simplify local development and include Key from step 3
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "FUNCTIONS_WORKER_RUNTIME": "node",
    "OPENAI_API_KEY": "*Paste from step 3*"
  }
}
```

### Using Functions CLI
1) Open a new terminal and do the following:

```bash
npm install
func start
```
2) Using your favorite REST client, e.g. [RestClient in VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client), PostMan, curl, make a post.  `test.http` has been provided to run this quickly.   

Terminal:
```bash
curl -i -X POST http://localhost:7071/api/chat/ \
  -H "Content-Type: text/json" \
  --data-binary "@testdata.json"
```

testdata.json
```json
{
    "prompt": "Write a poem about Azure Functions.  Include two reasons why users love them."
}
```

test.http
```bash

POST http://localhost:7071/api/chat HTTP/1.1
content-type: application/json

{
    "prompt": "Write a poem about Azure Functions.  Include two reasons why users love them."
}
```

You will see chat happen in the Terminal standard out, the HTTP response, and saved off to a Blob for state management in the `samples-chatgpt-output` container.  

## Source Code

The key code that makes this work is as follows in `.src/functions/chat.js`.  You can customize this or learn more snippets using [Examples](https://platform.openai.com/examples) and [OpenAPI Playground](https://platform.openai.com/playground/).

```javascript
    completion = await openaiClient.createCompletion({
      model: "text-davinci-003",
      prompt: generatePrompt(prompt),
      temperature: 0.9,
      max_tokens: 200
    });
```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```
