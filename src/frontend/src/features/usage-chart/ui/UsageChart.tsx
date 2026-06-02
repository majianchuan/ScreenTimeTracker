import { useQuery } from "@tanstack/react-query";
import ReactECharts from "echarts-for-react";
import {
  appCategoryUsageQueryOptions,
  appUsageQueryOptions,
} from "../api/queries";
import type { DateOnly } from "@/shared/lib/date-only";
import { cn } from "@/shared/lib/shadcn";
import { formatSecondsDuration } from "@/shared/lib/time";
import { useTheme } from "next-themes";
import { useMemo } from "react";
import { format } from "date-fns";

export type UsageChartProps = {
  className?: string;
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
  type,
  granularity,
  xAxisType,
  startDate,
  endDate,
  includedIds,
  excludedIds,
}: UsageChartProps) => {
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
  const { theme, systemTheme } = useTheme();
  const isDark = (theme === "system" ? systemTheme : theme) === "dark";

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
        name = item.startTime.toLocaleDateString();
      } else if (xAxisType === "week") name = format(item.startTime, "EEE");

      return { name, value: item.durationSeconds };
    });

    return {
      backgroundColor: "transparent",
      grid: {
        bottom: 30,
        left: 90,
        right: 90,
      },
      title: {
        text: `总计：${formatSecondsDuration(sum)} ${granularity !== "hour" ? `| 平均：${formatSecondsDuration(avg)}` : ""}`,
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
                        formatter: "平均\n" + formatSecondsDuration(avg),
                      },
                    },
                  ],
                },
        },
      ],
    };
  }, [granularity, type, xAxisType, appUsageData, appCategoryUsageData]);

  return (
    <ReactECharts
      className={cn(className)}
      theme={isDark ? "dark" : undefined}
      option={option}
      notMerge={true}
    />
  );
};
