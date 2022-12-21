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
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache
{
	internal class CacheService
	{
		private readonly TableClient tableStorage;
		private readonly ILogger logger;

		public CacheService(TableClient tableStorage, ILogger logger)
		{
			this.tableStorage = tableStorage;
			this.logger = logger;
		}

		public async IAsyncEnumerable<CachedImage> GetLatestImagesAsync(int count)
		{
			var allCacheEntriesQuery = tableStorage.QueryAsync<CachedImage>(
				cachedImage => cachedImage.PartitionKey == CachedImage.DefaultPartitionKey);

			var resultCount = 0;

			var duplicatedHashes = new HashSet<string>();

			await foreach (var cacheEntry in allCacheEntriesQuery)
			{
				if (cacheEntry.SimilarityHash is null)
				{
					++resultCount;
					yield return cacheEntry;
				}
				else if (!duplicatedHashes.Contains(cacheEntry.SimilarityHash))
				{
					duplicatedHashes.Add(cacheEntry.SimilarityHash);
					++resultCount;
					yield return cacheEntry;
				}
				if (resultCount >= count)
				{
					yield break;
				}
			}
		}
	}
}
