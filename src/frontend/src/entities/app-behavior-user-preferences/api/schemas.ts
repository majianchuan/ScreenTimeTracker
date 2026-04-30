import { z } from "zod";

export const appBehaviorUserPreferencesDtoSchema = z.object({
  defaultUIOpenMode: z.enum(["Window", "Browser"]),
  isAutoStartEnabled: z.boolean(),
  isSilentStartEnabled: z.boolean(),
  language: z.string(),
  shouldDestroyWindowOnClose: z.boolean(),
});
export type AppBehaviorUserPreferencesDto = z.infer<
  typeof appBehaviorUserPreferencesDtoSchema
>;

// api 参数类型
export const patchAppBehaviorUserPreferencesParamsSchema =
  appBehaviorUserPreferencesDtoSchema.partial();
export type PatchAppBehaviorUserPreferencesParams = z.infer<
  typeof patchAppBehaviorUserPreferencesParamsSchema
>;
