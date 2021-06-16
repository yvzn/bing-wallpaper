using System.Collections.Generic;
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

        internal async Task CleanCacheAsync()
        {
            var batchDelete = new TableBatchOperation();

            await foreach (string rowKey in GetOutdatedCacheEntries())
            {
                logger.LogInformation("Clean outdated cache entry RowKey={RowKey}", rowKey);

                var delete = TableOperation.Delete(
                    new CachedImage
                    {
                        PartitionKey = CachedImage.DefaultPartitionKey,
                        RowKey = rowKey,
                        ETag = "*"
                    });
                batchDelete.Add(delete);
            }

            if (batchDelete.Count > 0)
            {
                await tableStorage.ExecuteBatchAsync(batchDelete);
            }
            else
            {
                logger.LogDebug("No outdated cache entry to delete");
            }
        }

        private async IAsyncEnumerable<string> GetOutdatedCacheEntries()
        {
            var partitionKeyFilter = TableQuery.GenerateFilterCondition(
                nameof(CachedImage.PartitionKey),
                QueryComparisons.Equal,
                CachedImage.DefaultPartitionKey);

            var allCacheEntriesQuery = new TableQuery<CachedImage>()
                .Where(partitionKeyFilter)
                .Select(new[] { nameof(CachedImage.RowKey) });

            var numberOfValidEntries = 0;
            TableContinuationToken? continuationToken = default;

            do
            {
                var results = await tableStorage
                    .ExecuteQuerySegmentedAsync(allCacheEntriesQuery, continuationToken);
                continuationToken = results.ContinuationToken;

                foreach (var result in results)
                {
                    if (numberOfValidEntries < CachedImage.NumberOfEntriesToKeep)
                    {
                        // skip the first n entries
                        ++numberOfValidEntries;
                    }
                    else
                    {
                        yield return result.RowKey;
                    }
                }
            } while (continuationToken != null);
        }
    }
}