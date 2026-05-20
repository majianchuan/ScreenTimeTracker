import { useRef } from "react";
import type { Dimension } from "./schemas";

export type DimensionCache = Partial<Record<Dimension, string[]>>;

export type UseDimensionControlOptions = {
  currentDimension: Dimension;
  initialCache?: DimensionCache;
  onValueChange: (dimension: Dimension, memberIds: string[]) => void;
  onCacheSync?: (cache: DimensionCache) => void;
};

export const useDimensionControl = ({
  currentDimension,
  initialCache,
  onValueChange,
  onCacheSync,
}: UseDimensionControlOptions) => {
  const dimensionCacheRef = useRef<DimensionCache>(initialCache || {});

  const handleDimensionChange = (newDimension: Dimension) => {
    const nextMemberIds = dimensionCacheRef.current[newDimension] || [];
    onValueChange(newDimension, nextMemberIds);
  };

  const handleMemberIdsChange = (newMemberIds: string[]) => {
    dimensionCacheRef.current[currentDimension] = newMemberIds;
    onCacheSync?.(dimensionCacheRef.current);
    onValueChange(currentDimension, newMemberIds);
  };

  return {
    handleDimensionChange,
    handleMemberIdsChange,
  };
};
