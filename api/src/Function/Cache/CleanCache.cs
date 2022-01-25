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
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Function.Cache
{
	public static class CleanCache
	{
		private static readonly HttpClient httpClient = new HttpClient();

		[FunctionName("CleanImageCache")]
		public static async Task RunOrchestrator(
			[OrchestrationTrigger] IDurableOrchestrationContext context)
		{
			await context.CallActivityAsync("CleanImageCache_RemoveDuplicates", default);

			await context.CallActivityAsync("CleanImageCache_CleanOldest", default);
		}

		[FunctionName("CleanImageCache_RemoveDuplicates")]
		public static Task RemoveDuplicatesAsync(
			[ActivityTrigger]
			object trigger,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			return new CleanCacheService(tableStorage, logger).RemoveDuplicatesAsync();
		}

		[FunctionName("CleanImageCache_CleanOldest")]
		public static Task CleanOldestAsync(
			[ActivityTrigger]
			object trigger,
			[Table("ImageCache")]
			CloudTable tableStorage,
			ILogger logger)
		{
			return new CleanCacheService(tableStorage, logger).CleanOldestAsync();
		}

		[FunctionName("CleanImageCache_TimerTrigger")]
		public static async Task RunAsync(
			[TimerTrigger("0 30 1 * * *"
#if DEBUG
				, RunOnStartup=true
#endif
			)]
			TimerInfo timerInfo,
			[DurableClient]
			IDurableOrchestrationClient starter)
		{
			await starter.StartNewAsync("CleanImageCache", default);
		}
	}
}
