using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Ludeo.BingWallpaper.Tests.Function.Cache
{
	public static class CleanCache
	{
		[FunctionName("CleanImageCache")]
		public static async Task RunAsync(
			[TimerTrigger("0 30 1 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			await new CleanCacheService(tableStorage, logger).CleanAsync();
		}
	}
}
