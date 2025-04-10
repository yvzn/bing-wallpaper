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
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function;

public class Latest(CacheService cacheService, ILogger<Latest> logger)
{
	[Function("RedirectToLatest")]
	public async Task<IActionResult> RunAsync(
		[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "redirection-to/latest")]
		HttpRequest req)
	{
		var latestImageFromCache = cacheService.GetLatestImagesAsync(1);

		await foreach (var cachedImage in latestImageFromCache)
		{
			var latestWallpaperUri = cachedImage.Uri.ToFullResolution().ToString();

			logger.LogInformation("Redirecting to {LatestWallpaperUri}", latestWallpaperUri);
			return new RedirectResult(latestWallpaperUri);
		}

		return new StatusCodeResult(StatusCodes.Status410Gone);
	}
}
