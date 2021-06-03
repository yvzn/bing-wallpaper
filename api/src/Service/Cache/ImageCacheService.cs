using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Model.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache
{
    public class ImageCacheService
    {
        private readonly CloudTable tableStorage;
        private readonly ILogger logger;

        public ImageCacheService(CloudTable tableStorage, ILogger logger)
        {
            this.tableStorage = tableStorage;
            this.logger = logger;
        }
        internal async Task UpdateCacheAsync(ImageArchive imageArchive)
        {
            var cachedImages = Mapper.Map(imageArchive);

            var batchInsert = new TableBatchOperation();

            foreach (var cachedImage in cachedImages)
            {
                logger.LogInformation("Update cache with {LatestWallpaperUri} and RowKey={RowKey}", cachedImage.Uri, cachedImage.RowKey);

                var insertOrReplace = TableOperation.InsertOrReplace(cachedImage);
                batchInsert.Add(insertOrReplace);
            }

            await tableStorage.ExecuteBatchAsync(batchInsert);
        }
    }
}