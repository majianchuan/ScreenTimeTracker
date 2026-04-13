import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  getScreenTimeUserSettingsDto,
  getAppBehaviorUserPreferencesDto,
  patchScreenTimeUserSettingsDto,
  patchAppBehaviorUserPreferencesDto,
} from "./requests";
import type {
  PatchScreenTimeUserSettingsParams,
  PatchAppBehaviorUserPreferencesParams,
  ScreenTimeUserSettingsDto,
  AppBehaviorUserPreferencesDto,
} from "./schemas";

export const screenTimeUserSettingsQueries = {
  screenTimeUserSettings: () => {
    return queryOptions({
      queryKey: ["screen-time-user-settings"],
      queryFn: getScreenTimeUserSettingsDto,
      staleTime: 0,
    });
  },
};

export const usePatchScreenTimeUserSettings = () => {
  const queryClient = useQueryClient();
  return useMutation<
    ScreenTimeUserSettingsDto,
    Error,
    PatchScreenTimeUserSettingsParams
  >({
    mutationFn: patchScreenTimeUserSettingsDto,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["screen-time-user-settings"],
      });
    },
  });
};

export const appBehaviorUserPreferencesQueries = {
  appBehaviorUserPreferences: () => {
    return queryOptions({
      queryKey: ["app-behavior-user-preferences"],
      queryFn: getAppBehaviorUserPreferencesDto,
      staleTime: 0,
    });
  },
};

export const usePatchAppBehaviorUserPreferences = () => {
  const queryClient = useQueryClient();
  return useMutation<AppBehaviorUserPreferencesDto, Error, PatchAppBehaviorUserPreferencesParams>(
    {
      mutationFn: patchAppBehaviorUserPreferencesDto,
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ["app-behavior-user-preferences"] });
      },
    },
  );
};
