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
        [BlobOutput("images-thumbnails/thumbnail.jpg", Connection = "")]
        public async Task<Byte[]> Run([BlobTrigger("images/{name}", Connection = "")] Stream stream, string name)
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

                // Save the thumbnail to the output blob
                _logger.LogInformation($"Finished processing blob\n Name:{name} and saved to output blob");

                return outputBlob.ToArray();
            }
        }

    }
}
