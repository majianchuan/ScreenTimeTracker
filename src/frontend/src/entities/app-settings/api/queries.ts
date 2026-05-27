import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { getAppSettingsDto, patchAppSettingsDto } from "./requests";
import type { PatchAppSettingsParams, AppSettingsDto } from "./schemas";

export const appSettingsQueries = {
  appSettings: () => {
    return queryOptions({
      queryKey: ["app-settings"],
      queryFn: getAppSettingsDto,
      staleTime: 0,
    });
  },
};

export const usePatchAppSettings = () => {
  const queryClient = useQueryClient();
  return useMutation<AppSettingsDto, Error, PatchAppSettingsParams>({
    mutationFn: patchAppSettingsDto,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["app-settings"],
      });
    },
  });
};
