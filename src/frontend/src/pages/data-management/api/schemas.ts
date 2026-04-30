import z from "zod";
import { dateOnlySchema } from "@/shared/lib/date-only";

export const deleteUsageDataParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
});
export type DeleteUsageDataParams = z.infer<typeof deleteUsageDataParamsSchema>;
