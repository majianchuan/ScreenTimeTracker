import { z } from "zod";

export const dateOnlySchema = z.iso.date();
export type DateOnly = z.infer<typeof dateOnlySchema>; // YYYY-MM-DD
