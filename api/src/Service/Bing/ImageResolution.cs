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

namespace Ludeo.BingWallpaper.Service.Bing
{
	internal static class ImageResolution
	{
		public const string Low = "320x180";
		public const string Full = "1920x1080";
		public const string UltraHigh = "UHD";

		public static Uri ToLowResolution(this string? uri) => new Uri($"{uri}_{Low}.jpg");
		public static Uri ToFullResolution(this string? uri) => new Uri($"{uri}_{Full}.jpg");
		public static Uri ToUltraHighResolution(this string? uri) => new Uri($"{uri}_{UltraHigh}.jpg");
	}
}
