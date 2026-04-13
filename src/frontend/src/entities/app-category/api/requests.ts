import { apiClient, baseApiUrl } from "@/shared/api";
import {
  appCategoryDtoSchema,
  type CreateAppCategoryParams,
  type GetAppCategoriesParams,
  type PatchAppCategoryParams,
} from "./schemas";
import type { AppCategory } from "../model/schemas";

export const getAppCategories = async (
  params: GetAppCategoriesParams,
): Promise<Partial<AppCategory>[]> => {
  const { data } = await apiClient.get("/screen-time/app-categories", {
    params,
  });
  return data.map((dto: unknown): Partial<AppCategory> => {
    const validated = appCategoryDtoSchema.partial().parse(dto);
    return {
      id: validated.id,
      name: validated.name,
      iconPath: validated.iconPath,
      isSystem: validated.isSystem,
    };
  });
};

export const getAppCategoryIconUrl = (appCategoryId: string) =>
  `${baseApiUrl}/screen-time/app-categories/${appCategoryId}/icon`;

export const createAppCategory = async (
  params: CreateAppCategoryParams,
): Promise<AppCategory> => {
  const { data } = await apiClient.post("/screen-time/app-categories", params);
  const validated = appCategoryDtoSchema.parse(data);
  return {
    id: validated.id,
    name: validated.name,
    iconPath: validated.iconPath,
    isSystem: validated.isSystem,
  };
};

export const patchAppCategory = async (params: PatchAppCategoryParams) => {
  const { data } = await apiClient.patch(
    `/screen-time/app-categories/${params.id}`,
    params.body,
  );
  return data;
};

export const deleteAppCategory = async (id: string) => {
  const { data } = await apiClient.delete(`/screen-time/app-categories/${id}`);
  return data;
};
