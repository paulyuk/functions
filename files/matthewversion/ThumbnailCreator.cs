using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace matthewversion
{
    public class ThumbnailCreator
    {
        private readonly ILogger<ThumbnailCreator> _logger;

        public ThumbnailCreator(ILogger<ThumbnailCreator> logger)
        {
            _logger = logger;
        }

        [Function(nameof(ThumbnailCreator))]
        [BlobOutput("thumbnails/{name}", Connection = "AzureWebJobsStorage")]
        public async Task<Byte[]> Run([BlobTrigger("images/{name}", Connection = "")] Stream stream, string name, [BlobInput("out")] BlobContainerClient client)
        {
            // using var blobStreamReader = new StreamReader(stream);
            // var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");


            //approach 1 -- sdk type
            //do work here
            var outstream = new MemoryStream();
            //save
            await client.UploadBlobAsync(name, outstream);


            //approach 2 -- stream in, out
            return outstream.ToArray();
        }
    }
}
namespace CaseManager.BackgroundTasks
{
    public class ThumbnailCreator
    {
        //QueueTrigger instead?

        [FunctionName(nameof(ThumbnailCreator))]
        public async Task Run(
            [BlobTrigger(Constants.IMAGES_CONTAINER + "/{fileName}", Connection = Constants.BLOB_CONNECTION_NAME)] Stream input,
            string fileName,
            [Blob(Constants.THUMBNAILS_CONTAINER, FileAccess.Write)] BlobContainerClient thumbnailClient, ILogger log)
        {
            var extension = Path.GetExtension(fileName);
            var encoder = GetEncoder(extension);

            if (encoder != null)
            {
                var thumbnailWidth = Convert.ToInt32(Environment.GetEnvironmentVariable("THUMBNAIL_WIDTH") ?? "64");

                using (var output = new MemoryStream())
                using (var image = Image.Load(input))
                {
                    var divisor = image.Width / thumbnailWidth;
                    var height = Convert.ToInt32(Math.Round((decimal)(image.Height / divisor)));
                    image.Mutate(x => x.Resize(thumbnailWidth, height));
                    image.Save(output, encoder);
                    output.Position = 0;
                    await thumbnailClient.CreateIfNotExistsAsync();
                    await thumbnailClient.UploadBlobAsync(fileName, output);
                }
            }
            else
            {
                log.LogInformation($"No encoder support for: {fileName}");
            }

        }

        private static IImageEncoder GetEncoder(string extension)
        {
            IImageEncoder encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension.ToLower())
                {
                    case "png":
                        encoder = new PngEncoder();
                        break;
                    case "jpg":
                        encoder = new JpegEncoder();
                        break;
                    case "jpeg":
                        encoder = new JpegEncoder();
                        break;
                    case "gif":
                        encoder = new GifEncoder();
                        break;
                    default:
                        break;
                }
            }

            return encoder;
        }
    }
}