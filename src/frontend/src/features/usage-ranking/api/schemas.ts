import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const appUsageRankingItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  iconPath: z.string().nullable(),
  durationSeconds: z.number(),
  percentage: z.number(),
});
export type AppUsageRankingItemDto = z.infer<
  typeof appUsageRankingItemDtoSchema
>;

export const appCategoryUsageRankingItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  iconPath: z.string().nullable(),
  durationSeconds: z.number(),
  percentage: z.number(),
});
export type AppCategoryUsageRankingItemDto = z.infer<
  typeof appCategoryUsageRankingItemDtoSchema
>;

// api 参数类型
export const getAppUsageRankingParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  topN: z.number().optional(),
  excludedIds: z.array(z.string()).optional(),
});
export type GetAppUsageRankingParams = z.infer<
  typeof getAppUsageRankingParamsSchema
>;

export const getAppCategoryUsageRankingParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  topN: z.number().optional(),
  excludedIds: z.array(z.string()).optional(),
});
export type GetAppCategoryUsageRankingParams = z.infer<
  typeof getAppCategoryUsageRankingParamsSchema
>;
