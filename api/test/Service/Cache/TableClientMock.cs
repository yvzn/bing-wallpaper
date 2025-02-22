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
using Azure.Data.Tables;
using Azure;
using System.Threading;
using System.Linq.Expressions;
using Ludeo.BingWallpaper.Model.Cache;
using NSubstitute;

namespace Ludeo.BingWallpaper.Tests.Service.Cache;

internal class TableClientMock : TableClient
{
	public TableClientMock()
	{
		Entities = [];
	}

	public IReadOnlyList<CachedImage> Entities { get; internal set; }

	public override AsyncPageable<T> QueryAsync<T>(
		Expression<Func<T, bool>> filter,
		int? maxPerPage = null,
		IEnumerable<string>? select = null,
		CancellationToken cancellationToken = default)
	{
		var values = Entities as IReadOnlyList<T>;
		var page = Page<T>.FromValues(values!, continuationToken: null, Substitute.For<Response>());
		return AsyncPageable<T>.FromPages([page]);
	}
}
