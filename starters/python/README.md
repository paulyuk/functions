# Azure Functions
## Starter template (Python v2 Function)

This sample template provides an "empty starting point" function that is ready to run and deploy Azure easily.  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)

## Run on your local environment

### Pre-reqs
1) [Python 3.8+](https://www.python.org/) 
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) Add this local.settings.json file to the text_summarize folder to simplify local development
```json
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "python",
    "AzureWebJobsFeatureFlags": "EnableWorkerIndexing",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true"
  }
}
```
4) Start Azurite storage emulator
```bash
docker run -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

### Using Functions CLI
1) Create python VEnv to keep dependencies tidy and avoid package collisions
```bash
python -m venv .venv
source .venv/bin/activate
```

2) Open a new terminal and do the following:

```bash
pip3 install -r requirements.txt
func start
```
3) Test a Web hook or GET using the browser to open http://localhost:7071/api/http

4) Test a POST using your favorite REST client, e.g. [RestClient in VS Code](https://marketplace.visualstudio.com/items?itemName=humao.rest-client), PostMan, curl.  `test.http` has been provided to run this quickly.   

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

The key code that makes this work is as follows in [function_app.py](function_app.py).  

```python
import azure.functions as func
import logging

app = func.FunctionApp()

# Learn more at aka.ms/pythonprogrammingmodel
@app.function_name(name="http")
@app.route(route="http")
def test_function(req: func.HttpRequest) -> func.HttpResponse:
     logging.info('Python HTTP trigger function processed a request.')

     name = req.params.get('name')
     if not name:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('name')

     if name:
        hello = f"Hello, {name}. This HTTP triggered function executed successfully."
        logging.info(hello)
        return func.HttpResponse(hello)
     else:
        hello = "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
        logging.info(hello)
        return func.HttpResponse(hello, status_code=200)
```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```
