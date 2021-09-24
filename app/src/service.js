/*
   Copyright 2021 Yvan Razafindramanana

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

export const numberOfImages = 10;
const defaultResolutions = {
	low: { width: 320, height: 180 },
	full: { width: 1920, height: 1080 },
};

async function getWallpapers() {
	const lastWallpapersUrl = `${
		import.meta.env.VITE_API_URL
	}/api/last/${numberOfImages}`;

	const response = await fetch(lastWallpapersUrl);
	const json = await response.json();

	return json.map((image) => ({
		title: image.title,
		copyright: image.copyright,
		thumbnail: {
			url: `${image.uri}_${defaultResolutions.low.width}x${defaultResolutions.low.height}.jpg`,
		},
		large: {
			url: `${image.uri}_${defaultResolutions.full.width}x${defaultResolutions.full.height}.jpg`,
		},
	}));
}

export default getWallpapers;
