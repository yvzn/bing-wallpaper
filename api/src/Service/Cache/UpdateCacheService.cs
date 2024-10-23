/*
   Copyright 2021-2024 Yvan Razafindramanana

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
using Microsoft.Extensions.Azure;
using Azure;

namespace Ludeo.BingWallpaper.Service.Cache;

public class UpdateCacheService(IAzureClientFactory<TableClient> azureClientFactory, ILogger<UpdateCacheService> logger)
{
	private readonly TableClient tableStorage = azureClientFactory.CreateClient("ImageCacheTableClient");

	internal async Task UpdateAsync(IEnumerable<CachedImage> imagesToCache)
	{
		var batchInsert = new List<Task>();

		foreach (var cachedImage in imagesToCache)
		{
			logger.LogInformation("Update cache with {LatestWallpaperUri} and RowKey={RowKey}", cachedImage.Uri, cachedImage.RowKey);

			var insertOperation = AddToCacheAsync(cachedImage);

			batchInsert.Add(insertOperation);
		}

		await Task.WhenAll(batchInsert);
	}

	private async Task<Response?> AddToCacheAsync(CachedImage cachedImage)
	{
		try
		{
			return await tableStorage.AddEntityAsync(cachedImage);
		}
		catch (RequestFailedException ex)
		{
			logger.LogError(ex, "Failed to update cache with {LatestWallpaperUri} and RowKey={RowKey}", cachedImage.Uri, cachedImage.RowKey);
		}
		return default;
	}
}
