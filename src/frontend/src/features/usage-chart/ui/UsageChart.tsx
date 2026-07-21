import { useQuery } from "@tanstack/react-query";
import ReactECharts from "echarts-for-react";
import {
  appCategoryUsageQueryOptions,
  appUsageQueryOptions,
} from "../api/queries";
import type { DateOnly } from "@/shared/lib/date-only";
import { formatSecondsDuration } from "@/shared/lib/time";
import { useMemo } from "react";
import type { Theme } from "@emotion/react";
import { useTheme, type SxProps } from "@mui/material/styles";
import dayjs from "@/shared/lib/dayjs";
import Box from "@mui/material/Box";
import { useTranslation } from "react-i18next";

export type UsageChartProps = {
  className?: string;
  sx?: SxProps<Theme>;
  type: "app" | "app-category";
  granularity: "hour" | "day";
  xAxisType: "hour" | "day" | "week";
  startDate: DateOnly;
  endDate: DateOnly;
  includedIds?: string[];
  excludedIds?: string[];
};

export const UsageChart = ({
  className,
  sx,
  type,
  granularity,
  xAxisType,
  startDate,
  endDate,
  includedIds,
  excludedIds,
}: UsageChartProps) => {
  const { t } = useTranslation(["feature_usageChart", "shared"]);
  const { data: appUsageData } = useQuery({
    ...appUsageQueryOptions({
      granularity: granularity,
      startDate: startDate,
      endDate: endDate,
      includedIds: includedIds,
      excludedIds: excludedIds,
    }),
    enabled: type === "app",
  });
  const { data: appCategoryUsageData } = useQuery({
    ...appCategoryUsageQueryOptions({
      granularity: granularity,
      startDate: startDate,
      endDate: endDate,
      includedIds: includedIds,
      excludedIds: excludedIds,
    }),
    enabled: type === "app-category",
  });

  const theme = useTheme();
  const isDark = theme.palette.mode === "dark";

  const option = useMemo(() => {
    const usageData =
      (type === "app" ? appUsageData : appCategoryUsageData) || [];
    const sum = usageData.reduce((acc, cur) => acc + cur.durationSeconds, 0);
    const avg = Math.round(sum / (usageData.length || 1));
    const normalized = usageData.map((item) => {
      let name = "";
      // 判断是小时数据还是日期数据
      if (xAxisType === "hour") {
        name = item.startTime.getHours().toString().padStart(2, "0") + ":00";
      } else if (xAxisType === "day") {
        name = dayjs(item.startTime).format("L");
      } else if (xAxisType === "week")
        name = dayjs(item.startTime).format("ddd");

      return { name, value: item.durationSeconds };
    });

    const formattedSum = formatSecondsDuration(sum);
    const formattedAvg = formatSecondsDuration(avg);

    const titleText =
      granularity !== "hour"
        ? t("chart.titleWithAverage", {
            total: formattedSum,
            average: formattedAvg,
          })
        : t("chart.titleWithoutAverage", { total: formattedSum });

    return {
      backgroundColor: "transparent",
      grid: {
        bottom: 30,
        left: 70,
        right: 70,
      },
      title: {
        text: titleText,
      },
      tooltip: {
        trigger: "axis",
        formatter: (params: { name: string; value: number }[]) => {
          const item = params[0];
          return `${item.name}<br/>${formatSecondsDuration(item.value)}`;
        },
      },
      xAxis: {
        data: normalized.map((v) => v.name),
      },
      yAxis: {
        axisLabel: {
          formatter: (val: number) => formatSecondsDuration(val),
        },
      },
      series: [
        {
          type: "bar",
          data: normalized.map((v) => v.value),
          markLine:
            granularity === "hour"
              ? undefined
              : {
                  animation: false,
                  data: [
                    {
                      yAxis: avg,
                      label: {
                        formatter: t("chart.averageLabel", {
                          value: formattedAvg,
                        }),
                      },
                    },
                  ],
                },
        },
      ],
    };
  }, [granularity, type, xAxisType, appUsageData, appCategoryUsageData]);

  return (
    <Box
      sx={[
        { minWidth: 0, width: "100%", overflow: "hidden" },
        ...(Array.isArray(sx) ? sx : [sx]),
      ]}
      className={className}
    >
      <ReactECharts
        style={{
          height: "100%",
          width: "100%",
        }}
        theme={isDark ? "dark" : undefined}
        option={option}
        notMerge={true}
      />
    </Box>
  );
};
