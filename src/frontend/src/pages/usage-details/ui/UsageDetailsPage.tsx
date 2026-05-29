import type { SearchParams } from "../model/schemas";
import {
  DateRangeSelector,
  PeriodTypeSelector,
  useDateFilter,
} from "@/features/date-filter";
import { dateOnlyToDate, dateToDateOnly } from "@/shared/lib/date-only";
import {
  DimensionMemberPicker,
  DimensionTypeSelector,
  useDimensionControl,
} from "@/features/dimension-control";
import { useEffect, useMemo } from "react";
import { UsageChart } from "@/features/usage-chart";
import { UsageTimeline } from "@/features/usage-timeline";

interface UsageDetailsPageProps {
  search: SearchParams;
  onSearchChange: (newParams: Partial<SearchParams>) => void;
}

export const UsageDetailsPage = ({
  search,
  onSearchChange,
}: UsageDetailsPageProps) => {
  const DIMENSION_CACHE_STORAGE_KEY = "page_usage_details_page_dimension_cache";

  const { handleTimeFrameChange, handleDateRangeChange } = useDateFilter({
    currentTimeFrame: search.timeFrame,
    currentDateRange: {
      start: dateOnlyToDate(search.startDate),
      end: dateOnlyToDate(search.endDate),
    },
    onValueChange: (newTimeFrame, newDateRange) => {
      onSearchChange({
        timeFrame: newTimeFrame,
        startDate: dateToDateOnly(newDateRange.start),
        endDate: dateToDateOnly(newDateRange.end),
      });
    },
  });

  const getSavedDimensionCache = () => {
    if (typeof window === "undefined") return {};

    const saved = localStorage.getItem(DIMENSION_CACHE_STORAGE_KEY);
    try {
      return saved ? JSON.parse(saved) : {};
    } catch {
      return {};
    }
  };

  const initialDimensionCache = useMemo(() => getSavedDimensionCache(), []);

  useEffect(() => {
    if (search.id) return;

    onSearchChange({
      id: (getSavedDimensionCache()[search.dimension] || [])[0],
    });
  }, [search.dimension, search.id, onSearchChange]);

  const { handleDimensionChange, handleMemberIdsChange } = useDimensionControl({
    currentDimension: search.dimension,
    initialCache: initialDimensionCache,
    onValueChange: (dimension, memberIds) => {
      onSearchChange({ dimension, id: memberIds[0] });
    },
    onCacheSync: (cache) => {
      localStorage.setItem(DIMENSION_CACHE_STORAGE_KEY, JSON.stringify(cache));
    },
  });

  return (
    <>
      <div>
        <div className="flex w-full">
          <div className="flex flex-1 justify-start">
            <DimensionTypeSelector
              value={search.dimension}
              onValueChange={handleDimensionChange}
            />
          </div>
          <div className="flex flex-1 justify-center">
            <PeriodTypeSelector
              value={search.timeFrame}
              onValueChange={handleTimeFrameChange}
            />
          </div>
          <div className="flex flex-1 justify-end">
            <DimensionMemberPicker
              dimension={search.dimension}
              value={search.id ? [search.id] : []}
              onValueChange={handleMemberIdsChange}
              mode="single"
              placeholder="查看项"
            />
          </div>
        </div>

        <div className="mt-2 flex justify-center">
          <DateRangeSelector
            timeFrame={search.timeFrame}
            value={{
              start: dateOnlyToDate(search.startDate),
              end: dateOnlyToDate(search.endDate),
            }}
            onValueChange={handleDateRangeChange}
          />
        </div>
      </div>

      {/* 使用时间柱状图 */}
      <div className="border-border mt-4 rounded-lg border p-3">
        {search.id ? (
          <UsageChart
            type={search.dimension}
            granularity={search.timeFrame === "day" ? "hour" : "day"}
            xAxisType={
              search.timeFrame === "day"
                ? "hour"
                : search.timeFrame === "week"
                  ? "week"
                  : "day"
            }
            startDate={search.startDate}
            endDate={search.endDate}
            includedIds={[search.id]}
          />
        ) : (
          <div className="text-center">请选择查看项</div>
        )}

        {/* 日使用时间线 */}
      </div>
      {search.timeFrame === "day" && search.id ? (
        <div className="border-border mt-4 rounded-lg border p-3">
          <UsageTimeline
            className="h-30! w-full"
            type={search.dimension}
            date={search.startDate}
            includedIds={[search.id]}
          />
        </div>
      ) : (
        <></>
      )}
    </>
  );
};
