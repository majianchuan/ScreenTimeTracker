import { useQuery } from "@tanstack/react-query";
import ReactECharts from "echarts-for-react";
import {
  appCategoryUsageDistributionQueryOptions,
  appUsageDistributionQueryOptions,
} from "../api/queries";
import type { DateOnly } from "@/shared/lib/date-only";
import { formatSecondsDuration } from "@/shared/lib/time";
import { useMemo } from "react";
import { getAppCategoryIconUrl } from "@/entities/app-category";
import { getAppIconUrl } from "@/entities/app";
import UnknownApp from "@/shared/ui/UnknownApp.svg";
import UnknownAppCategory from "@/shared/ui/UnknownAppCategory.svg";
import type { Theme } from "@emotion/react";
import { useTheme, type SxProps } from "@mui/material/styles";
import Box from "@mui/material/Box";

export type UsageDistributionPieChartProps = {
  className?: string;
  sx?: SxProps<Theme>;
  type: "app" | "app-category";
  startDate: DateOnly;
  endDate: DateOnly;
  topN: number;
  excludedIds?: string[];
  onItemClick?: (id: string) => void;
};

export const UsageDistributionPieChart = ({
  className,
  sx,
  type,
  startDate,
  endDate,
  topN,
  excludedIds,
  onItemClick,
}: UsageDistributionPieChartProps) => {
  const theme = useTheme();
  const isDark = theme.palette.mode === "dark";

  const { data: appUsageDistributionData } = useQuery({
    ...appUsageDistributionQueryOptions({
      startDate: startDate,
      endDate: endDate,
      topN: topN,
      excludedIds: excludedIds,
    }),
    enabled: type === "app",
  });
  const { data: appCategoryUsageDistributionData } = useQuery({
    ...appCategoryUsageDistributionQueryOptions({
      startDate: startDate,
      endDate: endDate,
      topN: topN,
      excludedIds: excludedIds,
    }),
    enabled: type === "app-category",
  });
  const usageDistributiondata =
    type === "app"
      ? appUsageDistributionData
      : appCategoryUsageDistributionData;

  const option = useMemo(() => {
    return {
      backgroundColor: "transparent",
      legend: {
        type: "scroll",
        top: 0,
      },
      label: {
        formatter: (params: { data: { id: string | null; name: string } }) => {
          if (params.data.id !== null) {
            return `{${toRichKey(params.data.id)}|} {name|${params.data.name}}`;
          } else return `${params.data.name}`;
        },
        rich: usageDistributiondata?.items.reduce(
          (acc, v) => {
            const iconUrl =
              type === "app"
                ? getAppIconUrl(v.id, v.iconLastUpdatedAt)
                : getAppCategoryIconUrl(v.id, v.iconLastUpdatedAt);
            const fallbackIconUrl =
              type === "app" ? UnknownApp : UnknownAppCategory;
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
          percent: number;
          data: {
            name: string;
            durationSeconds: number;
          };
        }) => {
          const duration = formatSecondsDuration(params.data.durationSeconds);
          return `${params.data.name}<br/>${duration} (${params.percent}%)`;
        },
      },
      series: [
        {
          type: "pie",
          radius: "70%",
          data: [
            ...(usageDistributiondata?.items.map((v) => ({
              name: v.name,
              value: v.durationSeconds,
              id: v.id,
              iconPath: v.iconPath,
              durationSeconds: v.durationSeconds,
              itemStyle: {
                color: v.color,
              },
            })) || []),
            ...(usageDistributiondata &&
            usageDistributiondata.othersDurationSeconds > 0
              ? [
                  {
                    name:
                      type === "app"
                        ? `其他${usageDistributiondata?.othersCount}个应用`
                        : `其他${usageDistributiondata?.othersCount}个类别`,
                    value: usageDistributiondata?.othersDurationSeconds,
                    id: null,
                    itemStyle: {
                      color: isDark ? "#9CA3AF" : "#6B7280",
                    },
                  },
                ]
              : []),
          ],
        },
      ],
    };
  }, [type, usageDistributiondata, isDark]);

  return (
    <Box sx={sx} className={className}>
      <ReactECharts
        style={{
          height: "100%",
          width: "100%",
        }}
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
    </Box>
  );
};

function toRichKey(id: string): string {
  return `icon_${id.replace(/-/g, "_")}`;
}
