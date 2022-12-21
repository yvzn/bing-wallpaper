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
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Cache;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache;

public class CleanCacheService
{
	private readonly TableClient tableStorage;
	private readonly ILogger logger;

	public CleanCacheService(TableClient tableStorage, ILogger logger)
	{
		this.tableStorage = tableStorage;
		this.logger = logger;
	}

	internal async Task CleanOldestAsync()
	{
		var rowKeysToDelete = GetOutdatedCacheEntries();

		await DeleteCacheEntries(rowKeysToDelete);
	}

	internal async Task RemoveDuplicatesAsync()
	{
		var rowKeysToDelete = GetDuplicatedCacheEntries();

		await DeleteCacheEntries(rowKeysToDelete);
	}

	private async Task DeleteCacheEntries(IAsyncEnumerable<string> rowKeysToDelete)
	{
		var batchDelete = new List<Task>();

		await foreach (var rowKey in rowKeysToDelete)
		{
			logger.LogInformation("Clean cache entry RowKey={RowKey}", rowKey);

			var deleteOperation = tableStorage.DeleteEntityAsync(CachedImage.DefaultPartitionKey, rowKey);
			batchDelete.Add(deleteOperation);
		}

		await Task.WhenAll(batchDelete);
	}

	private async IAsyncEnumerable<string> GetOutdatedCacheEntries()
	{
		var allCacheEntriesQuery = tableStorage.QueryAsync<CachedImage>(
			filter: cachedImage => cachedImage.PartitionKey == CachedImage.DefaultPartitionKey,
			select: new[] { nameof(CachedImage.RowKey) });

		var numberOfSkippedCacheEntries = 0;

		await foreach (var result in allCacheEntriesQuery)
		{
			if (result.RowKey is null)
			{
				continue;
			}
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
	}

	internal async IAsyncEnumerable<string> GetDuplicatedCacheEntries()
	{
		var allCacheEntriesQuery = tableStorage.QueryAsync<CachedImage>(
			filter: cachedImage => cachedImage.PartitionKey == CachedImage.DefaultPartitionKey,
			select: new[] { nameof(CachedImage.RowKey), nameof(CachedImage.SimilarityHash), nameof(CachedImage.StartDate) });

		var oldestImagePerSimilarityHash = new Dictionary<string, CachedImage>();

		await foreach (var result in allCacheEntriesQuery)
		{
			if (result.SimilarityHash is null || result.RowKey is null)
			{
				continue;
			}
			else if (!oldestImagePerSimilarityHash.ContainsKey(result.SimilarityHash))
			{
				logger.LogDebug("Found new hash {SimilarityHash} ({RowKey} - {StartDate})", result.SimilarityHash, result.RowKey, result.StartDate);
				oldestImagePerSimilarityHash[result.SimilarityHash] = result;
			}
			else if (oldestImagePerSimilarityHash[result.SimilarityHash].StartDate == result.StartDate)
			{
				logger.LogDebug("Found duplicated hash, same date {SimilarityHash} ({RowKey} - {StartDate})", result.SimilarityHash, result.RowKey, result.StartDate);
				yield return result.RowKey;
			}
			else if (IsBefore(oldestImagePerSimilarityHash[result.SimilarityHash].StartDate, result.StartDate))
			{
				logger.LogDebug("Found hash with newer date {SimilarityHash} ({RowKey} - {StartDate})", result.SimilarityHash, result.RowKey, result.StartDate);
				logger.LogDebug("Will keep the existing {OldestRowKey}", oldestImagePerSimilarityHash[result.SimilarityHash].RowKey);
				yield return result.RowKey;
			}
			else
			{
				logger.LogDebug("Found hash with older date {SimilarityHash} ({RowKey} - {StartDate})", result.SimilarityHash, result.RowKey, result.StartDate);
				logger.LogDebug("Will keep the new find {OldestRowKey}", result.RowKey);

				var notSoOldImage = oldestImagePerSimilarityHash[result.SimilarityHash];
				oldestImagePerSimilarityHash[result.SimilarityHash] = result;

				yield return notSoOldImage.RowKey!;
			}
		}
	}

	private bool IsBefore(string? date1, string? date2)
	{
		if (date1 is null && date2 is null)
		{
			return true;
		}
		else if (date1 is null)
		{
			return true;
		}
		else if (date2 is null)
		{
			return false;
		}
		else
		{
			return date1.CompareTo(date2) <= 0;
		}
	}
}
