import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const appUsageTimelineItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  startTime: z.coerce.date(),
  endTime: z.coerce.date(),
});
export type AppUsageTimelineItemDto = z.infer<
  typeof appUsageTimelineItemDtoSchema
>;

export const appCategoryUsageTimelineItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  startTime: z.coerce.date(),
  endTime: z.coerce.date(),
});
export type AppCategoryUsageTimelineItemDto = z.infer<
  typeof appCategoryUsageTimelineItemDtoSchema
>;

// api 参数类型
export const getAppUsageTimelineParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  excludedIds: z.array(z.string()).optional(),
});
export type GetAppUsageTimelineParams = z.infer<
  typeof getAppUsageTimelineParamsSchema
>;

export const getAppCategoryUsageTimelineParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  excludedIds: z.array(z.string()).optional(),
});
export type GetAppCategoryUsageTimelineParams = z.infer<
  typeof getAppCategoryUsageTimelineParamsSchema
>;
