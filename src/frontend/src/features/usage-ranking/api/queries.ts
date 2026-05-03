import { queryOptions } from "@tanstack/react-query";
import type {
  GetAppCategoryUsageRankingParams,
  GetAppUsageRankingParams,
} from "./schemas";
import { getAppCategoryUsageRanking, getAppUsageRanking } from "./requests";

export const appUsageRankingQueryOptions = (
  params: GetAppUsageRankingParams,
) => {
  return queryOptions({
    queryKey: ["app-usage-ranking", params],
    queryFn: () => getAppUsageRanking(params),
  });
};

export const appCategoryUsageRankingQueryOptions = (
  params: GetAppCategoryUsageRankingParams,
) => {
  return queryOptions({
    queryKey: ["app-category-usage-ranking", params],
    queryFn: () => getAppCategoryUsageRanking(params),
  });
};
