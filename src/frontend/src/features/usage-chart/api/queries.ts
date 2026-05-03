import { queryOptions } from "@tanstack/react-query";
import type { GetAppCategoryUsageParams, GetAppUsageParams } from "./schemas";
import { getAppCategoryUsage, getAppUsage } from "./requests";

export const appUsageQueryOptions = (params: GetAppUsageParams) => {
  return queryOptions({
    queryKey: ["app-usage", params],
    queryFn: () => getAppUsage(params),
  });
};

export const appCategoryUsageQueryOptions = (
  params: GetAppCategoryUsageParams,
) => {
  return queryOptions({
    queryKey: ["app-category-usage", params],
    queryFn: () => getAppCategoryUsage(params),
  });
};
