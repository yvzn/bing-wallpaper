using System;
using System.Collections.Generic;
using System.Linq;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Service.Bing;

namespace Ludeo.BingWallpaper.Model.Cache
{
	internal static class Mapper {
		internal static IEnumerable<CachedImage> Map(ImageArchive wallpaperImageArchive) =>
			wallpaperImageArchive.Images.Select(Map);

		private static CachedImage Map(Image wallpaperImage) =>
			new CachedImage
			{
				PartitionKey = CachedImage.DefaultPartitionKey,
				RowKey = MapRowKey(wallpaperImage.StartDate),
				Copyright = wallpaperImage.Copyright,
				Title = wallpaperImage.Title,
				Uri = new Uri(WallpaperService.bingHomepageUri, wallpaperImage.Url).AbsoluteUri.ToString()
			};

		private static string MapRowKey(string? startDateString)
		{
			if (int.TryParse(startDateString, out var startDateInt))
			{
				// set cache RowKey to have most recent items on top
				return (99999999 - startDateInt).ToString();
			}
			return DateTime.Now.ToString("yyyyMMdd");
		}
	}
}
