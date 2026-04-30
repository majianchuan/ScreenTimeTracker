import { apiClient } from "@/shared/api";
import {
  type GetAppUsageTimelineParams,
  type AppUsageTimelineItemDto,
  appUsageTimelineItemDtoSchema,
  type GetAppCategoryUsageTimelineParams,
  type AppCategoryUsageTimelineItemDto,
  appCategoryUsageTimelineItemDtoSchema,
} from "./schemas";

export const getAppUsageTimeline = async (
  params: GetAppUsageTimelineParams,
): Promise<AppUsageTimelineItemDto[]> => {
  const { data } = await apiClient.get("/screen-time/usage/apps/Timeline", {
    params,
  });
  return data.map((dto: unknown) => {
    const validated = appUsageTimelineItemDtoSchema.parse(dto);
    return {
      id: validated.id,
      name: validated.name,
      startTime: validated.startTime,
      endTime: validated.endTime,
    };
  });
};

export const getAppCategoryUsageTimeline = async (
  params: GetAppCategoryUsageTimelineParams,
): Promise<AppCategoryUsageTimelineItemDto[]> => {
  const { data } = await apiClient.get(
    "/screen-time/usage/app-categories/Timeline",
    {
      params,
    },
  );
  return data.map((dto: unknown) => {
    const validated = appCategoryUsageTimelineItemDtoSchema.parse(dto);
    return {
      id: validated.id,
      name: validated.name,
      startTime: validated.startTime,
      endTime: validated.endTime,
    };
  });
};
