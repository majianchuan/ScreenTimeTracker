import { apiClient } from "@/shared/api";
import type {
  UsageByDayItem,
  UsageByHourItem,
  UsageRankingItem,
} from "../model/schemas";
import {
  usageByDayItemDtoSchema,
  usageByHourItemDtoSchema,
  usageRankingItemDtoSchema,
  type DeleteUsageDataParams,
  type GetUsageDetailsByDayParams,
  type GetUsageDetailsByHourParams,
  type GetUsageRankingsParams,
  type GetUsageSummaryByDayParams,
  type GetUsageSummaryByHourParams,
} from "./schemas";

export const getUsageDetailsByDay = async (
  params: GetUsageDetailsByDayParams,
): Promise<UsageByDayItem[]> => {
  const { data } = await apiClient.get("/screen-time/usage/daily", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = usageByDayItemDtoSchema.parse(dto);
    return {
      date: new Date(validated.date),
      durationSeconds: validated.durationSeconds,
    };
  });
};

export const getUsageDetailsByHour = async (
  params: GetUsageDetailsByHourParams,
): Promise<UsageByHourItem[]> => {
  const { data } = await apiClient.get("/screen-time/usage/hourly", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = usageByHourItemDtoSchema.parse(dto);
    return {
      hour: validated.hour,
      durationSeconds: validated.durationSeconds,
    };
  });
};

export const getUsageRankings = async (
  params: GetUsageRankingsParams,
): Promise<UsageRankingItem[]> => {
  const { data } = await apiClient.get("/screen-time/usage/rankings", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = usageRankingItemDtoSchema.parse(dto);
    return {
      id: validated.id,
      name: validated.name,
      iconPath: validated.iconPath,
      durationSeconds: validated.durationSeconds,
      percentage: validated.percentage,
    };
  });
};

export const getUsageSummaryByDay = async (
  params: GetUsageSummaryByDayParams,
): Promise<UsageByDayItem[]> => {
  const { data } = await apiClient.get("/screen-time/usage/summary/daily", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = usageByDayItemDtoSchema.parse(dto);
    return {
      date: new Date(validated.date),
      durationSeconds: validated.durationSeconds,
    };
  });
};

export const getUsageSummaryByHour = async (
  params: GetUsageSummaryByHourParams,
): Promise<UsageByHourItem[]> => {
  const { data } = await apiClient.get("/screen-time/usage/summary/hourly", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = usageByHourItemDtoSchema.parse(dto);
    return {
      hour: validated.hour,
      durationSeconds: validated.durationSeconds,
    };
  });
};

export const deleteUsageData = async (params: DeleteUsageDataParams) => {
  const { data } = await apiClient.delete(`/screen-time/usage-data`, {
    params,
  });
  return data;
};
