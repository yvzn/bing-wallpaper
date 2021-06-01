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
        private readonly CloudTable tableStorage;
        private readonly ILogger logger;

        public ImageCacheService(CloudTable tableStorage, ILogger logger)
        {
            this.tableStorage = tableStorage;
            this.logger = logger;
        }
        internal async Task UpdateCacheAsync(ImageArchive imageArchive)
        {
            var cachedImages = CreateCacheImages(imageArchive);

            var batchInsert = new TableBatchOperation();

            foreach (var cachedImage in cachedImages)
            {
                logger.LogInformation("Update cache with {LatestWallpaperUri} and RowKey={RowKey}", cachedImage.Uri, cachedImage.RowKey);

                var insertOrReplace = TableOperation.InsertOrReplace(cachedImage);
                batchInsert.Add(insertOrReplace);
            }

            await tableStorage.ExecuteBatchAsync(batchInsert);
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