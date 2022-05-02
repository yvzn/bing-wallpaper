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
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.Azure.Cosmos.Table;
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
			var tableStorageMock = new CloudTableMock();

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
			var tableStorageMock = new CloudTableMock
			{
				Entities = new [] {
					new CachedImage { RowKey = "1", SimilarityHash = "a" },
					new CachedImage { RowKey = "2", SimilarityHash = "b" },
					new CachedImage { RowKey = "3", SimilarityHash = "c" },
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
		public async Task Should_return_list_of_duplicates_of_single_image()
		{
			// Given
			var tableStorageMock = new CloudTableMock
			{
				Entities = new [] {
					new CachedImage { RowKey = "original", SimilarityHash = "a" },
					new CachedImage { RowKey = "duplicate1", SimilarityHash = "a" },
					new CachedImage { RowKey = "duplicate2", SimilarityHash = "a" },
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
		public async Task Should_return_list_of_duplicates_of_multiple_images()
		{
			// Given
			var tableStorageMock = new CloudTableMock
			{
				Entities = new [] {
					new CachedImage { RowKey = "original1", SimilarityHash = "a" },
					new CachedImage { RowKey = "duplicate1", SimilarityHash = "a" },
					new CachedImage { RowKey = "duplicate2", SimilarityHash = "a" },
					new CachedImage { RowKey = "original2", SimilarityHash = "b" },
					new CachedImage { RowKey = "duplicate3", SimilarityHash = "b" },
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
		}	}

	public class CloudTableMock : CloudTable
	{
		public CloudTableMock() : base(new("http://127.0.0.1:10002/devstoreaccount1/pizzas*"))
		{
			Entities = Array.Empty<CachedImage>();
		}

		public ICollection<CachedImage> Entities { get; internal set; }

		public override Task<TableQuerySegment<TElement>?> ExecuteQuerySegmentedAsync<TElement>(
			TableQuery<TElement> query,
			TableContinuationToken token)
		{
			var ctor = typeof(TableQuerySegment<TElement>)
				.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
				.FirstOrDefault(c => c.GetParameters().Count() == 1);

			var mockTableQuerySegment = ctor?.Invoke(new object[] { Entities.ToList() }) as TableQuerySegment<TElement>;
			return Task.FromResult(mockTableQuerySegment);
		}
	}
}
