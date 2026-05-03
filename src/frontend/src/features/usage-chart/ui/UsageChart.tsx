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

  const { chartData, total, average } = useMemo(() => {
    const usageData = type === "app" ? appUsageData : appCategoryUsageData;
    if (!usageData?.length) {
      return { chartData: [], total: 0, average: 0 };
    }

    // 统一映射为 { name: string, value: number } 格式
    const normalized = usageData.map((item) => {
      let name = "";
      // 判断是小时数据还是日期数据
      if (xAxisType === "hour") {
        name = item.startTime.getHours().toString().padStart(2, "0") + ":00";
      } else if (xAxisType === "day") {
        name = item.startTime.toLocaleDateString();
      } else if (xAxisType === "week") name = format(item.startTime, "EEE");

      const value = item.durationSeconds ?? 0;

      return { name, value };
    });

    // 计算总和
    const sum = normalized.reduce((acc, cur) => acc + cur.value, 0);
    // 计算平均数
    const avg = Math.round(sum / (normalized.length || 1));

    return {
      chartData: normalized,
      total: sum,
      average: avg,
    };
  }, [xAxisType, type, appUsageData, appCategoryUsageData]);

  const option = useMemo(() => {
    return {
      backgroundColor: "transparent",
      title: {
        text: `总计：${formatSecondsDuration(total)} ${granularity !== "hour" ? `| 平均：${formatSecondsDuration(average)}` : ""}`,
      },
      tooltip: {
        trigger: "axis",
        // 格式化 Tooltip 显示具体时长
        formatter: (params: { name: string; value: number }[]) => {
          const item = params[0];
          return `${item.name}<br/>时长：<b>${formatSecondsDuration(item.value)}</b>`;
        },
      },
      xAxis: {
        data: chartData.map((d) => d.name),
      },
      yAxis: {
        axisLabel: {
          formatter: (val: number) => formatSecondsDuration(val),
        },
      },
      series: [
        {
          type: "bar",
          name: "时长",
          data: chartData.map((d) => d.value),
          markLine:
            granularity === "hour"
              ? undefined
              : {
                  animation: false,
                  data: [
                    {
                      yAxis: average,
                      label: {
                        formatter: "平均\n" + formatSecondsDuration(average),
                      },
                    },
                  ],
                },
        },
      ],
    };
  }, [chartData, total, average, granularity]);

  return (
    <ReactECharts
      className={cn(className)}
      theme={isDark ? "dark" : undefined}
      option={option}
      notMerge={true}
    />
  );
};
