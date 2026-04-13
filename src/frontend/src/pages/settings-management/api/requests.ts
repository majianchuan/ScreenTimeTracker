import { apiClient } from "@/shared/api";
import type {
  PatchScreenTimeUserSettingsParams,
  PatchAppBehaviorUserPreferencesParams ,
  ScreenTimeUserSettingsDto,
  AppBehaviorUserPreferencesDto,
} from "./schemas";

export const getScreenTimeUserSettingsDto =
  async (): Promise<ScreenTimeUserSettingsDto> => {
    const { data } = await apiClient.get("/screen-time/user-settings");
    return data;
  };

export const getAppBehaviorUserPreferencesDto =
  async (): Promise<AppBehaviorUserPreferencesDto> => {
    const { data } = await apiClient.get("/app-behavior/user-preferences");
    return data;
  };

export const patchScreenTimeUserSettingsDto = async (
  params: PatchScreenTimeUserSettingsParams,
) => {
  const { data } = await apiClient.patch("/screen-time/user-settings", params);
  return data;
};

export const patchAppBehaviorUserPreferencesDto = async (
  params: PatchAppBehaviorUserPreferencesParams,
) => {
  const { data } = await apiClient.patch("/app-behavior/user-preferences", params);
  return data;
};
