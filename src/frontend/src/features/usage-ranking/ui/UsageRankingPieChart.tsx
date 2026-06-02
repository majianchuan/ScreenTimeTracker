import { useQuery } from "@tanstack/react-query";
import ReactECharts from "echarts-for-react";
import {
  appCategoryUsageRankingQueryOptions,
  appUsageRankingQueryOptions,
} from "../api/queries";
import type { DateOnly } from "@/shared/lib/date-only";
import { cn } from "@/shared/lib/shadcn";
import { formatSecondsDuration } from "@/shared/lib/time";
import { useTheme } from "next-themes";
import { useMemo } from "react";
import { getAppCategoryIconUrl } from "@/entities/app-category";
import { getAppIconUrl } from "@/entities/app";
import {
  UnknownAppCategoryIconUrl,
  UnknownAppIconUrl,
} from "@/shared/ui/icons";

export type UsageRankingPieChartProps = {
  className?: string;
  type: "app" | "app-category";
  startDate: DateOnly;
  endDate: DateOnly;
  topN: number;
  excludedIds?: string[];
  onItemClick?: (id: string) => void;
};

export const UsageRankingPieChart = ({
  className,
  type,
  startDate,
  endDate,
  topN,
  excludedIds,
  onItemClick,
}: UsageRankingPieChartProps) => {
  const { data: appUsageRankingData } = useQuery({
    ...appUsageRankingQueryOptions({
      startDate: startDate,
      endDate: endDate,
      topN: topN,
      excludedIds: excludedIds,
    }),
    enabled: type === "app",
  });

  const { data: appCategoryUsageRankingData } = useQuery({
    ...appCategoryUsageRankingQueryOptions({
      startDate: startDate,
      endDate: endDate,
      topN: topN,
      excludedIds: excludedIds,
    }),
    enabled: type === "app-category",
  });

  const { theme, systemTheme } = useTheme();
  const isDark = (theme === "system" ? systemTheme : theme) === "dark";

  const toRichKey = (id: string) => `icon_${id.replace(/-/g, "_")}`;

  const option = useMemo(() => {
    const usageRankingdata =
      (type === "app" ? appUsageRankingData : appCategoryUsageRankingData) ||
      [];
    const otherPercentage =
      usageRankingdata.length > 0
        ? 100 - usageRankingdata.reduce((sum, v) => sum + v.percentage, 0)
        : 0;
    return {
      backgroundColor: "transparent",
      legend: {
        type: "scroll",
        top: 0,
      },
      label: {
        formatter: (params: {
          data: { id: string | null; name: string; percentage: number };
        }) => {
          if (params.data.id !== null) {
            return `{${toRichKey(params.data.id)}|} {name|${params.data.name}}`;
          } else return `${params.data.name}`;
        },
        rich: usageRankingdata.reduce(
          (acc, v) => {
            const iconUrl =
              type === "app"
                ? getAppIconUrl(v.id)
                : getAppCategoryIconUrl(v.id);
            const fallbackIconUrl =
              type === "app" ? UnknownAppIconUrl : UnknownAppCategoryIconUrl;
            acc[toRichKey(v.id)] = {
              backgroundColor: {
                image: v.iconPath ? iconUrl : fallbackIconUrl,
              },
              width: 15,
              height: 15,
            };
            return acc;
          },
          {} as Record<
            string,
            {
              backgroundColor: { image: string };
              width: number;
              height: number;
            }
          >,
        ),
      },
      tooltip: {
        trigger: "item",
        formatter: (params: {
          data: {
            name: string;
            percentage: number;
            durationSeconds: number;
          };
        }) => {
          const duration = formatSecondsDuration(params.data.durationSeconds);
          return `${params.data.name}<br/>${duration} (${params.data.percentage}%)`;
        },
      },
      series: [
        {
          type: "pie",
          radius: "70%",
          data: [
            ...usageRankingdata.map((v) => ({
              name: v.name,
              value: v.percentage,
              id: v.id,
              iconPath: v.iconPath,
              durationSeconds: v.durationSeconds,
              percentage: v.percentage,
            })),
            ...(otherPercentage > 0
              ? [
                  {
                    name: type === "app" ? "其他应用" : "其他类别",
                    value: otherPercentage,
                    id: null,
                    percentage: otherPercentage,
                  },
                ]
              : []),
          ],
        },
      ],
    };
  }, [type, appUsageRankingData, appCategoryUsageRankingData]);

  return (
    <ReactECharts
      className={cn(className)}
      theme={isDark ? "dark" : undefined}
      option={option}
      notMerge={true}
      onEvents={{
        click: (params: { data: { id: string | null } }) => {
          if (params.data.id !== null) {
            onItemClick?.(params.data.id);
          }
        },
      }}
    />
  );
};
