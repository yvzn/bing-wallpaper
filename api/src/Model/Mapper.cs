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
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Service.Bing;

namespace Ludeo.BingWallpaper.Model.Cache
{
	internal class Mapper
	{
		internal static IEnumerable<CachedImage> Map(IEnumerable<(string market, ImageArchive archive)> imageArchives) =>
			imageArchives.SelectMany((entry, index) => new Mapper(entry.market, index).Map(entry.archive.Images));

		private readonly string market;
		private readonly int marketIndex;

		private Mapper(string market, int marketIndex)
		{
			this.market = market;
			this.marketIndex = marketIndex;
		}

		private IEnumerable<CachedImage> Map(IEnumerable<Image> wallpaperImages) =>
			wallpaperImages.Select(Map);

		private CachedImage Map(Image wallpaperImage) =>
			new CachedImage
			{
				PartitionKey = CachedImage.DefaultPartitionKey,
				RowKey = MapRowKey(wallpaperImage.StartDate),
				Copyright = wallpaperImage.Copyright,
				CopyrightLink = wallpaperImage.CopyrightLink,
				Title = wallpaperImage.Title,
				Uri = MapUri(wallpaperImage.UrlBase),
				Market = market,
			};

		private string MapRowKey(string? startDateString)
		{
			// sets cache RowKey to have most recent items on top
			var rowKey = 99999999;

			if (int.TryParse(startDateString, out var startDateInt))
			{
				rowKey = rowKey - startDateInt;
			}
			else
			{
				var now = DateTime.Now;
				var nowKey = now.Year * 10000 + now.Month * 100 + now.Day;
				rowKey = rowKey - nowKey;
			}

			return $"{rowKey}.{marketIndex}";
		}

		private static string MapUri(string? relativeUrl) =>
			new Uri(WallpaperService.bingHomepageUri, relativeUrl).AbsoluteUri.ToString();
	}
}
