import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  getScreenTimeUserSettingsDto,
  patchScreenTimeUserSettingsDto,
} from "./requests";
import type {
  PatchScreenTimeUserSettingsParams,
  ScreenTimeUserSettingsDto,
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
