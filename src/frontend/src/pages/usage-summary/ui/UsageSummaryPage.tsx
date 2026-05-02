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
import {
  SelectTrigger,
  SelectContent,
  Select,
  SelectValue,
  SelectItem,
  SelectGroup,
} from "@/shared/lib/shadcn";
import { useNavigate } from "@tanstack/react-router";
import { UsageRanking } from "@/features/usage-ranking";
import { UsageTimeline } from "@/features/usage-timeline";
import { UsageChart } from "@/features/usage-chart";

interface UsageSummaryPageProps {
  search: SearchParams;
  onSearchChange: (newParams: Partial<SearchParams>) => void;
}

export const UsageSummaryPage = ({
  search,
  onSearchChange,
}: UsageSummaryPageProps) => {
  const DIMENSION_CACHE_STORAGE_KEY = "page_usage_summary_page_dimension_cache";

  const { handleTimeFrameChange, handleDateRangeChange } = useDateFilter({
    currentTimeFrame: search.timeFrame,
    currentDateRange: {
      start: dateOnlyToDate(search.startDate),
      end: dateOnlyToDate(search.endDate),
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
    if (search.excludedIds) return;

    onSearchChange({
      excludedIds: getSavedDimensionCache()[search.dimension] || [],
    });
  }, [search.dimension, search.excludedIds, onSearchChange]);

  const { handleDimensionChange, handleMemberIdsChange } = useDimensionControl({
    currentDimension: search.dimension,
    initialCache: initialDimensionCache,
    onChange: (dimension, memberIds) => {
      onSearchChange({ dimension, excludedIds: memberIds });
    },
    onCacheSync: (cache) => {
      localStorage.setItem(DIMENSION_CACHE_STORAGE_KEY, JSON.stringify(cache));
    },
  });

  const navigate = useNavigate();

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
              value={search.excludedIds || []}
              onChange={handleMemberIdsChange}
              mode="multiple"
              placeholder="排除项"
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
            onChange={handleDateRangeChange}
          />
        </div>
      </div>

      {/* 使用时间柱状图 */}
      <div className="border-border mt-4 rounded-lg border p-3">
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
          excludedIds={search.excludedIds}
        />
      </div>

      {/* 日使用时间线 */}
      {search.timeFrame === "day" ? (
        <div className="border-border mt-4 rounded-lg border p-3">
          <UsageTimeline
            className="h-30! w-full"
            type={search.dimension}
            date={search.startDate}
            excludedIds={search.excludedIds}
          />
        </div>
      ) : (
        <></>
      )}

      {/* 使用排名 */}
      <div className="border-border mt-4 rounded-lg border p-3">
        <div className="flex flex-row justify-end">
          <Select
            value={`${search.topN}`}
            onValueChange={(value) => onSearchChange({ topN: Number(value) })}
          >
            <SelectTrigger className="w-40">
              <SelectValue />
            </SelectTrigger>
            <SelectContent position="popper">
              <SelectGroup>
                <SelectItem value="5">5</SelectItem>
                <SelectItem value="10">10</SelectItem>
                <SelectItem value="20">20</SelectItem>
              </SelectGroup>
            </SelectContent>
          </Select>
        </div>
        <UsageRanking
          className="mt-4"
          type={search.dimension}
          startDate={search.startDate}
          endDate={search.endDate}
          topN={search.topN}
          onItemClick={(id) =>
            navigate({
              to: "/usage/details",
              search: {
                dimension: search.dimension,
                id: id,
                timeFrame: search.timeFrame,
                startDate: search.startDate,
                endDate: search.endDate,
              },
            })
          }
          excludedIds={search.excludedIds}
        />
      </div>
    </>
  );
};
