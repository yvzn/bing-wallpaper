/*
   Copyright 2021-2022 Yvan Razafindramanana

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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

			var duplicatedHashes = new HashSet<string>();

			TableContinuationToken? continuationToken = default;
			do
			{
				var results = await tableStorage
					.ExecuteQuerySegmentedAsync(allCacheEntriesQuery, continuationToken);

				continuationToken = results.ContinuationToken;

				for (var enumerator = results.GetEnumerator(); enumerator.MoveNext() && resultCount < count;)
				{
					if (enumerator.Current.SimilarityHash is null)
					{
						++resultCount;
						yield return enumerator.Current;
					}
					else if (!duplicatedHashes.Contains(enumerator.Current.SimilarityHash))
					{
						duplicatedHashes.Add(enumerator.Current.SimilarityHash);
						++resultCount;
						yield return enumerator.Current;
					}
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
