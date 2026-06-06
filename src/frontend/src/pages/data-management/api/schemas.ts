import z from "zod";
import { dateOnlySchema } from "@/shared/lib/date-only";

export const deleteUsageDataParamsSchema = z.object({
  startDate: dateOnlySchema,
  endDate: dateOnlySchema,
});
export type DeleteUsageDataParams = z.infer<typeof deleteUsageDataParamsSchema>;

export const importDataDtoSchema = z.object({
  newAppCategories: z.number(),
  newApps: z.number(),
  importedSessions: z.number(),
  skippedSessions: z.number(),
});
export type ImportDataDto = z.infer<typeof importDataDtoSchema>;
