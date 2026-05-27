import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { getUserSettingsDto, patchUserSettingsDto } from "./requests";
import type { PatchUserSettingsParams, UserSettingsDto } from "./schemas";

export const userSettingsQueries = {
  userSettings: () => {
    return queryOptions({
      queryKey: ["user-settings"],
      queryFn: getUserSettingsDto,
      staleTime: 0,
    });
  },
};

export const usePatchuserSettings = () => {
  const queryClient = useQueryClient();
  return useMutation<UserSettingsDto, Error, PatchUserSettingsParams>({
    mutationFn: patchUserSettingsDto,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["user-settings"],
      });
    },
  });
};
