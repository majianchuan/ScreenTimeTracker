import { timeFrameSchema } from "@/features/date-filter";
import { dimensionSchema } from "@/features/dimension-control";
import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const searchParamsSchema = z.object({
  timeFrame: timeFrameSchema.default("day"),
  startDate: dateOnlySchema.optional(),
  endDate: dateOnlySchema.optional(),
  dimension: dimensionSchema.default("app"),
  excludedIds: z.array(z.string()).optional(),
  topN: z.number().int().default(10),
});

export type SearchParams = z.infer<typeof searchParamsSchema>;
