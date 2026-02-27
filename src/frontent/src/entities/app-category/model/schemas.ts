import { z } from "zod";

export const appCategorySchema = z.object({
  id: z.string(),
  name: z.string(),
  iconPath: z.string().nullable(),
  isSystem: z.boolean(),
});
export type AppCategory = z.infer<typeof appCategorySchema>;
