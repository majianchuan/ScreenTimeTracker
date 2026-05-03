import { queryOptions } from "@tanstack/react-query";
import { getAppUsageTimeline, getAppCategoryUsageTimeline } from "./requests";
import type {
  GetAppUsageTimelineParams,
  GetAppCategoryUsageTimelineParams,
} from "./schemas";

export const appUsageTimelineQueryOptions = (
  params: GetAppUsageTimelineParams,
) => {
  return queryOptions({
    queryKey: ["app-usage-timeline", params],
    queryFn: () => getAppUsageTimeline(params),
  });
};

export const appCategoryUsageTimelineQueryOptions = (
  params: GetAppCategoryUsageTimelineParams,
) => {
  return queryOptions({
    queryKey: ["app-category-usage-timeline", params],
    queryFn: () => getAppCategoryUsageTimeline(params),
  });
};
