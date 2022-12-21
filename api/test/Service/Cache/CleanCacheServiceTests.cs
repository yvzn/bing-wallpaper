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

using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Ludeo.BingWallpaper.Tests.Service.Cache;

public class CleanCacheServiceTests
{
	public class GetDuplicatedCacheEntries
	{
		[Fact]
		public async Task Should_return_empty_list_when_no_cache_entries()
		{
			// Given
			var tableStorageMock = new TableClientMock();

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			actual.Should().BeEmpty();
		}

		[Fact]
		public async Task Should_return_empty_list_when_no_duplicates()
		{
			// Given
			var tableStorageMock = new TableClientMock
			{
				Entities = new CachedImage[] {
					new() { RowKey = "1", SimilarityHash = "a" },
					new() { RowKey = "2", SimilarityHash = "b" },
					new() { RowKey = "3", SimilarityHash = "c" },
				}
			};

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			actual.Should().BeEmpty();
		}

		[Fact]
		public async Task Should_return_duplicates_of_single_image()
		{
			// Given
			var tableStorageMock = new TableClientMock
			{
				Entities = new CachedImage[] {
					new() { RowKey = "original", SimilarityHash = "a" },
					new() { RowKey = "duplicate1", SimilarityHash = "a" },
					new() { RowKey = "duplicate2", SimilarityHash = "a" },
				}
			};

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			actual.Should().ContainInOrder("duplicate1", "duplicate2");
		}

		[Fact]
		public async Task Should_return_duplicates_of_multiple_images()
		{
			// Given
			var tableStorageMock = new TableClientMock
			{
				Entities = new CachedImage[] {
					new() { RowKey = "original1", SimilarityHash = "a" },
					new() { RowKey = "duplicate1", SimilarityHash = "a" },
					new() { RowKey = "duplicate2", SimilarityHash = "a" },
					new() { RowKey = "original2", SimilarityHash = "b" },
					new() { RowKey = "duplicate3", SimilarityHash = "b" },
			}
			};

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			actual.Should().ContainInOrder("duplicate1", "duplicate2", "duplicate3");
		}

		[Fact]
		public async Task Should_keep_first_image_when_duplicates_with_same_startdate()
		{
			// Given
			var cachedImages = new CachedImage[] {
				new() { RowKey = "date1_first", SimilarityHash = "a", StartDate = "20220502" },
				new() { RowKey = "date1_second", SimilarityHash = "a", StartDate = "20220502" },
				new() { RowKey = "date1_third", SimilarityHash = "a", StartDate = "20220502" },
			};
			var tableStorageMock = new TableClientMock
			{
				Entities = cachedImages
			};

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			var shouldKeep = new[] { "date1_first" };
			var expected = new[] { "date1_first", "date1_second", "date1_third" }.Except(shouldKeep);

			actual.Should().ContainInOrder(expected);
		}

		[Fact]
		public async Task Should_keep_older_image_when_duplicates_with_distinct_startdate()
		{
			// Given
			var tableStorageMock = new TableClientMock
			{
				Entities = new CachedImage[] {
					new() { RowKey = "duplicate_first", SimilarityHash = "a", StartDate = "20220502" },
					new() { RowKey = "duplicate_second", SimilarityHash = "a", StartDate = "20220501" },
					new() { RowKey = "duplicate_third", SimilarityHash = "a", StartDate = "20220503" },
				}
			};

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			var shouldKeep = new[] { "duplicate_second" };
			var expected = new[] { "duplicate_first", "duplicate_second", "duplicate_third" }.Except(shouldKeep);

			actual.Should().BeEquivalentTo(expected);
		}


		[Fact]
		public async Task Should_keep_older_image_when_duplicates_with_distinct_startdate_and_multiple_images()
		{
			// Given
			var tableStorageMock = new TableClientMock
			{
				Entities = new CachedImage[] {
					new() { RowKey = "date1_first", SimilarityHash = "a", StartDate = "20220502" },
					new() { RowKey = "date1_second", SimilarityHash = "a", StartDate = "20220502" },
					new() { RowKey = "date1_third", SimilarityHash = "a", StartDate = "20220502" },
					new() { RowKey = "date2_first", SimilarityHash = "a", StartDate = "20220501" },
					new() { RowKey = "date2_second", SimilarityHash = "a", StartDate = "20220501" },
				}
			};

			var service = new CleanCacheService(
				tableStorageMock,
				NullLogger<CleanCacheService>.Instance
			);

			// When
			var actual = await service.GetDuplicatedCacheEntries().ToListAsync();

			// Then
			var shouldKeep = new[] { "date2_first" };
			var expected = new[] { "date1_first", "date1_second", "date1_third", "date2_first", "date2_second" }.Except(shouldKeep);

			actual.Should().BeEquivalentTo(expected);
		}
	}
}
