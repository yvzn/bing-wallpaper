using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Bing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Ludeo.BingWallpaper.Service.Cache
{
    public class ImageCacheService
    {
        private readonly CloudTable storage;
        private readonly ILogger logger;

        public ImageCacheService(CloudTable storage, ILogger logger)
        {
            this.storage = storage;
            this.logger = logger;
        }
        internal async Task UpdateCacheAsync(ImageArchive imageArchive)
        {
            var cachedImages = CreateCacheImages(imageArchive);

            try
            {
                foreach (var cachedImage in cachedImages)
                {
                    logger.LogInformation("Update cache with {LatestWallpaperUri} and RowKey={RowKey}", cachedImage.Uri, cachedImage.RowKey);

                    var operation = TableOperation.InsertOrReplace(cachedImage);
                    await storage.ExecuteAsync(operation);
                }
            }
            catch (StorageException ex)
            {
                logger.LogError(ex, "Failed to update cache");
            }
        }

        internal IEnumerable<CachedImage> CreateCacheImages(ImageArchive wallpaperImageArchive) =>
            wallpaperImageArchive.Images.Select(CreateCacheImage);

        private CachedImage CreateCacheImage(Image wallpaperImage) =>
            new CachedImage
            {
                PartitionKey = "cache",
                RowKey = wallpaperImage.StartDate,
                Copyright = wallpaperImage.Copyright,
                Title = wallpaperImage.Title,
                Uri = new Uri(WallpaperService.bingHomepageUri, wallpaperImage.Url)
            };
    }
}