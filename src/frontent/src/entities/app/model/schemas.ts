import { z } from "zod";

export const appSchema = z.object({
  id: z.string(),
  name: z.string(),
  processName: z.string(),
  isAutoUpdateEnabled: z.boolean(),
  lastAutoUpdated: z.date(),
  appCategoryId: z.string(),
  executablePath: z.string().nullable(),
  iconPath: z.string().nullable(),
  description: z.string().nullable(),
});
export type App = z.infer<typeof appSchema>;
