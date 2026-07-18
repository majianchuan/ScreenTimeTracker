import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const appUsageDistributionItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  color: z.string(),
  iconPath: z.string().nullable(),
  iconLastUpdatedAt: z.coerce.date(),
  durationSeconds: z.number(),
});
export type AppUsageDistributionItemDto = z.infer<
  typeof appUsageDistributionItemDtoSchema
>;

export const appUsageDistributionDtoSchema = z.object({
  items: appUsageDistributionItemDtoSchema.array(),
  totalCount: z.number(),
  totalDurationSeconds: z.number(),
  othersCount: z.number(),
  othersDurationSeconds: z.number(),
});
export type AppUsageDistributionDto = z.infer<
  typeof appUsageDistributionDtoSchema
>;

export const appCategoryUsageDistributionItemDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  color: z.string(),
  iconPath: z.string().nullable(),
  iconLastUpdatedAt: z.coerce.date(),
  durationSeconds: z.number(),
});
export type AppCategoryUsageDistributionItemDto = z.infer<
  typeof appCategoryUsageDistributionItemDtoSchema
>;

export const appCategoryUsageDistributionDtoSchema = z.object({
  items: appCategoryUsageDistributionItemDtoSchema.array(),
  totalCount: z.number(),
  totalDurationSeconds: z.number(),
  othersCount: z.number(),
  othersDurationSeconds: z.number(),
});
export type AppCategoryUsageDistributionDto = z.infer<
  typeof appCategoryUsageDistributionDtoSchema
>;

// api 参数类型
export const getAppUsageDistributionParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  topN: z.number().optional(),
  excludedIds: z.array(z.string()).optional(),
});
export type GetAppUsageDistributionParams = z.infer<
  typeof getAppUsageDistributionParamsSchema
>;

export const getAppCategoryUsageDistributionParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
  topN: z.number().optional(),
  excludedIds: z.array(z.string()).optional(),
});
export type GetAppCategoryUsageDistributionParams = z.infer<
  typeof getAppCategoryUsageDistributionParamsSchema
>;
