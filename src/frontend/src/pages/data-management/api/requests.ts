import { apiClient } from "@/shared/api";
import type { DeleteUsageDataParams } from "./schemas";

export const deleteUsageData = async (params: DeleteUsageDataParams) => {
  const { data } = await apiClient.delete(`/screen-time/usage-data`, {
    params,
  });
  return data;
};
