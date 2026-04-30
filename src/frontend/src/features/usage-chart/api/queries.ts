import { queryOptions } from "@tanstack/react-query";
import type { GetAppCategoryUsageParams, GetAppUsageParams } from "./schemas";
import { getAppCategoryUsage, getAppUsage } from "./requests";

export const appUsage = (params: GetAppUsageParams) => {
  return queryOptions({
    queryKey: ["app-usage", params],
    queryFn: () => getAppUsage(params),
  });
};

export const appCategoryUsage = (params: GetAppCategoryUsageParams) => {
  return queryOptions({
    queryKey: ["app-category-usage", params],
    queryFn: () => getAppCategoryUsage(params),
  });
};
