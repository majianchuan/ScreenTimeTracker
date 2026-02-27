import { z } from "zod";

export const timeFrameSchema = z.enum(["day", "week", "month", "custom"]);
export type TimeFrame = z.infer<typeof timeFrameSchema>;

export const dateRangeSchema = z.object({
  start: z.date(),
  end: z.date(),
});
export type DateRange = z.infer<typeof dateRangeSchema>;
