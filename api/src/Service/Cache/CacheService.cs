using System.Collections.Generic;
using Ludeo.BingWallpaper.Model.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache
{
	internal class CacheService
	{
		private readonly CloudTable tableStorage;
		private readonly ILogger logger;

		public CacheService(CloudTable tableStorage, ILogger logger)
		{
			this.tableStorage = tableStorage;
			this.logger = logger;
		}

		public async IAsyncEnumerable<CachedImage> GetLatestImagesAsync(int count)
		{
			var partitionKeyFilter = GeneratePartitionKeyFilter();

			var allCacheEntriesQuery = new TableQuery<CachedImage>()
				.Where(partitionKeyFilter);

			var resultCount = 0;

			TableContinuationToken? continuationToken = default;
			do
			{
				var results = await tableStorage
					.ExecuteQuerySegmentedAsync(allCacheEntriesQuery, continuationToken);

				continuationToken = results.ContinuationToken;

				for (var enumerator = results.GetEnumerator(); enumerator.MoveNext() && resultCount < count; ++resultCount)
				{
					yield return enumerator.Current;
				}
			} while (continuationToken != null);
		}

		internal static string GeneratePartitionKeyFilter()
		{
			return TableQuery.GenerateFilterCondition(
				nameof(CachedImage.PartitionKey),
				QueryComparisons.Equal,
				CachedImage.DefaultPartitionKey);
		}
	}
}
