import { timeFrameSchema } from "@/features/date-filter";
import { dimensionSchema } from "@/features/dimension-control";
import { dateOnlySchema } from "@/shared/lib/date-only";
import { z } from "zod";

export const searchParamsSchema = z.object({
  timeFrame: timeFrameSchema.default("day"),
  startDate: dateOnlySchema.optional(),
  endDate: dateOnlySchema.optional(),
  dimension: dimensionSchema.default("app"),
  id: z.string().optional(),
});

export type SearchParams = z.infer<typeof searchParamsSchema>;
