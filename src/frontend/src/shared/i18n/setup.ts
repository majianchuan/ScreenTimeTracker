import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import sharedEnUS from "./en-US.json";
import sharedZhCN from "./zh-CN.json";

i18n.use(initReactI18next).init({
  fallbackLng: "en-US",
  lng: "en-US",
  ns: ["shared"],
  defaultNS: "shared",
  resources: {
    "en-US": {
      shared: sharedEnUS,
    },
    "zh-CN": {
      shared: sharedZhCN,
    },
  },
  interpolation: {
    escapeValue: false,
  },
});

export default i18n;
