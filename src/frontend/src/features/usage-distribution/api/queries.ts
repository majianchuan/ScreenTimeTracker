import { queryOptions } from "@tanstack/react-query";
import type {
  GetAppCategoryUsageDistributionParams,
  GetAppUsageDistributionParams,
} from "./schemas";
import {
  getAppCategoryUsageDistribution,
  getAppUsageDistribution,
} from "./requests";

export const appUsageDistributionQueryOptions = (
  params: GetAppUsageDistributionParams,
) => {
  return queryOptions({
    queryKey: ["app-usage-distribution", params],
    queryFn: () => getAppUsageDistribution(params),
  });
};

export const appCategoryUsageDistributionQueryOptions = (
  params: GetAppCategoryUsageDistributionParams,
) => {
  return queryOptions({
    queryKey: ["app-category-usage-distribution", params],
    queryFn: () => getAppCategoryUsageDistribution(params),
  });
};
