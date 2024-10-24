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

(function showLegacyBrowserContent() {
	var noScriptTag = document.getElementsByTagName("noscript")[0];
	var contentForNoScriptBrowsers = noScriptTag.innerText;

	var mainContentTag = document.getElementsByTagName("main")[0];
	mainContentTag.innerHTML = contentForNoScriptBrowsers;

	var explanatoryTextTag = mainContentTag.getElementsByTagName("strong")[0];
	explanatoryTextTag.innerText =
		"Updating to a newer browser, like Firefox or Chrome, will display more wallpapers.";

})();
