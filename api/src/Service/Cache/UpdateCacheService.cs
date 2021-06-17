using System.Collections.Generic;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Model.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache
{
    public class UpdateCacheService
    {
        private readonly CloudTable tableStorage;
        private readonly ILogger logger;

        public UpdateCacheService(CloudTable tableStorage, ILogger logger)
        {
            this.tableStorage = tableStorage;
            this.logger = logger;
        }

        internal async Task UpdateAsync(ImageArchive imageArchive)
        {
            var imagesToCache = Mapper.Map(imageArchive);

            var batchInsert = new TableBatchOperation();

            foreach (var cachedImage in imagesToCache)
            {
                logger.LogInformation("Update cache with {LatestWallpaperUri} and RowKey={RowKey}", cachedImage.Uri, cachedImage.RowKey);

                var insertOperation = TableOperation.InsertOrReplace(cachedImage);
                batchInsert.Add(insertOperation);
            }

            await tableStorage.ExecuteBatchAsync(batchInsert);
        }
    }
}