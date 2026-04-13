import { z } from "zod";

export const dimensionSchema = z.enum(["app", "app-category"]);
export type Dimension = z.infer<typeof dimensionSchema>;
