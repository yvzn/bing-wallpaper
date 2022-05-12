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

const { loadEnv } = require("vite");

/**
 * The simpliest plugin ever to replace import.meta.env variables in index.html file.
 *
 * @param {string} mode production, development, etc.
 * @returns a vite plugin definition
 */
export function importMetaEnvHTMLPlugin(mode) {
	return {
		name: "vite-plugin-import-meta-env-html",
		transformIndexHtml(html) {
			const env = loadEnv(mode, process.cwd());
			return html.replace(
				/import\.meta\.env\.(VITE_[A-Z0-9_]+)/g,
				(_, key) =>
					key in env ? env[key] : key
			);
		},
	};
}

// Kudos to: https://github.com/lxs24sxl/vite-plugin-html-env
// Kudos to: https://github.com/vbenjs/vite-plugin-html
