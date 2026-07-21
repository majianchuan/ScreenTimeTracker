import i18n from "@/shared/i18n";
import enUS from "./i18n/en-US.json";
import zhCN from "./i18n/zh-CN.json";

i18n.addResourceBundle("en-US", "feature_dimensionControl", enUS, true, true);
i18n.addResourceBundle("zh-CN", "feature_dimensionControl", zhCN, true, true);

export * from "./model/schemas";
export * from "./ui/DimensionMemberPicker";
export * from "./ui/DimensionTypeSelector";
