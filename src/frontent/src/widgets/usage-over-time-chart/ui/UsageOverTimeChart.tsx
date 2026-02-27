import ReactECharts from "echarts-for-react";
import { useMemo } from "react";
import { useQuery } from "@tanstack/react-query";
import {
  usageQueries,
  useUsageConfig,
  type UsageByDayItem,
  type UsageByHourItem,
} from "@/entities/usage";
import { formatSeconds } from "@/shared/lib/time";
import { useTheme } from "next-themes";
import { dateOnlyToDate, type DateOnly } from "@/shared/lib/date-only";
import type { TimeFrame } from "@/features/date-filter";
import type { Dimension } from "@/features/dimension-control";
import { cn } from "@/shared/lib/shadcn";
import { format } from "date-fns";
import { zhCN } from "date-fns/locale";

type UsageOverTimeChartProps = {
  className?: string;
  mode: "summary" | "details";
  timeFrame: TimeFrame;
  dimension: Dimension;
  startDate: DateOnly;
  endDate: DateOnly;
  id?: string;
  excludedIds?: string[];
};

export const UsageOverTimeChart = ({
  className,
  mode,
  timeFrame,
  startDate,
  endDate,
  dimension,
  id,
  excludedIds,
}: UsageOverTimeChartProps) => {
  const { theme, systemTheme } = useTheme();
  const isDark = (theme === "system" ? systemTheme : theme) === "dark";
  const isSummary = mode === "summary";
  const isDetails = mode === "details";
  const isByHour = timeFrame === "day";
  const isByDay = timeFrame !== "day";
  const isSummaryByHour = isSummary && isByHour;
  const isSummaryByDay = isSummary && isByDay;
  const isDetailsByHour = isDetails && isByHour;
  const isDetailsByDay = isDetails && isByDay;
  const { refetchIntervalSeconds } = useUsageConfig();

  const { data: summaryByHourData } = useQuery({
    ...usageQueries.summaryByHour({
      dimension: dimension,
      date: endDate,
      excludedIds: excludedIds,
    }),
    enabled: isSummaryByHour,
    refetchInterval: refetchIntervalSeconds * 1000,
  });
  const { data: summaryByDayData } = useQuery({
    ...usageQueries.summaryByDay({
      dimension: dimension,
      startDate: startDate,
      endDate: endDate,
      excludedIds: excludedIds,
    }),
    enabled: isSummaryByDay,
    refetchInterval: refetchIntervalSeconds * 1000,
  });
  const { data: detailsByHourData } = useQuery({
    ...usageQueries.detailsByHour({
      dimension: dimension,
      date: endDate,
      id: id!,
    }),
    enabled: isDetailsByHour && !!id?.length,
    refetchInterval: refetchIntervalSeconds * 1000,
  });
  const { data: detailsByDayData } = useQuery({
    ...usageQueries.detailsByDay({
      dimension: dimension,
      startDate: startDate,
      endDate: endDate,
      id: id!,
    }),
    enabled: isDetailsByDay && !!id?.length,
    refetchInterval: refetchIntervalSeconds * 1000,
  });

  const { chartData, total, average } = useMemo(() => {
    let rawData: (UsageByDayItem | UsageByHourItem)[] = [];

    if (isSummaryByHour) {
      rawData = summaryByHourData ?? [];
    } else if (isSummaryByDay) {
      rawData = summaryByDayData ?? [];
    } else if (isDetailsByHour) {
      rawData = detailsByHourData ?? [];
    } else if (isDetailsByDay) {
      rawData = detailsByDayData ?? [];
    }

    if (!rawData.length) {
      return { chartData: [], total: 0, average: 0 };
    }

    // 统一映射为 { name: string, value: number } 格式
    const normalized = rawData.map((item: UsageByDayItem | UsageByHourItem) => {
      let name = "";
      // 判断是小时数据还是日期数据
      if ("hour" in item) {
        name = item.hour.toString().padStart(2, "0") + ":00";
      } else if ("date" in item) {
        if (timeFrame === "week") {
          name = format(dateOnlyToDate(item.date), "EEE", {
            locale: zhCN,
          });
        } else {
          name = dateOnlyToDate(item.date).toLocaleDateString();
        }
      }

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
  }, [
    summaryByHourData,
    summaryByDayData,
    detailsByHourData,
    detailsByDayData,
    isSummaryByHour,
    isSummaryByDay,
    isDetailsByHour,
    isDetailsByDay,
    timeFrame,
  ]);

  const option = useMemo(() => {
    return {
      backgroundColor: "transparent",
      title: {
        text: `总计：${formatSeconds(total)} ${isByHour ? "" : ` | 平均：${formatSeconds(average)}`}`,
      },
      tooltip: {
        trigger: "axis",
        // 格式化 Tooltip 显示具体时长
        formatter: (params: { name: string; value: number }[]) => {
          const item = params[0];
          return `${item.name}<br/>时长：<b>${formatSeconds(item.value)}</b>`;
        },
      },
      xAxis: {
        data: chartData.map((d) => d.name),
      },
      yAxis: {
        axisLabel: {
          formatter: (val: number) => formatSeconds(val),
        },
      },
      series: [
        {
          type: "bar",
          name: "时长",
          data: chartData.map((d) => d.value),
          markLine: isByHour
            ? undefined
            : {
                data: [
                  {
                    yAxis: average,
                    label: {
                      formatter: "平均\n" + formatSeconds(average),
                    },
                  },
                ],
              },
        },
      ],
    };
  }, [chartData, total, average, isByHour]);

  return (
    <ReactECharts
      className={cn(className)}
      theme={isDark ? "dark" : undefined}
      option={option}
      notMerge={true}
    />
  );
};
