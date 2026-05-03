import { apiClient } from "@/shared/api";
import type { DeleteUsageDataParams, ImportDataDto } from "./schemas";

export const deleteUsageData = async (params: DeleteUsageDataParams) => {
  const { data } = await apiClient.delete(`/screen-time/data`, {
    params,
  });
  return data;
};

export const exportData = async () => {
  const { data } = await apiClient.get(`/screen-time/data/export`);
  return data;
};

export const importData = async (params: string): Promise<ImportDataDto> => {
  const { data } = await apiClient.post(`/screen-time/data/import`, params);
  return data;
};
