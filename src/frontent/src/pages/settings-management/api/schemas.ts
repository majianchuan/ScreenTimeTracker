import { z } from "zod";

export const screenTimeUserSettingsDtoSchema = z.object({
  samplingIntervalMilliseconds: z.int(),
  idleDetection: z.boolean(),
  idleTimeoutSeconds: z.int(),
  appInfoStaleThresholdMinutes: z.int(),
  aggregationIntervalMinutes: z.int(),
  appIconDirectory: z.string(),
});
export type ScreenTimeUserSettingsDto = z.infer<
  typeof screenTimeUserSettingsDtoSchema
>;

export const shellUserSettingsDtoSchema = z.object({
  uiOpenMode: z.enum(["Window", "Browser"]),
  autoStart: z.boolean(),
  silentStart: z.boolean(),
  language: z.string(),
  windowDestroyOnClose: z.boolean(),
});
export type ShellUserSettingsDto = z.infer<typeof shellUserSettingsDtoSchema>;

// api 参数类型
export const patchScreenTimeUserSettingsParamsSchema =
  screenTimeUserSettingsDtoSchema.partial();
export type PatchScreenTimeUserSettingsParams = z.infer<
  typeof patchScreenTimeUserSettingsParamsSchema
>;

export const patchShellUserSettingsParamsSchema =
  shellUserSettingsDtoSchema.partial();
export type PatchShellUserSettingsParams = z.infer<
  typeof patchShellUserSettingsParamsSchema
>;
