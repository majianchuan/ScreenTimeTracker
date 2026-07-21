import type { SearchParams } from "../model/schemas";
import {
  DateRangeSelector,
  TimeFrameSelector,
  useDateFilter,
} from "@/features/date-filter";
import { dateOnlyToDate, dateToDateOnly } from "@/shared/lib/date-only";
import {
  DimensionMemberPicker,
  DimensionTypeSelector,
  type Dimension,
} from "@/features/dimension-control";
import { useEffect, useRef, useState } from "react";
import { useNavigate } from "@tanstack/react-router";
import {
  UsageDistributionList,
  UsageDistributionPieChart,
} from "@/features/usage-distribution";
import { UsageTimeline } from "@/features/usage-timeline";
import { UsageChart } from "@/features/usage-chart";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import ToggleButton from "@mui/material/ToggleButton";
import Select from "@mui/material/Select";
import MenuItem from "@mui/material/MenuItem";
import CircularProgress from "@mui/material/CircularProgress";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import z from "zod";
import Stack from "@mui/material/Stack";
import { useTranslation } from "react-i18next";

interface UsageSummaryPageProps {
  search: SearchParams;
  onSearchChange: (newParams: Partial<SearchParams>) => void;
}

const dimensionCacheSchema = z.record(
  z.enum(["app", "app-category"]),
  z.array(z.string()),
);
type DimensionCache = z.infer<typeof dimensionCacheSchema>;

const defaultDimensionCache: DimensionCache = {
  app: [],
  "app-category": [],
};

const DIMENSION_CACHE_STORAGE_KEY = "page_usage_summary_page_dimension_cache";
const DISTRIBUTION_TYPE_STORAGE_KEY = "page_usage_summary_page_ranking_type";

function getSavedDimensionCache(): DimensionCache | null {
  if (typeof window === "undefined") return null;

  const saved = localStorage.getItem(DIMENSION_CACHE_STORAGE_KEY);
  if (saved === null) return null;
  const parsed = JSON.parse(saved);
  const result = dimensionCacheSchema.safeParse(parsed);
  if (result.success) return result.data;
  else return null;
}

export const UsageSummaryPage = ({
  search,
  onSearchChange,
}: UsageSummaryPageProps) => {
  const { t } = useTranslation(["page_usageSummary", "shared"]);
  const {
    handleTimeFrameChange,
    handleDateRangeChange,
    isLoading: isDateFilterLoading,
  } = useDateFilter({
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

  useEffect(() => {
    if (search.excludedIds !== undefined) return;

    const dimensionCache = getSavedDimensionCache() || defaultDimensionCache;
    onSearchChange({
      excludedIds: dimensionCache[search.dimension],
    });
  }, [search.dimension, search.excludedIds, onSearchChange]);

  const dimensionCacheRef = useRef<DimensionCache>(
    getSavedDimensionCache() || defaultDimensionCache,
  );
  const handleDimensionChange = (newDimension: Dimension) => {
    onSearchChange({
      dimension: newDimension,
      excludedIds: dimensionCacheRef.current[newDimension],
    });
  };
  const handleMemberIdsChange = (newMemberIds: string[]) => {
    dimensionCacheRef.current[search.dimension] = newMemberIds;
    localStorage.setItem(
      DIMENSION_CACHE_STORAGE_KEY,
      JSON.stringify(dimensionCacheRef.current),
    );
    onSearchChange({ excludedIds: newMemberIds });
  };

  const navigate = useNavigate();

  const [distributionType, setDistributionType] = useState(
    localStorage.getItem(DISTRIBUTION_TYPE_STORAGE_KEY) || "pieChart",
  );

  if (isDateFilterLoading)
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
        }}
      >
        <CircularProgress />
      </Box>
    );

  return (
    <Stack spacing={2}>
      {/* 类别切换，时间范围选择 */}
      <Stack spacing={1}>
        <Stack direction="row" sx={{ alignItems: "center" }}>
          <Box sx={{ flex: 1, display: "flex", justifyContent: "start" }}>
            <DimensionTypeSelector
              value={search.dimension}
              onValueChange={handleDimensionChange}
            />
          </Box>
          <TimeFrameSelector
            value={search.timeFrame}
            onValueChange={handleTimeFrameChange}
          />
          <Box sx={{ flex: 1, display: "flex", justifyContent: "end" }}>
            <DimensionMemberPicker
              sx={{ width: "90%" }}
              dimension={search.dimension}
              value={search.excludedIds || []}
              onValueChange={handleMemberIdsChange}
              mode="multiple"
              placeholder={t("filters.excludePlaceholder")}
            />
          </Box>
        </Stack>

        <Box sx={{ display: "flex", justifyContent: "center" }}>
          <DateRangeSelector
            sx={{ minWidth: "20rem" }}
            timeFrame={search.timeFrame}
            value={{
              start: dateOnlyToDate(search.startDate),
              end: dateOnlyToDate(search.endDate),
            }}
            onValueChange={handleDateRangeChange}
          />
        </Box>
      </Stack>

      {/* 使用时间柱状图 */}
      <Paper
        variant="outlined"
        sx={{
          p: 2,
        }}
      >
        <UsageChart
          sx={{
            height: "18rem",
          }}
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
      </Paper>

      {/* 日使用时间线 */}
      {search.timeFrame === "day" ? (
        <Paper
          variant="outlined"
          sx={{
            p: 2,
          }}
        >
          <UsageTimeline
            sx={{
              height: "7.5rem",
            }}
            type={search.dimension}
            date={search.startDate}
            excludedIds={search.excludedIds}
          />
        </Paper>
      ) : (
        <></>
      )}

      {/* 使用分布 */}
      <Paper
        variant="outlined"
        sx={{
          p: 2,
        }}
      >
        <Stack spacing={1}>
          <Stack
            direction="row"
            sx={{
              justifyContent: "space-between",
            }}
          >
            <ToggleButtonGroup
              size="small"
              exclusive
              value={distributionType}
              onChange={(_, newType) => {
                if (newType !== null) {
                  setDistributionType(newType);
                  localStorage.setItem(DISTRIBUTION_TYPE_STORAGE_KEY, newType);
                }
              }}
            >
              <ToggleButton value="pieChart">
                {t("viewTypes.pieChart")}
              </ToggleButton>
              <ToggleButton value="list">{t("viewTypes.list")}</ToggleButton>
            </ToggleButtonGroup>
            <Select
              value={search.topN}
              size="small"
              onChange={(event) => onSearchChange({ topN: event.target.value })}
            >
              <MenuItem value={5}>5</MenuItem>
              <MenuItem value={10}>10</MenuItem>
              <MenuItem value={20}>20</MenuItem>
            </Select>
          </Stack>
          {distributionType === "pieChart" ? (
            <UsageDistributionPieChart
              sx={{
                height: "25rem",
              }}
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
          ) : (
            <UsageDistributionList
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
          )}
        </Stack>
      </Paper>
    </Stack>
  );
};
