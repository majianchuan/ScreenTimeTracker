import i18n from "@/shared/i18n";
import enUS from "./i18n/en-US.json";
import zhCN from "./i18n/zh-CN.json";

i18n.addResourceBundle("en-US", "entity_appCategory", enUS, true, true);
i18n.addResourceBundle("zh-CN", "entity_appCategory", zhCN, true, true);

export * from "./model/schemas";

export * from "./api/schemas";
export * from "./api/queries";
export * from "./api/requests";

export * from "./ui/AppCategoryIcon";
export * from "./ui/AppCategoryPicker";
export * from "./ui/AppCategorySelecter";
