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
        private static HttpClient httpClient = new HttpClient();

        private static WallpaperService wallpaperService = new WallpaperService(httpClient);

        [FunctionName("CacheImages")]
        public static async Task Run(
            [TimerTrigger("0 0 1 * * *")] TimerInfo timerInfo,
            [Table("ImageCache")] CloudTable storage,
            ILogger logger)
        {
            var imageArchive = await wallpaperService.GetImageArchiveAsync();

            await new ImageCacheService(storage, logger).UpdateCacheAsync(imageArchive);
        }
    }
}
