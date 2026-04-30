import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  getAppBehaviorUserPreferencesDto,
  patchAppBehaviorUserPreferencesDto,
} from "./requests";
import type {
  PatchAppBehaviorUserPreferencesParams,
  AppBehaviorUserPreferencesDto,
} from "./schemas";

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
  return useMutation<
    AppBehaviorUserPreferencesDto,
    Error,
    PatchAppBehaviorUserPreferencesParams
  >({
    mutationFn: patchAppBehaviorUserPreferencesDto,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["app-behavior-user-preferences"],
      });
    },
  });
};
