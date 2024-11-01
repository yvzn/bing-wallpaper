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

using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Functions.Worker;

namespace Ludeo.BingWallpaper.Function.Cache;

public class UpdateCache(WallpaperService wallpaperService, HashingService hashingService, UpdateCacheService updateCacheService)
{
	[Function("UpdateImageCache")]
	public async Task RunAsync(
		[TimerTrigger("0 0 1 * * *"
#if DEBUG
			, RunOnStartup=true
#endif
		)]
		TimerInfo timerInfo)
	{
#if DEBUG
		await Task.Delay(millisecondsDelay: 10_000);
#endif

		var imageArchives = await wallpaperService.GetImageArchivesAsync();

		var imagesToCache = Mapper.Map(imageArchives);

		var imagesToCacheWithHash = await hashingService.HashAsync(imagesToCache);

		await updateCacheService.UpdateAsync(imagesToCacheWithHash);
	}
}
