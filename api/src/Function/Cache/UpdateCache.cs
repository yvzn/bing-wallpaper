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
using System.Net.Http;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function.Cache
{
	public static class UpdateCache
	{
		private static readonly HttpClient httpClient = new HttpClient();

		[FunctionName("UpdateImageCache")]
		public static async Task RunOrchestrator(
			[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			var imageArchives = await context
				.CallActivityAsync<Dictionary<string, ImageArchive>>("UpdateImageCache_GetImages", default);

			var imagesToCache = Mapper.Map(imageArchives);

			var imagesToCacheWithHash = await context
				.CallActivityAsync<IEnumerable<CachedImage>>("UpdateImageCache_HashImages", imagesToCache);

			await context.CallActivityAsync("UpdateImageCache_UpdateCache", imagesToCacheWithHash);
		}

		[FunctionName("UpdateImageCache_GetImages")]
		public static Task<Dictionary<string, ImageArchive>> GetImagesAsync(
			[ActivityTrigger] object trigger,
			ILogger logger)
		{
			return new WallpaperService(httpClient, logger).GetImageArchivesAsync();
		}

		[FunctionName("UpdateImageCache_HashImages")]
		public static Task<IEnumerable<CachedImage>> HashImagesAsync(
			[ActivityTrigger]
			IEnumerable<CachedImage> images,
			ILogger logger)
		{
			return new HashingService(httpClient, logger).HashAsync(images);
		}

		[FunctionName("UpdateImageCache_UpdateCache")]
		public static Task UpdateCacheAsync(
			[ActivityTrigger]
			IEnumerable<CachedImage> images,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			return new UpdateCacheService(tableStorage, logger).UpdateAsync(images);
		}

		[FunctionName("UpdateImageCache_TimerTrigger")]
		public static async Task RunAsync(
			[TimerTrigger("0 0 1 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[DurableClient]
			IDurableOrchestrationClient starter)
		{
			await starter.StartNewAsync("UpdateImageCache", default);
		}
	}
}
