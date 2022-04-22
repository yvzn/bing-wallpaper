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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Bing
{
	internal class WallpaperService
	{
		private static readonly string[] markets = new[] { "en-GB", "fr-FR", "en-US", "de-DE", "ja-JP" };
		internal static readonly Uri bingHomepageUri = new Uri("https://www.bing.com");
		private static readonly Uri imageArchiveUri = new Uri(bingHomepageUri, "HPImageArchive.aspx?format=js&idx=0");
		private readonly static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

		private readonly HttpClient httpClient;
		private readonly ILogger logger;

		public WallpaperService(HttpClient httpClient, ILogger logger)
		{
			this.httpClient = httpClient;
			this.logger = logger;
		}

		private async Task<ImageArchive> GetImageArchiveAsync(string market = "en-GB", int imageCount = 1)
		{
			var uriBuilder = new UriBuilder(imageArchiveUri);
			uriBuilder.Query = uriBuilder.Query + $"&mkt={market}&n={imageCount}";

			try
			{
				using var response = await this.httpClient.GetAsync(uriBuilder.Uri);
				using var stream = await response.Content.ReadAsStreamAsync();

				var imageArchive = await JsonSerializer.DeserializeAsync<ImageArchive>(stream, jsonSerializerOptions);
				if (imageArchive is not null)
				{
					return imageArchive;
				}

				logger.LogWarning("Failed to deserialize image archive {ImageArchiveUri}", uriBuilder.Uri);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to retrieve image archive for market {ImageArchiveUri}", uriBuilder.Uri);
			}
			return new ImageArchive();
		}

		internal async Task<IEnumerable<(string, ImageArchive)>> GetImageArchivesAsync()
		{
			var imageArchives = markets.Select(market => GetImageArchiveAsync(market)).ToList();

			await Task.WhenAll(imageArchives);

			return imageArchives
				.Zip(markets, (archive, market) => (market, archive.Result));
		}
	}
}
