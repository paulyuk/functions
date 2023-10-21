# Azure Functions
## Processing files and blobs - create image thumbnails (C#-Isolated, BlobClient SDK type)

This sample shows how to take image files as a input via BlobTrigger, processes the file to create thumbnails, and then outputs to another image file blob using BlobClient in the Azure SDK.  Note there is an alternate version in the `main` [branch](https://github.com/paulyuk/functions/tree/main/files/dotnet) showing how to use BlobOutput trigger

[![Open in GitHub Codespaces](https://github.com/codespaces/badge.svg)](https://github.com/codespaces/new?hide_repo_select=true&ref=main&repo=575770869)

## Run on your local environment

### Pre-reqs for local development
1) [.NET 6 SDK or higher](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2) [Azure Functions Core Tools](https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local?tabs=v4%2Cmacos%2Ccsharp%2Cportal%2Cbash#install-the-azure-functions-core-tools)
3) Add this `local.settings.json`` file to this /files/dotnet folder to simplify local development using azurite (storage emulator). 

```json
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "UseDevelopmentStorage=true",
        "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    }
}
```
4) [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/), [Azure Storage extension for VSCode](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azurestorage), or storage explorer features of [Azure Portal](https://portal.azure.com)
5) [Azurite](https://github.com/Azure/Azurite)

The easiest way to install Azurite is using a Docker container or the support built into Visual Studio.  To start container, enter this in a new Terminal:
```bash
docker run -d -p 10000:10000 -p 10001:10001 -p 10002:10002 mcr.microsoft.com/azure-storage/azurite
```

### Pre-reqs for Azure cloud development
6) An Azure [subsubcription](https://azure.microsoft.com/en-us/free)
7) [Azure Developer CLI](https://learn.microsoft.com/en-us/azure/developer/azure-developer-cli/get-started?tabs=localinstall&pivots=programming-language-csharp)


### Using Visual Studio
1) Open `dotnet.sln` using Visual Studio 2022 or later.
2) Press Run/F5 to run in the debugger
3) Open Storage Explorer, Storage Accounts -> Emulator -> Blob Containers -> and create a container `images` if it does not already exist
4) Copy any image file into the `images` container

You will see blob processing in Terminal standard out.  The result will be saved in a another image file in the `images-thumbnails` blob container.

### Using Functions CLI
1) Open a new terminal and do the following:

```bash
func start
```
2) Open Storage Explorer, Storage Accounts -> Emulator -> Blob Containers -> and create a container `images` if it does not already exist
3) Copy any image file into the `images` container

You will see blob processing in Terminal standard out.  The result will be saved in a another image file in the `images-thumbnails` blob container.

## Deploy to Azure

The easiest way to deploy this app is using the [Azure Dev CLI aka AZD](https://aka.ms/azd).  If you open this repo in GitHub CodeSpaces the AZD tooling is already preinstalled.

To provision and deploy:
```bash
azd up
```

## How it works

The main logic is contained in the [CreateThumbnail.cs](CreateThumbnail.cs).  When an image is uploaded to the blob container, the BlobTrigger will fire and call this function passing you the stream of what was uploaded.  The body of the function is our user code that handles the conversion of the blob stream to a smaller thumbnail (have fun and put any logic you want here).  Note in this version we pass a `BlobContainerClient client` SDK type into the function which gives you a working and initialized version of the client.  This gives you full access to all features in the Azure Storage Blob SDK.  We even use it to write/output/upload the blob at the end.  

Here is the most interesting code:
```csharp
        [Function(nameof(CreateThumbnail))]
        public async Task Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name, [BlobInput("images-thumbnails", Connection = "AzureWebJobsStorage")] BlobContainerClient client)
        {

            _logger.LogInformation($"Processing blob\n Name: {name} \n Data: {name.Length}");

            using (var image = Image.Load(stream))
            {

                // Generate thumbnail
                image.Mutate(async x => x.Resize(new ResizeOptions
                {
                    Size = new Size(thumbnailWidth, thumbnailHeight),
                    Mode = ResizeMode.Max
                }));

                var outputBlob = new MemoryStream();
                image.Save(outputBlob, new JpegEncoder()); 

                outputBlob.Position = 0;

                // Save the thumbnail to the output blob
                await client.UploadBlobAsync(name, outputBlob);
                _logger.LogInformation($"Finished processing blob\n Name:{name} and saved to output blob");

            }
        }
```
