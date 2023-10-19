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
        [BlobOutput("thumbnails/{name}-thumb.jpg", Connection = "")] // Output binding to write the thumbnail to a blob
        public static async Task<Image> Run(
            [BlobTrigger("images/{name}", Connection = "")] Stream stream, // Input binding to read the image from a blob
            string name,
            ILogger log,
            Stream outputBlob)
        {
            using (var image = Image.Load(stream))
            {
                // Generate thumbnail
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(thumbnailWidth, thumbnailHeight),
                    Mode = ResizeMode.Max
                }));

                image.Save(outputBlob, new JpegEncoder()); 

                // Save the thumbnail to the output blob
                log.LogInformation($"C# Blob trigger function processed blob\n Name:{name} and saved to thumbnails/{name}-thumb.jpg");
                return image;
            }
        }

    }
}
