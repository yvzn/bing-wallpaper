using System.Collections.Generic;
using System.Linq;

namespace Ludeo.BingWallpaper.Model.Bing
{
    internal class ImageArchive
    {
        public IEnumerable<Image> Images { get; set; } = Enumerable.Empty<Image>();
    }
}
