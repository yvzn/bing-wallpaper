import { defineConfig } from "vite";
import reactRefresh from "@vitejs/plugin-react-refresh";
import dotEnvHTMLPlugin from "vite-plugin-dotenv-in-html";

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
	return {
		plugins: [
			reactRefresh(),
			dotEnvHTMLPlugin(mode),
		],
	};
});


