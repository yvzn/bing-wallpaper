using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function
{
	public static class LastN
	{
		[FunctionName("GetLastNimages")]
		public static async Task<IActionResult> RunAsync(
			[HttpTrigger(AuthorizationLevel.Function, "get", Route = "last/{count:int?}")]
			HttpRequest req,
			int? count,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			var latestImageFromCache = new CacheService(tableStorage, logger).GetLatestImagesAsync(count.GetValueOrDefault(10));

			var result = new List<object>();

			await foreach (var cachedImage in latestImageFromCache)
			{
				result.Add(new
				{
					Copyright = cachedImage.Copyright,
					Title = cachedImage.Title,
					Uri = cachedImage.Uri
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
