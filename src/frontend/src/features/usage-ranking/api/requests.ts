import { apiClient } from "@/shared/api";
import {
  appUsageRankingItemDtoSchema,
  type GetAppUsageRankingParams,
  type AppUsageRankingItemDto,
  appCategoryUsageRankingItemDtoSchema,
  type GetAppCategoryUsageRankingParams,
  type AppCategoryUsageRankingItemDto,
} from "./schemas";

export const getAppUsageRanking = async (
  params: GetAppUsageRankingParams,
): Promise<AppUsageRankingItemDto[]> => {
  const { data } = await apiClient.get("/screen-time/usage/apps/ranking", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = appUsageRankingItemDtoSchema.parse(dto);
    return {
      id: validated.id,
      name: validated.name,
      iconPath: validated.iconPath,
      durationSeconds: validated.durationSeconds,
      percentage: validated.percentage,
    };
  });
};

export const getAppCategoryUsageRanking = async (
  params: GetAppCategoryUsageRankingParams,
): Promise<AppCategoryUsageRankingItemDto[]> => {
  const { data } = await apiClient.get(
    "/screen-time/usage/app-categories/ranking",
    {
      params,
    },
  );
  return data.map((dto: unknown) => {
    const validated = appCategoryUsageRankingItemDtoSchema.parse(dto);
    return {
      id: validated.id,
      name: validated.name,
      iconPath: validated.iconPath,
      durationSeconds: validated.durationSeconds,
      percentage: validated.percentage,
    };
  });
};
