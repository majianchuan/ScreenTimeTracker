import { z } from "zod";

export const userSettingsDtoSchema = z.object({
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
export type UserSettingsDto = z.infer<typeof userSettingsDtoSchema>;

// api 参数类型
export const patchUserSettingsParamsSchema = userSettingsDtoSchema.partial();
export type PatchUserSettingsParams = z.infer<
  typeof patchUserSettingsParamsSchema
>;
