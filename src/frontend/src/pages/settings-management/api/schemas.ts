import { z } from "zod";

export const screenTimeUserSettingsDtoSchema = z.object({
  appIconDirectory: z.string(),
  appInfoStaleThresholdMinutes: z.int(),
  activeSessionAutoSaveSeconds: z.int(),

  isIdleDetectionEnabled: z.boolean(),
  idleThresholdSeconds: z.int(),
  idleDetectionPollingIntervalSeconds: z.int(),

  minValidSessionDurationSeconds: z.int(),
  sessionMergeToleranceSeconds: z.int(),
  sessionOptimizationIntervalSeconds: z.int(),

  dayBoundaryOffsetHours: z.int(),
});
export type ScreenTimeUserSettingsDto = z.infer<
  typeof screenTimeUserSettingsDtoSchema
>;

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
export const patchScreenTimeUserSettingsParamsSchema =
  screenTimeUserSettingsDtoSchema.partial();
export type PatchScreenTimeUserSettingsParams = z.infer<
  typeof patchScreenTimeUserSettingsParamsSchema
>;

export const patchAppBehaviorUserPreferencesParamsSchema =
  appBehaviorUserPreferencesDtoSchema.partial();
export type PatchAppBehaviorUserPreferencesParams = z.infer<
  typeof patchAppBehaviorUserPreferencesParamsSchema
>;
