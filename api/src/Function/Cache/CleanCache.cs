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
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Functions.Worker;

namespace Ludeo.BingWallpaper.Function.Cache;

public class CleanCache(CleanCacheService cleanCacheService)
{
	[Function("CleanImageCache")]
	public async Task RunAsync(
		[TimerTrigger("0 15 1 * * *"
#if DEBUG
			, RunOnStartup=true
#endif
		)]
		TimerInfo timerInfo)
	{
#if DEBUG
		await Task.Delay(millisecondsDelay: 15_000);
#endif

		await cleanCacheService.RemoveDuplicatesAsync();
		await cleanCacheService.CleanOldestAsync();
	}
}
