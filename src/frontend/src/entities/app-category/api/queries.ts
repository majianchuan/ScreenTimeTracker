import {
  queryOptions,
  useMutation,
  useQueryClient,
} from "@tanstack/react-query";
import {
  createAppCategory,
  deleteAppCategory,
  getAppCategories,
  patchAppCategory,
} from "./requests";
import type {
  CreateAppCategoryParams,
  GetAppCategoriesParams,
  PatchAppCategoryParams,
} from "./schemas";
import type { AppCategory } from "../model/schemas";

export const appCategoryKeys = {
  all: ["app-category"] as const,
  lists: () => [...appCategoryKeys.all, "list"] as const,
  list: (params: GetAppCategoriesParams) =>
    [...appCategoryKeys.lists(), params] as const,
};

export const appCategoryQueries = {
  appCategories: (params: GetAppCategoriesParams) => {
    return queryOptions({
      queryKey: appCategoryKeys.list(params),
      queryFn: () => getAppCategories(params),
      staleTime: 0,
    });
  },
};

export const useCreateAppCategory = () => {
  const queryClient = useQueryClient();
  return useMutation<AppCategory, Error, CreateAppCategoryParams>({
    mutationFn: createAppCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: appCategoryKeys.all });
    },
  });
};

export const usePatchAppCategory = () => {
  const queryClient = useQueryClient();
  return useMutation<AppCategory, Error, PatchAppCategoryParams>({
    mutationFn: patchAppCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: appCategoryKeys.all });
    },
  });
};

export const useDeleteAppCategory = () => {
  const queryClient = useQueryClient();
  return useMutation<AppCategory, Error, string>({
    mutationFn: deleteAppCategory,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: appCategoryKeys.all });
    },
  });
};
