import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const usageDimensionSchema = z.enum(["app", "app-category"]);
export type UsageDimension = z.infer<typeof usageDimensionSchema>;

export const usageByDayItemDtoSchema = z.object({
  date: dateOnlySchema,
  durationSeconds: z.number(),
});
export type UsageByDayItemDto = z.infer<typeof usageByDayItemDtoSchema>;

export const usageByHourItemDtoSchema = z.object({
  hour: z.number().min(0).max(23),
  durationSeconds: z.number(),
});
export type UsageByHourItemDto = z.infer<typeof usageByHourItemDtoSchema>;

export const usageRankingItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  iconPath: z.string().nullable(),
  durationSeconds: z.number(),
  percentage: z.number(),
});
export type UsageRankingItemDto = z.infer<typeof usageRankingItemDtoSchema>;

// api 参数类型
export const getUsageDetailsByDayParamsSchema = z.object({
  dimension: usageDimensionSchema,
  id: z.string(),
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
});
export type GetUsageDetailsByDayParams = z.infer<
  typeof getUsageDetailsByDayParamsSchema
>;

export const getUsageDetailsByHourParamsSchema = z.object({
  dimension: usageDimensionSchema,
  id: z.string(),
  date: dateOnlySchema,
});
export type GetUsageDetailsByHourParams = z.infer<
  typeof getUsageDetailsByHourParamsSchema
>;

export const getUsageRankingsParamsSchema = z.object({
  dimension: usageDimensionSchema,
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  topN: z.number().optional(),
  excludedIds: z.array(z.string()).optional(),
});
export type GetUsageRankingsParams = z.infer<
  typeof getUsageRankingsParamsSchema
>;

export const getUsageSummaryByDayParamsSchema = z.object({
  dimension: usageDimensionSchema,
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  excludedIds: z.array(z.string()).optional(),
});
export type GetUsageSummaryByDayParams = z.infer<
  typeof getUsageSummaryByDayParamsSchema
>;

export const getUsageSummaryByHourParamsSchema = z.object({
  dimension: usageDimensionSchema,
  date: dateOnlySchema,
  excludedIds: z.array(z.string()).optional(),
});
export type GetUsageSummaryByHourParams = z.infer<
  typeof getUsageSummaryByHourParamsSchema
>;

export const deleteUsageDataParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
});
export type DeleteUsageDataParams = z.infer<typeof deleteUsageDataParamsSchema>;
