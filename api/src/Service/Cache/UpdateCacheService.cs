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

		internal async Task UpdateAsync(IEnumerable<CachedImage> imagesToCache)
		{
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
