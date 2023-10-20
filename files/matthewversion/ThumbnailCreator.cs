using System.IO;
using System.Threading.Tasks;
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
        public async Task Run([BlobTrigger("images/{name}", Connection = "")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
