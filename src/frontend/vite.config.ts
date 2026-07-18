import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import { tanstackRouter } from "@tanstack/router-plugin/vite";
import { fileURLToPath } from "node:url";
import svgr from "vite-plugin-svgr";

// https://vite.dev/config/
export default defineConfig({
  server: {
    host: "0.0.0.0",
    watch: {
      usePolling: true,
    },
  },
  plugins: [
    tanstackRouter({
      target: "react",
      autoCodeSplitting: true,
      routesDirectory: "./src/app/routes",
      generatedRouteTree: "./src/app/routeTree.gen.ts",
    }),
    react(),
    svgr(),
  ],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
});
