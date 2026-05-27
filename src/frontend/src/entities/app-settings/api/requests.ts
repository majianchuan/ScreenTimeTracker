import { apiClient } from "@/shared/api";
import type { PatchAppSettingsParams, AppSettingsDto } from "./schemas";

export const getAppSettingsDto = async (): Promise<AppSettingsDto> => {
  const { data } = await apiClient.get("/desktop/app-settings");
  return data;
};

export const patchAppSettingsDto = async (params: PatchAppSettingsParams) => {
  const { data } = await apiClient.patch("/desktop/app-settings", params);
  return data;
};
