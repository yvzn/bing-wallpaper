import { defineConfig, loadEnv } from "vite";
import reactRefresh from "@vitejs/plugin-react-refresh";
import { createHtmlPlugin } from "vite-plugin-html";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
	const env = loadEnv(mode, process.cwd());
	return {
		plugins: [
			reactRefresh(),
			createHtmlPlugin({
				minify: true,
				inject: {
					data: {
						apiRedirectionLatestUrl:
							env.VITE_API_REDIRECTION_LATEST_URL,
					},
				},
			}),
		],
	};
});
