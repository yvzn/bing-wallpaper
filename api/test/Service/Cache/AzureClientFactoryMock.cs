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

using Azure.Data.Tables;
using Microsoft.Extensions.Azure;
using NSubstitute;

namespace Ludeo.BingWallpaper.Tests.Service.Cache;

internal class AzureClientFactoryMock
{
	internal static IAzureClientFactory<TableClient> Create(TableClientMock tableStorageMock)
	{
		var mock = Substitute.For<IAzureClientFactory<TableClient>>();
		mock.CreateClient(default).ReturnsForAnyArgs(tableStorageMock);
		return mock;
	}
}
