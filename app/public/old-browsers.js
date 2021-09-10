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

(function () {
	var imageRenderingSuccessful =
		document.getElementsByTagName("article").length > 1;
	if (imageRenderingSuccessful) {
		return;
	}

	var noScriptTag = document.getElementsByTagName("noscript")[0];
	var mainTag = document.getElementsByTagName("main")[0];

	mainTag.innerHTML = noScriptTag.innerText;

	var secondParagraphTag = mainTag.getElementsByTagName("p")[1];
	secondParagraphTag.innerText =
		"More images available in a newer browser, like Firefox or Chrome.";
})();
