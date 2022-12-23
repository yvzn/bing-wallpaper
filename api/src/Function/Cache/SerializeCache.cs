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
using Azure.Data.Tables;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function.Cache;

public static class SerializeCache
{
	[FunctionName("SerializeImageCache")]
	public static async Task RunAsync(
			[TimerTrigger("0 30 1 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[Table("ImageCache")]
			TableClient tableStorage,
			ILogger logger)
	{
		var latestImagesFromCache = new CacheService(tableStorage, logger).GetLatestImagesAsync(12);

		var imagesToSerialize = new List<object>();

		await foreach (var cachedImage in latestImagesFromCache)
		{
			imagesToSerialize.Add(new
			{
				copyright = cachedImage.Copyright,
				title = cachedImage.Title,
				lowResolution = cachedImage.Uri.ToLowResolution(),
				fullResolution = cachedImage.Uri.ToFullResolution(),
				market = cachedImage.Market,
			});
		}

		await new SerializeCacheService().Serialize(imagesToSerialize);
	}
}
