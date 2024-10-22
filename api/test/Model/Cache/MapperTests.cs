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

using FluentAssertions;
using Ludeo.BingWallpaper.Model.Bing;
using Ludeo.BingWallpaper.Model.Cache;
using System.Collections.Generic;
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
				var imageArchives = new List<(string, ImageArchive)> {
					("en-US", new ImageArchive { Images = new [] { new Image { } } } ),
				};

				// When
				var actual = Mapper.Map(imageArchives);

				// Then
				actual.First().PartitionKey.Should().Be("cache");
			}

			[Fact]
			public void Should_generate_RowKey_from_StartDate()
			{
				// Given
				var startDate = 20210602.ToString();

				var imageArchives = new List<(string, ImageArchive)> {
					("en-US", new ImageArchive { Images = new [] { new Image { StartDate = startDate } } } ),
				};

				// When
				var actual = Mapper.Map(imageArchives).ToList();

				// Then
				var expected = $"{99999999 - 20210602}.0";
				actual.ElementAt(0).RowKey.Should().Be(expected);
			}

			[Fact]
			public void Should_generate_RowKey_from_market_index()
			{
				// Given
				var startDate = 20210602.ToString();

				var imageArchives = new List<(string, ImageArchive)> {
					("en-US", new ImageArchive { Images = new [] { new Image { StartDate = startDate } } } ),
					("en-GB", new ImageArchive { Images = new [] { new Image { StartDate = startDate } } } ),
				};

				// When
				var actual = Mapper.Map(imageArchives).ToList();

				// Then
				var expected = $"{99999999 - 20210602}.1";
				actual.ElementAt(1).RowKey.Should().Be(expected);
			}
		}
	}
}
