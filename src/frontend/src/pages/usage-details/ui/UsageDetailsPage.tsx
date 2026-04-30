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
import { useQuery } from "@tanstack/react-query";
import { startOfDay, subHours } from "date-fns";
import { screenTimeUserSettingsQueries } from "@/entities/screen-time-user-settings";
import { UsageChart } from "@/features/usage-chart";

interface UsageDetailsPageProps {
  search: SearchParams;
  onSearchChange: (newParams: Partial<SearchParams>) => void;
}

export const UsageDetailsPage = ({
  search,
  onSearchChange,
}: UsageDetailsPageProps) => {
  const DIMENSION_CACHE_STORAGE_KEY = "page_usage_details_page_dimension_cache";
  const { data: settings } = useQuery(
    screenTimeUserSettingsQueries.screenTimeUserSettings(),
  );
  const offsetHours = settings?.dayCutoffHour ?? 0;
  const logicalTodayDateOnly = useMemo(() => {
    const logicalToday = startOfDay(subHours(new Date(), offsetHours));
    return dateToDateOnly(logicalToday);
  }, [offsetHours]);
  const effectiveStartDate = search.startDate ?? logicalTodayDateOnly;
  const effectiveEndDate = search.endDate ?? logicalTodayDateOnly;

  const { handleTimeFrameChange, handleDateRangeChange } = useDateFilter({
    currentTimeFrame: search.timeFrame,
    currentDateRange: {
      start: dateOnlyToDate(effectiveStartDate),
      end: dateOnlyToDate(effectiveEndDate),
    },
    onChange: (newTimeFrame, newDateRange) => {
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
    onChange: (dimension, memberIds) => {
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
              onChange={handleDimensionChange}
            />
          </div>
          <div className="flex flex-1 justify-center">
            <PeriodTypeSelector
              value={search.timeFrame}
              onChange={handleTimeFrameChange}
            />
          </div>
          <div className="flex flex-1 justify-end">
            <DimensionMemberPicker
              dimension={search.dimension}
              value={search.id ? [search.id] : []}
              onChange={handleMemberIdsChange}
              mode="single"
              placeholder="查看项"
            />
          </div>
        </div>

        <div className="mt-2 flex justify-center">
          <DateRangeSelector
            timeFrame={search.timeFrame}
            value={{
              start: dateOnlyToDate(effectiveStartDate),
              end: dateOnlyToDate(effectiveEndDate),
            }}
            onChange={handleDateRangeChange}
          />
        </div>
      </div>

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
            startDate={effectiveStartDate}
            endDate={effectiveEndDate}
            includedIds={[search.id]}
          />
        ) : (
          <div className="text-center">请选择查看项</div>
        )}
      </div>
    </>
  );
};
