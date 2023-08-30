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
using System.Linq;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Azure.Data.Tables;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function
{
	public static class LastN
	{
		[FunctionName("GetLastNimages")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "last/{count:int?}")]
			HttpRequest req,
			int? count,
			[Table("ImageCache")]
			TableClient tableStorage,
			ILogger logger)
		{
			var latestImageFromCache = new CacheService(tableStorage, logger)
				.GetLatestImagesAsync(count.GetValueOrDefault(10));

			var result = new List<object>();

			await foreach (var cachedImage in latestImageFromCache)
			{
				result.Add(new
				{
					cachedImage.Copyright,
					cachedImage.Title,
					LowResolution = cachedImage.Uri.ToLowResolution(),
					FullResolution = cachedImage.Uri.ToFullResolution(),
					cachedImage.Market,
				});
			}

			if (result.Any())
			{
				return new OkObjectResult(result);
			}

			return new NotFoundResult();
		}
	}
}
