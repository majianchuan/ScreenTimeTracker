import { apiClient } from "@/shared/api";
import {
  appUsageItemDtoSchema,
  type GetAppUsageParams,
  type AppUsageItemDto,
  appCategoryUsageItemDtoSchema,
  type GetAppCategoryUsageParams,
  type AppCategoryUsageItemDto,
} from "./schemas";

export const getAppUsage = async (
  params: GetAppUsageParams,
): Promise<AppUsageItemDto[]> => {
  const { data } = await apiClient.get("/screen-time/usage/apps/", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = appUsageItemDtoSchema.parse(dto);
    return {
      startTime: validated.startTime,
      durationSeconds: validated.durationSeconds,
    };
  });
};

export const getAppCategoryUsage = async (
  params: GetAppCategoryUsageParams,
): Promise<AppCategoryUsageItemDto[]> => {
  const { data } = await apiClient.get("/screen-time/usage/app-categories/", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = appCategoryUsageItemDtoSchema.parse(dto);
    return {
      startTime: validated.startTime,
      durationSeconds: validated.durationSeconds,
    };
  });
};
