using System.Collections.Generic;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache
{
    public class CleanCacheService
    {
        private readonly CloudTable tableStorage;
        private readonly ILogger logger;

        public CleanCacheService(CloudTable tableStorage, ILogger logger)
        {
            this.tableStorage = tableStorage;
            this.logger = logger;
        }

        internal async Task CleanAsync()
        {
            var batchDelete = new TableBatchOperation();

            await foreach (var rowKey in GetOutdatedCacheEntries())
            {
                logger.LogInformation("Clean outdated cache entry RowKey={RowKey}", rowKey);

                var imageToDelete = new CachedImage
                {
                    PartitionKey = CachedImage.DefaultPartitionKey,
                    RowKey = rowKey,
                    ETag = "*"
                };

                var deleteOperation = TableOperation.Delete(imageToDelete);
                batchDelete.Add(deleteOperation);
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
            var partitionKeyFilter = CacheService.GeneratePartitionKeyFilter();

            var allCacheEntriesQuery = new TableQuery<CachedImage>()
                .Where(partitionKeyFilter)
                .Select(new[] { nameof(CachedImage.RowKey) });

            var numberOfSkippedCacheEntries = 0;
            TableContinuationToken? continuationToken = default;

            do
            {
                var results = await tableStorage
                    .ExecuteQuerySegmentedAsync(allCacheEntriesQuery, continuationToken);
                continuationToken = results.ContinuationToken;

                foreach (var result in results)
                {
                    if (numberOfSkippedCacheEntries < CachedImage.NumberOfEntriesToKeep)
                    {
                        // skip the first n entries
                        ++numberOfSkippedCacheEntries;
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