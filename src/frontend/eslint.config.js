import js from "@eslint/js";
import globals from "globals";
import reactHooks from "eslint-plugin-react-hooks";
import reactRefresh from "eslint-plugin-react-refresh";
import tseslint from "typescript-eslint";
import { defineConfig, globalIgnores } from "eslint/config";

export default defineConfig([
  globalIgnores(["dist", "**/routeTree.gen.ts"]),
  {
    files: ["**/*.{ts,tsx}"],
    extends: [
      js.configs.recommended,
      tseslint.configs.recommended,
      reactHooks.configs.flat.recommended,
      reactRefresh.configs.vite,
    ],
    languageOptions: {
      globals: globals.browser,
    },
    rules: {
      "@typescript-eslint/no-unused-vars": [
        "error",
        {
          // 允许变量名以 _ 开头但未使用
          varsIgnorePattern: "^_",
          // 允许参数名以 _ 开头但未使用
          argsIgnorePattern: "^_",
          // 允许解构中的 _ 被忽略
          caughtErrorsIgnorePattern: "^_",
        },
      ],
      "no-restricted-imports": [
        "error",
        {
          patterns: [
            {
              regex: "^@mui/(?!x-)[^/]+$",
            },
          ],
        },
      ],
    },
  },
]);
