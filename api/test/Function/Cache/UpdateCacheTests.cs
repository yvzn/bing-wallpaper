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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using FluentAssertions;
using Ludeo.BingWallpaper.Function.Cache;
using Ludeo.BingWallpaper.Model.Cache;
using Ludeo.BingWallpaper.Service.Bing;
using Ludeo.BingWallpaper.Service.Cache;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Xunit;

namespace Ludeo.BingWallpaper.Tests.Function.Cache;

public class UpdateCacheTests
{
	[Fact]
	public async Task Refresh_inserts_images_then_removes_duplicates_and_old_entries()
	{
		var table = new CacheTable();
		for (var i = 0; i < 51; i++)
		{
			table.Entities.Add(new CachedImage
			{
				PartitionKey = "cache",
				RowKey = $"9{i:D3}",
				SimilarityHash = $"old-{i}",
				StartDate = "20200101"
			});
		}

		var factory = Substitute.For<IAzureClientFactory<TableClient>>();
		factory.CreateClient("ImageCacheTableClient").Returns(table);
		var httpFactory = Substitute.For<IHttpClientFactory>();
		httpFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient(new BingHandler()));
		var function = new UpdateCache(
			new WallpaperService(httpFactory, NullLogger<WallpaperService>.Instance),
			new HashingService(httpFactory, NullLogger<HashingService>.Instance),
			new UpdateCacheService(factory, NullLogger<UpdateCacheService>.Instance),
			new CleanCacheService(factory, NullLogger<CleanCacheService>.Instance));

		var result = await function.RunAsync(new DefaultHttpContext().Request);

		result.Should().BeOfType<NoContentResult>();
		table.Events.Take(5).Should().OnlyContain(entry => entry == "add");
		table.Events.Skip(5).First().Should().Be("query");
		table.Entities.Should().HaveCount(50);
		table.Entities.Count(image => image.StartDate == "20260101").Should().Be(1);
		table.Entities.Should().NotContain(image => image.RowKey == "9049" || image.RowKey == "9050");
	}

	private sealed class BingHandler : HttpMessageHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (request.RequestUri!.AbsolutePath.EndsWith("HPImageArchive.aspx", StringComparison.Ordinal))
			{
				return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
				{
					Content = new StringContent("""{"images":[{"urlbase":"/image","startdate":"20260101"}]}""")
				});
			}

			using var image = new Image<Rgba32>(32, 32);
			using var stream = new MemoryStream();
			image.SaveAsPng(stream);
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
			{
				Content = new ByteArrayContent(stream.ToArray())
			});
		}
	}

	private sealed class CacheTable : TableClient
	{
		public List<CachedImage> Entities { get; } = [];
		public List<string> Events { get; } = [];

		public override Task<Response> AddEntityAsync<T>(T entity, CancellationToken cancellationToken = default)
		{
			Events.Add("add");
			Entities.Add((CachedImage)(object)entity);
			return Task.FromResult(Substitute.For<Response>());
		}

		public override AsyncPageable<T> QueryAsync<T>(
			Expression<Func<T, bool>> filter,
			int? maxPerPage = null,
			IEnumerable<string>? select = null,
			CancellationToken cancellationToken = default)
		{
			Events.Add("query");
			var values = Entities.OrderBy(image => image.RowKey).Cast<T>().ToList();
			return AsyncPageable<T>.FromPages([Page<T>.FromValues(values, null, Substitute.For<Response>())]);
		}

		public override Task<Response> DeleteEntityAsync(
			string partitionKey, string rowKey, ETag ifMatch = default, CancellationToken cancellationToken = default)
		{
			Events.Add("delete");
			Entities.RemoveAll(image => image.PartitionKey == partitionKey && image.RowKey == rowKey);
			return Task.FromResult(Substitute.For<Response>());
		}
	}
}
