using Azure;
using System;
using Azure.AI.TextAnalytics;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Example
{
    class Program
    {

        private static readonly AzureKeyCredential credentials = new AzureKeyCredential(Environment.GetEnvironmentVariable("AI_SECRET") ?? "SETENVVAR!");
        private static readonly Uri endpoint = new Uri(Environment.GetEnvironmentVariable("AI_URL") ?? "SETENVVAR!");

        // Method for summarizing text
        static async Task<string> AISummerizeText(TextAnalyticsClient client, string document, ILogger logger)
        {

            string summarizedText = "";
            
            // Prepare analyze operation input. You can add multiple documents to this list and perform the same
            // operation to all of them.
            var batchInput = new List<string>
            {
                document
            };

            TextAnalyticsActions actions = new TextAnalyticsActions()
            {
                ExtractSummaryActions = new List<ExtractSummaryAction>() { new ExtractSummaryAction() }
            };

            // Start analysis process.
            AnalyzeActionsOperation operation = await client.StartAnalyzeActionsAsync(batchInput, actions);
            await operation.WaitForCompletionAsync();
            // View operation status.
            summarizedText += $"AnalyzeActions operation has completed" + Newline();
            summarizedText += $"Created On   : {operation.CreatedOn}" + Newline();
            summarizedText += $"Expires On   : {operation.ExpiresOn}" + Newline();
            summarizedText += $"Id           : {operation.Id}" + Newline();
            summarizedText += $"Status       : {operation.Status}" + Newline();

            // View operation results.
            await foreach (AnalyzeActionsResult documentsInPage in operation.Value)
            {
                IReadOnlyCollection<ExtractSummaryActionResult> summaryResults = documentsInPage.ExtractSummaryResults;

                foreach (ExtractSummaryActionResult summaryActionResults in summaryResults)
                {
                    if (summaryActionResults.HasError)
                    {
                        logger.LogError($"  Error!");
                        logger.LogError($"  Action error code: {summaryActionResults.Error.ErrorCode}.");
                        logger.LogError($"  Message: {summaryActionResults.Error.Message}");
                        continue;
                    }

                    foreach (ExtractSummaryResult documentResults in summaryActionResults.DocumentsResults)
                    {
                        if (documentResults.HasError)
                        {
                            logger.LogError($"  Error!");
                            logger.LogError($"  Document error code: {documentResults.Error.ErrorCode}.");
                            logger.LogError($"  Message: {documentResults.Error.Message}");
                            continue;
                        }

                        summarizedText += $"  Extracted the following {documentResults.Sentences.Count} sentence(s):" + Newline();


                        foreach (SummarySentence sentence in documentResults.Sentences)
                        {
                            summarizedText += $"  Sentence: {sentence.Text}" + Newline();
                        }
                    }
                }
            }

            logger.LogInformation("Returning summarized text: " + Newline + summarizedText);
            return summarizedText;
        }

        static string Newline()
        {
            return "\r\n";
        }
        static async Task Main(string[] args)
        {
            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("NonHostConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger logger = loggerFactory.CreateLogger<Program>();

            string document = @"The extractive summarization feature uses natural language processing techniques to locate key sentences in an unstructured text document. 
                These sentences collectively convey the main idea of the document. This feature is provided as an API for developers. 
                They can use it to build intelligent solutions based on the relevant information extracted to support various use cases. 
                In the public preview, extractive summarization supports several languages. It is based on pretrained multilingual transformer models, part of our quest for holistic representations. 
                It draws its strength from transfer learning across monolingual and harness the shared nature of languages to produce models of improved quality and efficiency.";

            var client = new TextAnalyticsClient(endpoint, credentials);
            
            // analyze document text using Azure Cognitive Language Services
            var summarizedText = await AISummerizeText(client, document, logger);
            
            Console.WriteLine(summarizedText);
        }
    }
}
