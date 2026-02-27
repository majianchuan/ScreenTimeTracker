import { useRef } from "react";
import type { Dimension } from "./schemas";

export type DimensionCache = Partial<Record<Dimension, string[]>>;

export type UseDimensionControlOptions = {
  currentDimension: Dimension;
  initialCache?: DimensionCache;
  onChange: (dimension: Dimension, memberIds: string[]) => void;
  onCacheSync?: (cache: DimensionCache) => void;
};

export const useDimensionControl = ({
  currentDimension,
  initialCache,
  onChange,
  onCacheSync,
}: UseDimensionControlOptions) => {
  const dimensionCacheRef = useRef<DimensionCache>(initialCache || {});

  const handleDimensionChange = (newDimension: Dimension) => {
    const nextMemberIds = dimensionCacheRef.current[newDimension] || [];
    onChange(newDimension, nextMemberIds);
  };

  const handleMemberIdsChange = (newMemberIds: string[]) => {
    dimensionCacheRef.current[currentDimension] = newMemberIds;
    onCacheSync?.(dimensionCacheRef.current);
    onChange(currentDimension, newMemberIds);
  };

  return {
    handleDimensionChange,
    handleMemberIdsChange,
  };
};
