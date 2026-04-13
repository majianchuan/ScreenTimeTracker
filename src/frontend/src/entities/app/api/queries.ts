import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import { deleteApp, getApps, patchApp } from "./requests";
import type { GetAppsParams, PatchAppParams } from "./schemas";
import type { App } from "../model/schemas";

export const appKeys = {
  all: ["app"] as const,
  lists: () => [...appKeys.all, "list"] as const,
  list: (params: GetAppsParams) => [...appKeys.lists(), params] as const,
};

export const appQueries = {
  apps: (params: GetAppsParams) => {
    return queryOptions({
      queryKey: appKeys.list(params),
      queryFn: () => getApps(params),
      staleTime: 0,
    });
  },
};

export const usePatchApp = () => {
  const queryClient = useQueryClient();
  return useMutation<App, Error, PatchAppParams>({
    mutationFn: patchApp,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: appKeys.all });
    },
  });
};

export const useDeleteApp = () => {
  const queryClient = useQueryClient();
  return useMutation<App, Error, string>({
    mutationFn: deleteApp,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: appKeys.all });
    },
  });
};
