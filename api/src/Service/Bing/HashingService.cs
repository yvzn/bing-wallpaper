/*
   Copyright 2021-2021 Yvan Razafindramanana

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

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Cache;
using Microsoft.Extensions.Logging;
using System;
using CoenM.ImageHash.HashAlgorithms;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Ludeo.BingWallpaper.Service.Bing
{
	internal class HashingService
	{
		private HttpClient httpClient;
		private readonly ILogger logger;

		public HashingService(HttpClient httpClient, ILogger logger)
		{
			this.httpClient = httpClient;
			this.logger = logger;
		}

		internal async Task<IEnumerable<CachedImage>> HashAsync(IEnumerable<CachedImage> images) =>
			await Task.WhenAll(images.Select(HashAsync));

		private async Task<CachedImage> HashAsync(CachedImage cachedImage)
		{
			try
			{
				using var stream = await httpClient.GetStreamAsync(cachedImage.Uri?.ToLowResolution());
				using var image = Image.Load<Rgba32>(stream);
				var hashAlgorithm = new PerceptualHash();
				cachedImage.SimilarityHash = hashAlgorithm.Hash(image).ToString();
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Failed to compute hash of image {ImageUri}", cachedImage.Uri);
			}

			return cachedImage;
		}
	}
}
