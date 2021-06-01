using System.Net.Http;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Service.Bing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function
{
    public static class Latest
    {
        private static HttpClient httpClient = new HttpClient();

        private static WallpaperService wallpaperService = new WallpaperService(httpClient);

        [FunctionName("RedirectToLatest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "redirection-to/latest")] HttpRequest req,
            ILogger logger)
        {
            var latestWallpaperUri = await wallpaperService.GetLatestWallpaperUriAsync();
            logger.LogInformation("Redirecting to {LatestWallpaperUri}", latestWallpaperUri);

            return new RedirectResult(latestWallpaperUri.AbsoluteUri.ToString());
        }
    }
}
