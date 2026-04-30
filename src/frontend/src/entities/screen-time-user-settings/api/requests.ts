import { apiClient } from "@/shared/api";
import type {
  PatchScreenTimeUserSettingsParams,
  ScreenTimeUserSettingsDto,
} from "./schemas";

export const getScreenTimeUserSettingsDto =
  async (): Promise<ScreenTimeUserSettingsDto> => {
    const { data } = await apiClient.get("/screen-time/user-settings");
    return data;
  };

export const patchScreenTimeUserSettingsDto = async (
  params: PatchScreenTimeUserSettingsParams,
) => {
  const { data } = await apiClient.patch("/screen-time/user-settings", params);
  return data;
};
