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
import { useEffect, useRef } from "react";
import { UsageChart } from "@/features/usage-chart";
import { UsageTimeline } from "@/features/usage-timeline";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import CircularProgress from "@mui/material/CircularProgress";
import z from "zod";
import Typography from "@mui/material/Typography";
import Stack from "@mui/material/Stack";
import { useTranslation } from "react-i18next";

interface UsageDetailsPageProps {
  search: SearchParams;
  onSearchChange: (newParams: Partial<SearchParams>) => void;
}

const dimensionCacheSchema = z.record(
  z.enum(["app", "app-category"]),
  z.string().nullable(),
);
type DimensionCache = z.infer<typeof dimensionCacheSchema>;

const defaultDimensionCache: DimensionCache = {
  app: null,
  "app-category": null,
};

const DIMENSION_CACHE_STORAGE_KEY = "page_usage_details_page_dimension_cache";

function getSavedDimensionCache(): DimensionCache | null {
  if (typeof window === "undefined") return null;

  const saved = localStorage.getItem(DIMENSION_CACHE_STORAGE_KEY);
  if (saved === null) return null;
  const parsed = JSON.parse(saved);
  const result = dimensionCacheSchema.safeParse(parsed);
  if (result.success) return result.data;
  else return null;
}

export const UsageDetailsPage = ({
  search,
  onSearchChange,
}: UsageDetailsPageProps) => {
  const { t } = useTranslation(["page_usageDetails", "shared"]);
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
    if (search.id !== undefined) return;

    const dimensionCache = getSavedDimensionCache() || defaultDimensionCache;
    onSearchChange({
      id: dimensionCache[search.dimension],
    });
  }, [search.dimension, search.id, onSearchChange]);

  const dimensionCacheRef = useRef<DimensionCache>(
    getSavedDimensionCache() || defaultDimensionCache,
  );

  const handleDimensionChange = (newDimension: Dimension) => {
    onSearchChange({
      dimension: newDimension,
      id: dimensionCacheRef.current[newDimension],
    });
  };

  const handleMemberIdChange = (newMemberId: string | null) => {
    dimensionCacheRef.current[search.dimension] = newMemberId;
    localStorage.setItem(
      DIMENSION_CACHE_STORAGE_KEY,
      JSON.stringify(dimensionCacheRef.current),
    );
    onSearchChange({ id: newMemberId });
  };

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
              value={search.id || null}
              onValueChange={handleMemberIdChange}
              mode="single"
              placeholder={t("filters.memberPickerPlaceholder")}
            />
          </Box>
        </Stack>

        <Box sx={{ display: "flex", justifyContent: "center" }}>
          <DateRangeSelector
            sx={{ width: "19rem" }}
            timeFrame={search.timeFrame}
            value={{
              start: dateOnlyToDate(search.startDate),
              end: dateOnlyToDate(search.endDate),
            }}
            onValueChange={handleDateRangeChange}
          />
        </Box>
      </Stack>

      {search.id == null ? (
        <Paper
          variant="outlined"
          sx={{
            p: 2,
          }}
        >
          <Typography sx={{ textAlign: "center" }}>
            {t("states.emptyPrompt")}
          </Typography>
        </Paper>
      ) : (
        <>
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
              includedIds={[search.id]}
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
                includedIds={[search.id]}
              />
            </Paper>
          ) : (
            <></>
          )}
        </>
      )}
    </Stack>
  );
};
