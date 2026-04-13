import { z } from "zod";
import { appSchema } from "../model/schemas";

export const appDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  processName: z.string(),
  isAutoUpdateEnabled: z.boolean(),
  lastAutoUpdated: z.coerce.date(),
  appCategoryId: z.string(),
  executablePath: z.string().nullable(),
  iconPath: z.string().nullable(),
  description: z.string().nullable(),
});
export type AppDto = z.infer<typeof appDtoSchema>;

// api 参数类型
export const getAppsParamsSchema = z.object({
  fields: z.string().optional(),
});
export type GetAppsParams = z.infer<typeof getAppsParamsSchema>;

export const patchAppSchema = z.object({
  id: z.string(),
  body: appSchema
    .pick({
      name: true,
      isAutoUpdateEnabled: true,
      appCategoryId: true,
      iconPath: true,
    })
    .partial(),
});
export type PatchAppParams = z.infer<typeof patchAppSchema>;
