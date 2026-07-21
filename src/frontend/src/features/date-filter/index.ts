import i18n from "@/shared/i18n";
import enUS from "./i18n/en-US.json";
import zhCN from "./i18n/zh-CN.json";

i18n.addResourceBundle("en-US", "feature_dateFilter", enUS, true, true);
i18n.addResourceBundle("zh-CN", "feature_dateFilter", zhCN, true, true);

export * from "./model/schemas";
export * from "./model/useDateFilter";
export * from "./ui/TimeFrameSelector";
export * from "./ui/DateRangeSelector";
