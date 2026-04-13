import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import type {
  DeleteUsageDataParams,
  GetUsageDetailsByDayParams,
  GetUsageDetailsByHourParams,
  GetUsageRankingsParams,
  GetUsageSummaryByDayParams,
  GetUsageSummaryByHourParams,
} from "./schemas";
import {
  deleteUsageData,
  getUsageDetailsByDay,
  getUsageDetailsByHour,
  getUsageRankings,
  getUsageSummaryByDay,
  getUsageSummaryByHour,
} from "./requests";

export const usageKeys = {
  all: ["usage"] as const,
  summaries: () => [...usageKeys.all, "summary"] as const,
  summaryByDay: (params: object) =>
    [...usageKeys.summaries(), "daily", params] as const,
  summaryByHour: (params: object) =>
    [...usageKeys.summaries(), "hourly", params] as const,

  rankings: (params: object) => [...usageKeys.all, "rankings", params] as const,

  details: () => [...usageKeys.all, "details"] as const,
  detailByDay: (params: object) =>
    [...usageKeys.details(), "daily", params] as const,
  detailByHour: (params: object) =>
    [...usageKeys.details(), "hourly", params] as const,
};

export const usageQueries = {
  detailsByDay: (params: GetUsageDetailsByDayParams) => {
    return queryOptions({
      queryKey: usageKeys.detailByDay(params),
      queryFn: () => getUsageDetailsByDay(params),
    });
  },

  detailsByHour: (params: GetUsageDetailsByHourParams) => {
    return queryOptions({
      queryKey: usageKeys.detailByHour(params),
      queryFn: () => getUsageDetailsByHour(params),
    });
  },

  rankings: (params: GetUsageRankingsParams) => {
    return queryOptions({
      queryKey: usageKeys.rankings(params),
      queryFn: () => getUsageRankings(params),
    });
  },

  summaryByDay: (params: GetUsageSummaryByDayParams) => {
    return queryOptions({
      queryKey: usageKeys.summaryByDay(params),
      queryFn: () => getUsageSummaryByDay(params),
    });
  },

  summaryByHour: (params: GetUsageSummaryByHourParams) => {
    return queryOptions({
      queryKey: usageKeys.summaryByHour(params),
      queryFn: () => getUsageSummaryByHour(params),
    });
  },
};

export const useDeleteData = () => {
  const queryClient = useQueryClient();
  return useMutation<void, Error, DeleteUsageDataParams>({
    mutationFn: deleteUsageData,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: usageKeys.all });
    },
  });
};
