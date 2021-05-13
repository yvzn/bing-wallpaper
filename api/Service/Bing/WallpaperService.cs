using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Ludeo.BingWallpaper.Model.Bing;

namespace Ludeo.BingWallpaper.Service.Bing
{
    internal class WallpaperService
    {
        internal static Uri bingHomepageUri = new Uri("https://www.bing.com");
        private static Uri imageArchiveUri = new Uri(bingHomepageUri, "HPImageArchive.aspx?format=js&idx=0&n=1");
        private static JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly HttpClient httpClient;

        public WallpaperService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        internal async Task<Uri> GetLatestWallpaperUriAsync()
        {
            var imageArchive = await GetImageArchiveAsync();
            return new Uri(bingHomepageUri, imageArchive.Images.FirstOrDefault().Url);
        }

        internal async Task<ImageArchive> GetImageArchiveAsync()
        {
            using var response = await this.httpClient.GetAsync(imageArchiveUri);
            using var stream = await response.Content.ReadAsStreamAsync();

            return await JsonSerializer.DeserializeAsync<ImageArchive>(stream, jsonSerializerOptions);
        }
    }
}
