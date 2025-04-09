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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Bing;

public class WallpaperService(IHttpClientFactory httpClientFactory, ILogger<WallpaperService> logger)
{
	private static readonly string[] markets = ["en-GB", "fr-FR", "en-US", "de-DE", "ja-JP"];
	internal static readonly Uri bingHomepageUri = new("https://www.bing.com");
	private static readonly Uri imageArchiveUri = new(bingHomepageUri, "HPImageArchive.aspx?format=js&idx=0");
	private readonly static JsonSerializerOptions jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

	private readonly HttpClient httpClient = httpClientFactory.CreateClient();

	private async Task<ImageArchive> GetImageArchiveAsync(string market = "en-GB", int imageCount = 1)
	{
		var uriBuilder = new UriBuilder(imageArchiveUri);
		uriBuilder.Query += $"&mkt={market}&n={imageCount}";

		using var responseStreamCopy = new MemoryStream();

		try
		{
			using var response = await httpClient.GetAsync(uriBuilder.Uri);
			using var responseStream = await response.Content.ReadAsStreamAsync();

			await responseStream.CopyToAsync(responseStreamCopy);
			responseStreamCopy.Seek(0, SeekOrigin.Begin);

			var imageArchive = await JsonSerializer.DeserializeAsync<ImageArchive>(responseStreamCopy, jsonSerializerOptions);
			if (imageArchive is not null)
			{
				return imageArchive;
			}

			logger.LogWarning("Failed to deserialize image archive {ImageArchiveUri}", uriBuilder.Uri);
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to retrieve image archive for market {ImageArchiveUri}", uriBuilder.Uri);
			responseStreamCopy.Seek(0, SeekOrigin.Begin);
			logger.LogError("Image archive content {ImageArchiveUri} : {ResponseStreamCopy}", uriBuilder.Uri, Encoding.UTF8.GetString(responseStreamCopy.ToArray()));
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
