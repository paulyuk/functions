# Azure Functions
## Processing files and blobs - create image thumbnails (C#-Isolated)

This sample shows how to take image files as a input via BlobTrigger, processes the file to create thumbnails, and then outputs to another image file blob using BlobOutput binding.  Note there is an alternate version in the `blobclient` [branch](https://github.com/paulyuk/functions/tree/blobclient/files/dotnet) showing how to use richer features of the Azure Storage blob SDK instead of simple output binding.  

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)

## Run on your local environment

### Pre-reqs for local development
1) [.NET 6 SDK or higher](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) [Azurite](https://github.com/Azure/Azurite)

The easiest way to install Azurite is using a Docker container or the support built into Visual Studio.  To start container, enter this in a new Terminal:
```bash
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

4) Add this `local.settings.json`` file to this /dotnet/files folder to simplify local development using azurite (storage emulator). 

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    }
}
```
5) [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) or storage explorer features of [Azure Portal](https://portal.azure.com)

### Pre-reqs for Azure cloud development
6) An Azure [subsubcription](https://azure.microsoft.com/en-us/free)
7) [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/get-started?tabs=localinstall&pivots=programming-language-csharp)


### Using Visual Studio
1) Open `dotnet.sln` using Visual Studio 2022 or later.
2) Press Run/F5 to run in the debugger
3) Open Storage Explorer, Storage Accounts -> Emulator -> Blob Containers -> and create a container `images` if it does not already exists
4) Copy any image file into the `images` container

You will see blob processing in Terminal standard out.  The result will be saved in a another image file in the `images-thumbnails` blob container.

### Using Functions CLI
1) Open a new terminal and do the following:

```bash
func start
```
2) Open Storage Explorer, Storage Accounts -> Emulator -> Blob Containers -> and create a container `images` if it does not already exists
3) Copy any image file into the `images` container

You will see blob processing in Terminal standard out.  The result will be saved in a another image file in the `images-thumbnails` blob container.

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```
