import { useMutation } from "@tanstack/react-query";
import { deleteUsageData } from "./requests";
import type { DeleteUsageDataParams } from "./schemas";

export const useDeleteData = () => {
  return useMutation<void, Error, DeleteUsageDataParams>({
    mutationFn: deleteUsageData,
  });
};
