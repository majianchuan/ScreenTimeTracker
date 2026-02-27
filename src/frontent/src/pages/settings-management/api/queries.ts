import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  getScreenTimeUserSettingsDto,
  getShellUserSettingsDto,
  patchScreenTimeUserSettingsDto,
  patchShellUserSettingsDto,
} from "./requests";
import type {
  PatchScreenTimeUserSettingsParams,
  PatchShellUserSettingsParams,
  ScreenTimeUserSettingsDto,
  ShellUserSettingsDto,
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

export const shellUserSettingsQueries = {
  shellUserSettings: () => {
    return queryOptions({
      queryKey: ["shell-user-settings"],
      queryFn: getShellUserSettingsDto,
      staleTime: 0,
    });
  },
};

export const usePatchShellUserSettings = () => {
  const queryClient = useQueryClient();
  return useMutation<ShellUserSettingsDto, Error, PatchShellUserSettingsParams>(
    {
      mutationFn: patchShellUserSettingsDto,
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: ["shell-user-settings"] });
      },
    },
  );
};
