import { apiClient } from "@/shared/api";
import {
  type GetAppUsageDistributionParams,
  type GetAppCategoryUsageDistributionParams,
  appUsageDistributionDtoSchema,
  type AppUsageDistributionDto,
  appCategoryUsageDistributionDtoSchema,
  type AppCategoryUsageDistributionDto,
} from "./schemas";

export const getAppUsageDistribution = async (
  params: GetAppUsageDistributionParams,
): Promise<AppUsageDistributionDto> => {
  const { data } = await apiClient.get("/screen-time/usage/apps/distribution", {
    params,
  });
  return appUsageDistributionDtoSchema.parse(data);
};

export const getAppCategoryUsageDistribution = async (
  params: GetAppCategoryUsageDistributionParams,
): Promise<AppCategoryUsageDistributionDto> => {
  const { data } = await apiClient.get(
    "/screen-time/usage/app-categories/distribution",
    {
      params,
    },
  );
  return appCategoryUsageDistributionDtoSchema.parse(data);
};
