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

using System.Net.Http;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function.Cache
{
	public static class UpdateCache
	{
		private static readonly HttpClient httpClient = new HttpClient();

		[FunctionName("UpdateImageCache")]
		public static async Task RunAsync(
			[TimerTrigger("0 0 1 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			var imageArchives = await new WallpaperService(httpClient, logger).GetImageArchivesAsync();

			var imagesToCache = Mapper.Map(imageArchives);

			var imagesToCacheWithHash = await new HashingService(httpClient, logger).HashAsync(imagesToCache);

			await new UpdateCacheService(tableStorage, logger).UpdateAsync(imagesToCacheWithHash);
		}
	}
}
