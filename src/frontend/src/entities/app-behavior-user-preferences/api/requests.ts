import { apiClient } from "@/shared/api";
import type {
  PatchAppBehaviorUserPreferencesParams,
  AppBehaviorUserPreferencesDto,
} from "./schemas";

export const getAppBehaviorUserPreferencesDto =
  async (): Promise<AppBehaviorUserPreferencesDto> => {
    const { data } = await apiClient.get("/app-behavior/user-preferences");
    return data;
  };

export const patchAppBehaviorUserPreferencesDto = async (
  params: PatchAppBehaviorUserPreferencesParams,
) => {
  const { data } = await apiClient.patch(
    "/app-behavior/user-preferences",
    params,
  );
  return data;
};
