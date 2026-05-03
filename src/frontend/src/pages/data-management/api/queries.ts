import { queryOptions, useMutation } from "@tanstack/react-query";
import { deleteUsageData, exportData, importData } from "./requests";
import type { DeleteUsageDataParams, ImportDataDto } from "./schemas";

export const useDeleteData = () => {
  return useMutation<void, Error, DeleteUsageDataParams>({
    mutationFn: deleteUsageData,
  });
};

export const exportDataQueryOptions = () => {
  return queryOptions({
    queryKey: ["export-data"],
    queryFn: () => exportData(),
  });
};

export const useImportData = () => {
  return useMutation<ImportDataDto, Error, string>({
    mutationFn: importData,
  });
};
