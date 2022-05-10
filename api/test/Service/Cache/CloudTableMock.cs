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
using Ludeo.BingWallpaper.Model.Cache;
using Microsoft.Azure.Cosmos.Table;

namespace Ludeo.BingWallpaper.Tests.Service.Cache;

internal class CloudTableMock : CloudTable
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
		// TableQuerySegment<T> has no public constructor :(
		var ctor = typeof(TableQuerySegment<TElement>)
			.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
			.FirstOrDefault(c => c.GetParameters().Count() == 1);

		var mockTableQuerySegment = ctor?.Invoke(new object[] { Entities.ToList() }) as TableQuerySegment<TElement>;
		return Task.FromResult(mockTableQuerySegment);
	}
}
