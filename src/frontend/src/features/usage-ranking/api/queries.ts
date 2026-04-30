import { queryOptions } from "@tanstack/react-query";
import type {
  GetAppCategoryUsageRankingParams,
  GetAppUsageRankingParams,
} from "./schemas";
import { getAppCategoryUsageRanking, getAppUsageRanking } from "./requests";

export const appUsageRanking = (params: GetAppUsageRankingParams) => {
  return queryOptions({
    queryKey: ["app-usage-ranking", params],
    queryFn: () => getAppUsageRanking(params),
  });
};

export const appCategoryUsageRanking = (
  params: GetAppCategoryUsageRankingParams,
) => {
  return queryOptions({
    queryKey: ["app-category-usage-ranking", params],
    queryFn: () => getAppCategoryUsageRanking(params),
  });
};
