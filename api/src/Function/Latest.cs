using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function
{
	public static class Latest
	{
		[FunctionName("RedirectToLatest")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "redirection-to/latest")]
			HttpRequest req,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			var latestImageFromCache = new CacheService(tableStorage, logger).GetLatestImagesAsync(1);

			await foreach (var cachedImage in latestImageFromCache)
			{
				var latestWallpaperUri = cachedImage.Uri;

				if (!(latestWallpaperUri is null))
				{
					logger.LogInformation("Redirecting to {LatestWallpaperUri}", latestWallpaperUri);
					return new RedirectResult(latestWallpaperUri);
				}
			}

			return new NotFoundResult();
		}
	}
}
