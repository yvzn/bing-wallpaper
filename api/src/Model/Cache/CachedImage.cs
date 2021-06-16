using System;
using Microsoft.Azure.Cosmos.Table;

namespace Ludeo.BingWallpaper.Model.Cache
{
    public class CachedImage : TableEntity
    {
        internal static string DefaultPartitionKey = "cache";
        internal static int NumberOfEntriesToKeep = 10;

        public Uri? Uri { get; set; }
        public string? Title { get; set; }
        public string? Copyright { get; set; }
    }
}