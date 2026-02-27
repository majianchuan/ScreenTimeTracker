import { apiClient } from "@/shared/api";
import type {
  PatchScreenTimeUserSettingsParams,
  PatchShellUserSettingsParams,
  ScreenTimeUserSettingsDto,
  ShellUserSettingsDto,
} from "./schemas";

export const getScreenTimeUserSettingsDto =
  async (): Promise<ScreenTimeUserSettingsDto> => {
    const { data } = await apiClient.get("/screen-time/user-settings");
    return data;
  };

export const getShellUserSettingsDto =
  async (): Promise<ShellUserSettingsDto> => {
    const { data } = await apiClient.get("/shell/user-settings");
    return data;
  };

export const patchScreenTimeUserSettingsDto = async (
  params: PatchScreenTimeUserSettingsParams,
) => {
  const { data } = await apiClient.patch("/screen-time/user-settings", params);
  return data;
};

export const patchShellUserSettingsDto = async (
  params: PatchShellUserSettingsParams,
) => {
  const { data } = await apiClient.patch("/shell/user-settings", params);
  return data;
};
