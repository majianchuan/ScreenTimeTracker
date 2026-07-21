import i18n from "@/shared/i18n";
import enUS from "./i18n/en-US.json";
import zhCN from "./i18n/zh-CN.json";

i18n.addResourceBundle("en-US", "page_appCategoryManagement", enUS, true, true);
i18n.addResourceBundle("zh-CN", "page_appCategoryManagement", zhCN, true, true);

export * from "./ui/AppCategoryManagementPage";
