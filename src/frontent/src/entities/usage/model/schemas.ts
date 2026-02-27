import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const usageByDayItemSchema = z.object({
  date: dateOnlySchema,
  durationSeconds: z.number(),
});
export type UsageByDayItem = z.infer<typeof usageByDayItemSchema>;

export const usageByHourItemSchema = z.object({
  hour: z.number().min(0).max(23),
  durationSeconds: z.number(),
});
export type UsageByHourItem = z.infer<typeof usageByHourItemSchema>;

export const usageRankingItemSchema = z.object({
  id: z.string(),
  name: z.string(),
  iconPath: z.string().nullable(),
  durationSeconds: z.number(),
  percentage: z.number(),
});
export type UsageRankingItem = z.infer<typeof usageRankingItemSchema>;
