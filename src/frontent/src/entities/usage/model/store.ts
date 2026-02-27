import { create } from "zustand";
import { persist } from "zustand/middleware";

const STORAGE_KEY = "entity_usage_config";
const DEFAULT_REFETCH_INTERVAL_SECONDS = 10;

interface UsageConfigState {
  refetchIntervalSeconds: number;
  setRefetchIntervalSeconds: (interval: number) => void;
  resetRefetchIntervalSeconds: () => void;
}

export const useUsageConfig = create<UsageConfigState>()(
  persist(
    (set) => ({
      refetchIntervalSeconds: DEFAULT_REFETCH_INTERVAL_SECONDS,
      setRefetchIntervalSeconds: (interval) =>
        set({ refetchIntervalSeconds: interval }),
      resetRefetchIntervalSeconds: () =>
        set({ refetchIntervalSeconds: DEFAULT_REFETCH_INTERVAL_SECONDS }),
    }),
    {
      name: STORAGE_KEY,
    },
  ),
);
