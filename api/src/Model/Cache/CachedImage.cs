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

using Microsoft.Azure.Cosmos.Table;

namespace Ludeo.BingWallpaper.Model.Cache
{
	public class CachedImage : TableEntity
	{
		internal static string DefaultPartitionKey = "cache";
		internal static int NumberOfEntriesToKeep = 50;

		public string? Uri { get; set; }
		public string? Title { get; set; }
		public string? Copyright { get; set; }
		public string? CopyrightLink { get; internal set; }
		public string? Market { get; set; }
		public string? SimilarityHash { get; set; }
	}
}
