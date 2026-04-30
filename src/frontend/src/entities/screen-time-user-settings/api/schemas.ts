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

  dayCutoffHour: z.int(),
});
export type ScreenTimeUserSettingsDto = z.infer<
  typeof screenTimeUserSettingsDtoSchema
>;

// api 参数类型
export const patchScreenTimeUserSettingsParamsSchema =
  screenTimeUserSettingsDtoSchema.partial();
export type PatchScreenTimeUserSettingsParams = z.infer<
  typeof patchScreenTimeUserSettingsParamsSchema
>;
