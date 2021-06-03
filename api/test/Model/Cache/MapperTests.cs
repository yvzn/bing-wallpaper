using FluentAssertions;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Model.Cache;
using System.Linq;
using Xunit;

namespace Ludeo.BingWallpaper.Tests.Model.Cache
{
    public class MapperTest
    {
        public class Map
        {
            [Fact]
            public void Should_generate_default_PartitionKey()
            {
                // Given
                var imageArchive = new ImageArchive
                {
                    Images = new[]
                    {
                        new Image()
                    }
                };

                // When
                var actual = Mapper.Map(imageArchive);

                // Then
                actual.First().PartitionKey.Should().Be("cache");
            }

            [Fact]
            public void Should_generate_RowKey_from_StartDate()
            {
                // Given
                var startDate = 20210602.ToString();

                var imageArchive = new ImageArchive
                {
                    Images = new[]
                    {
                        new Image
                        {
                            StartDate = startDate
                        }
                    }
                };

                // When
                var actual = Mapper.Map(imageArchive);

                // Then
                var expected = (99999999 - 20210602).ToString();
                actual.First().RowKey.Should().Be(expected);
            }
        }
    }
}