# Azure Functions
## Starter template (Java Function)

This sample template provides an "empty starting point" function that is ready to run and deploy Azure easily.  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)

## Run on your local environment

### Pre-reqs
1) [Java 17 JDK and $JAVA_HOME set](https://www.microsoft.com/openjdk) 
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) [Maven](https://platform.openai.com/account/api-keys) 
4) Add this local.settings.json file to this folder to simplify local development and include Key from step 3
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "java",
    "JAVA_HOME": "/usr"
    }
}
```

### Using Functions CLI
1) Open a new terminal and do the following to do a clean build:

```bash
mvn clean package
```
2) Start the function app

```bash
mvn azure-functions:run
```

Note this is functionally equivent to doing a `func start` in the `./target/azure-functions/<function name>/` folder. 

## Source Code

The key code that makes this work is as follows in `./src/main/java/com/function/Function.java`.  You can customize this or learn more snippets using [Azure Functions Java developer guide](https://learn.microsoft.com/en-us/azure/azure-functions/functions-reference-java?tabs=bash%2Cconsumption).  

```java
    @FunctionName("HttpExample")
    public HttpResponseMessage run(
            @HttpTrigger(
                name = "req",
                methods = {HttpMethod.GET, HttpMethod.POST},
                authLevel = AuthorizationLevel.ANONYMOUS)
                HttpRequestMessage<Optional<String>> request,
            final ExecutionContext context) {
        context.getLogger().info("Java HTTP trigger processed a request.");

        // Parse query parameter
        final String query = request.getQueryParameters().get("name");
        final String name = request.getBody().orElse(query);

        if (name == null) {
            return request.createResponseBuilder(HttpStatus.BAD_REQUEST).body("Please pass a name on the query string or in the request body").build();
        } else {
            return request.createResponseBuilder(HttpStatus.OK).body("Hello, " + name).build();
        }
    }
```

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd) to provision Azure resources, and the Functions CLI to deploy.  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision all resources:
```bash
azd provision
```

Take note of the <APP_NAME> of the function app that was just provisioned. The default for this template
is `javafunc-1677374721012`. 

To deploy the appication:
```bash
func azure functionapp publish <APP_NAME>
```

Using defaults in this template:
```bash
func azure functionapp publish javafunc-1677374721012
```
