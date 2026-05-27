import { apiClient } from "@/shared/api";
import type { PatchUserSettingsParams, UserSettingsDto } from "./schemas";

export const getUserSettingsDto = async (): Promise<UserSettingsDto> => {
  const { data } = await apiClient.get("/screen-time/user-settings");
  return data;
};

export const patchUserSettingsDto = async (params: PatchUserSettingsParams) => {
  const { data } = await apiClient.patch("/screen-time/user-settings", params);
  return data;
};
