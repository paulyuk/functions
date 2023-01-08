# Plain-old Python App Before Modernizing to Azure Function
## Text Summarization using AI Cognitive Language Service

This sample shows how to take a text documents string (literal), does Text Summarization processing using the AI Congnitive Language service, and then outputs to standard out.  This shows you a working snippet of code that you can easily get running first before moving to Azure Functions.  

## Run on your local environment

### Pre-reqs
1) [Python 3.7 - 3.10](https://www.python.org/) required 
2) Once you have your Azure subscription, [create a Language resource](https://portal.azure.com/#create/Microsoft.CognitiveServicesTextAnalytics) in the Azure portal to get your key and endpoint. After it deploys, click Go to resource.
You will need the key and endpoint from the resource you create to connect your application to the API. You'll need to store the key and endpoint into the Env Vars or User Secrets code in a next step the quickstart.
You can use the free pricing tier (Free F0) to try the service, and upgrade later to a paid tier for production.
3) Export these secrets as Env Vars using values from Step 2.

Mac/Linux
```bash
export AI_URL=*Paste from step 2*
export AI_SECRET=*Paste from step 2*
```

Windows

Search for Environment Variables in Settings, create new System Variables similarly to [these instructions](https://docs.oracle.com/en/database/oracle/machine-learning/oml4r/1.5.1/oread/creating-and-modifying-environment-variables-on-windows.html#GUID-DD6F9982-60D5-48F6-8270-A27EC53807D0):

| Variable | Value |
| -------- | ----- |
| AI_URL | *Paste from step 4* |
| AI_SECRET | *Paste from step 4* |
6) [Azure Storage Explorer](https://azure.microsoft.com/en-us/products/storage/storage-explorer/) or storage explorer features of [Azure Portal](https://portal.azure.com)


### Using Functions CLI
1) Open a new terminal and do the following:

```bash
cd text_summarize
pip3 install -r requirements.txt
func start
```
You will see AI analysis happen in the Terminal standard out.  

## Next Step - Try in an Azure Function

Now you can try this same code running in an Azure Function - either on your local machine or in the cloud
https://github.com/paulyuk/functions/blob/main/ai/text_summarization/python/function_app/README.md
