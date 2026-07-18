import { useQuery } from "@tanstack/react-query";
import { dateOnlyToDate, type DateOnly } from "@/shared/lib/date-only";
import { type CustomSeriesRenderItem, graphic } from "echarts";
import { useMemo } from "react";
import {
  appCategoryUsageTimelineQueryOptions,
  appUsageTimelineQueryOptions,
} from "../api/queries";
import ReactECharts from "echarts-for-react";
import { userSettingsQueries } from "@/entities/user-settings/api/queries";
import type {
  AppCategoryUsageTimelineItemDto,
  AppUsageTimelineItemDto,
} from "../api/schemas";
import type { Theme } from "@emotion/react";
import { useTheme, type SxProps } from "@mui/material/styles";
import dayjs from "@/shared/lib/dayjs";
import Box from "@mui/material/Box";

export type UsageTimelineProps = {
  className?: string;
  sx?: SxProps<Theme>;
  type: "app" | "app-category";
  date: DateOnly;
  includedIds?: string[];
  excludedIds?: string[];
};

const renderItem: CustomSeriesRenderItem = (params, api) => {
  const categoryIndex = api.value(2);
  const start = api.coord([api.value(0), categoryIndex]);
  const end = api.coord([api.value(1), categoryIndex]);

  const size = api.size?.([0, 1]) ?? [0, 20];
  const height = ((Array.isArray(size) ? size[1] : size) ?? 0) * 1;

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const coordSys = params.coordSys as any;

  const rectShape = graphic.clipRectByRect(
    {
      x: start[0],
      y: start[1] - height / 2,
      width: end[0] - start[0],
      height: height,
    },
    {
      x: coordSys.x,
      y: coordSys.y,
      width: coordSys.width,
      height: coordSys.height,
    },
  );

  return (
    rectShape && {
      type: "rect",
      transition: ["shape", "style"],
      shape: rectShape,
      style: {
        fill: api.visual("color"),
      },
      focus: "series",
    }
  );
};

export const UsageTimeline = ({
  className,
  sx,
  date,
  type,
  includedIds,
  excludedIds,
}: UsageTimelineProps) => {
  const theme = useTheme();
  const isDark = theme.palette.mode === "dark";

  const { data: userSettingsDtoData } = useQuery(
    userSettingsQueries.userSettings(),
  );
  const dayCutoffHour = userSettingsDtoData?.dayCutoffHour ?? 0;
  const { data: appUsageTimelineData } = useQuery({
    ...appUsageTimelineQueryOptions({
      startDate: date,
      endDate: date,
      includedIds: includedIds,
      excludedIds: excludedIds,
    }),
    enabled: type === "app",
  });
  const { data: appCategoryUsageTimelineData } = useQuery({
    ...appCategoryUsageTimelineQueryOptions({
      startDate: date,
      endDate: date,
      includedIds: includedIds,
      excludedIds: excludedIds,
    }),
    enabled: type === "app-category",
  });

  const timelineData =
    type === "app" ? appUsageTimelineData : appCategoryUsageTimelineData;

  const option = useMemo(() => {
    const dayStart = dayjs(dateOnlyToDate(date))
      .add(dayCutoffHour, "hour")
      .toDate();
    const dayStartMs = dayStart.getTime();
    const dayEndMs = dayStartMs + 24 * 3600 * 1000;

    const groupedData = (timelineData || []).reduce(
      (acc, item) => {
        if (!acc[item.name]) acc[item.name] = [];
        acc[item.name].push(item);
        return acc;
      },
      {} as Record<
        string,
        AppUsageTimelineItemDto[] | AppCategoryUsageTimelineItemDto[]
      >,
    );

    const series = Object.entries(groupedData).map(([name, items]) => {
      return {
        name: name,
        type: "custom",
        renderItem,
        itemStyle: {
          color: items[0]?.color,
        },
        encode: {
          x: [0, 1],
          y: 2,
        },
        data: items.map((item) => {
          const startMs = item.startTime.getTime();
          const endMs = item.endTime.getTime();
          return {
            name: item.name,
            value: [startMs, endMs, 0, item.id],
            itemStyle: {
              color: item.color,
            },
          };
        }),
      };
    });

    return {
      backgroundColor: "transparent",
      grid: {
        top: 35,
        bottom: 60,
        left: 40,
        right: 40,
      },
      legend: {
        type: "scroll",
        top: 0,
      },
      dataZoom: [
        {
          type: "inside",
          xAxisIndex: 0,
          filterMode: "weakFilter",
        },
        {
          type: "slider",
          xAxisIndex: 0,
          filterMode: "weakFilter",
          height: 20,
          bottom: 10,
          labelFormatter: (value: Date) => {
            return dayjs(value).format("HH:mm");
          },
        },
      ],
      yAxis: {
        type: "category",
        data: ["Timeline"],
        show: false,
      },
      xAxis: {
        type: "time",
        min: dayStartMs,
        max: dayEndMs,
        axisLabel: {
          formatter: "{HH}:{mm}",
          showMinLabel: true,
          showMaxLabel: true,
        },
        splitLine: {
          show: true,
          lineStyle: { type: "dashed" },
        },
      },
      tooltip: {
        trigger: "item",
        position: "top",
        borderWidth: 0,
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        formatter: (params: any) => {
          const { value, seriesName, color } = params;
          const startStr = dayjs(new Date(value[0])).format("HH:mm:ss");
          const endStr = dayjs(new Date(value[1])).format("HH:mm:ss");

          return `
            <div>
              <span style="display:inline-block;margin-right:4px;border-radius:10px;width:10px;height:10px;background-color:${color};"></span>
              <b>${seriesName}</b><br/>
              <span style="color:#888;">${startStr} ~ ${endStr}</span>
            </div>
          `;
        },
      },
      series: series,
    };
  }, [timelineData, date, dayCutoffHour]);

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
      />
    </Box>
  );
};
