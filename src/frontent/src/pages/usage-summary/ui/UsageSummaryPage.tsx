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
import { UsageOverTimeChart } from "@/widgets/usage-over-time-chart";
import {
  SelectTrigger,
  SelectContent,
  Select,
  SelectValue,
  SelectItem,
  SelectGroup,
  Button,
  Progress,
} from "@/shared/lib/shadcn";
import { useQuery } from "@tanstack/react-query";
import { usageQueries, useUsageConfig } from "@/entities/usage";
import { useNavigate } from "@tanstack/react-router";
import { AppIcon } from "@/entities/app";
import { AppCategoryIcon } from "@/entities/app-category";
import { formatSeconds } from "@/shared/lib/time";

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
  const { refetchIntervalSeconds } = useUsageConfig();

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
  const { data: rankingData } = useQuery({
    ...usageQueries.rankings({
      dimension: search.dimension,
      startDate: search.startDate,
      endDate: search.endDate,
      topN: search.topN,
      excludedIds: search.excludedIds,
    }),
    refetchInterval: refetchIntervalSeconds * 1000,
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

      <div className="border-border mt-4 rounded-lg border p-3">
        <UsageOverTimeChart
          className="h-[50vh] min-h-70 w-full"
          mode="summary"
          timeFrame={search.timeFrame}
          dimension={search.dimension}
          startDate={search.startDate}
          endDate={search.endDate}
          excludedIds={search.excludedIds}
        />
      </div>

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
        <div className="mt-2 flex flex-col gap-1">
          {rankingData?.map((item) => (
            <Button
              variant="ghost"
              className="flex h-auto w-full flex-row px-2.5 py-1"
              key={`${search.dimension}-${item.id}`}
              onClick={() =>
                navigate({
                  to: "/usage/details",
                  search: {
                    dimension: search.dimension,
                    id: item.id,
                    timeFrame: search.timeFrame,
                    startDate: search.startDate,
                    endDate: search.endDate,
                  },
                })
              }
            >
              {search.dimension === "app" ? (
                <AppIcon
                  className="size-8"
                  id={item.id}
                  iconPath={item.iconPath}
                />
              ) : (
                <AppCategoryIcon
                  className="size-8"
                  id={item.id}
                  iconPath={item.iconPath}
                />
              )}
              <div className="flex flex-1 flex-col">
                <div className="flex w-full flex-1 flex-row items-center justify-between">
                  <div className="text-base">{item.name}</div>
                  <div>{formatSeconds(item.durationSeconds)}</div>
                </div>
                <div className="flex w-full flex-row items-center gap-2">
                  <Progress value={item.percentage} className="flex-1" />
                  <div>{item.percentage}%</div>
                </div>
              </div>
            </Button>
          ))}
        </div>
      </div>
    </>
  );
};
