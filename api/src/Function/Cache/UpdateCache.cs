using System.Net.Http;
using System.Threading.Tasks;
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

		private static readonly WallpaperService wallpaperService = new WallpaperService(httpClient);

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
			var imageArchive = await wallpaperService.GetImageArchiveAsync();

			await new UpdateCacheService(tableStorage, logger).UpdateAsync(imageArchive);
		}
	}
}
