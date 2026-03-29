import { defineConfig } from "vite";
import path from "path";
import tailwindcss from "@tailwindcss/vite";
import react from "@vitejs/plugin-react";
import { tanstackRouter } from "@tanstack/router-plugin/vite";
import svgr from "vite-plugin-svgr";

// https://vite.dev/config/
export default defineConfig({
  build: {
    outDir: "../Hosts/Desktop/Resources/wwwroot",
    emptyOutDir: true,
  },
  plugins: [
    tanstackRouter({
      target: "react",
      autoCodeSplitting: true,
      routesDirectory: "./src/app/routes",
      generatedRouteTree: "./src/app/routeTree.gen.ts",
    }),
    react(),
    tailwindcss(),
    svgr(),
  ],

  // shadcn
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
});
