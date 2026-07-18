import { z } from "zod";

export const appSettingsDtoSchema = z.object({
  defaultUIOpenMode: z.enum(["Window", "Browser"]),
  isAutoStartEnabled: z.boolean(),
  isSilentStartEnabled: z.boolean(),
  language: z.string(),
});
export type AppSettingsDto = z.infer<typeof appSettingsDtoSchema>;

// api 参数类型
export const patchAppSettingsParamsSchema = appSettingsDtoSchema.partial();
export type PatchAppSettingsParams = z.infer<
  typeof patchAppSettingsParamsSchema
>;
