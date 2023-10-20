using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace dotnet
{
    public class CreateThumbnail
    {
        private readonly ILogger<CreateThumbnail> _logger;

        public CreateThumbnail(ILogger<CreateThumbnail> logger)
        {
            _logger = logger;
        }

        const int thumbnailWidth = 1280;
        const int thumbnailHeight = 720;

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

    }
}
