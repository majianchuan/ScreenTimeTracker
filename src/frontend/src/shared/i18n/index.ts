import i18n from "./setup";

export const SUPPORTED_LANGUAGES = [
  { code: "en-US", label: "English" },
  { code: "zh-CN", label: "中文" },
] as const;

export type LanguageOption = (typeof SUPPORTED_LANGUAGES)[number];

export type LanguageCode = (typeof SUPPORTED_LANGUAGES)[number]["code"];

export default i18n;
