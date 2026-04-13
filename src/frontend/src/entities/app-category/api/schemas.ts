import { z } from "zod";
import { appCategorySchema } from "../model/schemas";

export const appCategoryDtoSchema = z.object({
  id: z.string(),
  name: z.string(),
  iconPath: z.string().nullable(),
  isSystem: z.boolean(),
});
export type AppCategoryDto = z.infer<typeof appCategoryDtoSchema>;

// api 参数类型
export const getAppCategoriesParamsSchema = z.object({
  fields: z.string().optional(),
});
export type GetAppCategoriesParams = z.infer<
  typeof getAppCategoriesParamsSchema
>;

export const createAppCategorySchema = appCategorySchema.pick({
  name: true,
  iconPath: true,
});
export type CreateAppCategoryParams = z.infer<typeof createAppCategorySchema>;

export const patchAppCategorySchema = z.object({
  id: z.string(),
  body: appCategorySchema.pick({ name: true, iconPath: true }).partial(),
});
export type PatchAppCategoryParams = z.infer<typeof patchAppCategorySchema>;
