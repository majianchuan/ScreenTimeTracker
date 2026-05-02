import { useQuery } from "@tanstack/react-query";
import { dateOnlyToDate, type DateOnly } from "@/shared/lib/date-only";
import { cn } from "@/shared/lib/shadcn";
import { addHours, format } from "date-fns";
import { type CustomSeriesRenderItem, graphic } from "echarts";
import { useTheme } from "next-themes";
import { useMemo } from "react";
import { appCategoryUsageTimeline, appUsageTimeline } from "../api/queries";
import ReactECharts from "echarts-for-react";
import { screenTimeUserSettingsQueries } from "@/entities/screen-time-user-settings/api/queries";
import type {
  AppCategoryUsageTimelineItemDto,
  AppUsageTimelineItemDto,
} from "../api/schemas";

export type UsageTimelineProps = {
  className?: string;
  type: "app" | "app-category";
  date: DateOnly;
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
  date,
  type,
  excludedIds,
}: UsageTimelineProps) => {
  const { theme, systemTheme } = useTheme();
  const isDark = (theme === "system" ? systemTheme : theme) === "dark";

  const { data: screenTimeUserSettingsDtoData } = useQuery(
    screenTimeUserSettingsQueries.screenTimeUserSettings(),
  );
  const dayCutoffHour = screenTimeUserSettingsDtoData?.dayCutoffHour ?? 0;
  const { data: appUsageTimelineData } = useQuery({
    ...appUsageTimeline({
      startDate: date,
      endDate: date,
      excludedIds: excludedIds,
    }),
    enabled: type === "app",
  });
  const { data: appCategoryUsageTimelineData } = useQuery({
    ...appCategoryUsageTimeline({
      startDate: date,
      endDate: date,
      excludedIds: excludedIds,
    }),
    enabled: type === "app-category",
  });

  const timelineData =
    type === "app" ? appUsageTimelineData : appCategoryUsageTimelineData;

  const option = useMemo(() => {
    const dayStart = addHours(dateOnlyToDate(date), dayCutoffHour);
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

    const series = Object.entries(groupedData).map(([name, items]) => ({
      name: name,
      type: "custom",
      renderItem,
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
        };
      }),
    }));

    return {
      backgroundColor: "transparent",
      grid: {
        top: 35,
        bottom: 60,
        left: "5%",
        right: "5%",
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
            return format(value, "HH:mm");
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
        },
        splitLine: {
          show: true,
          lineStyle: { type: "dashed", opacity: 0.2 },
        },
      },
      tooltip: {
        trigger: "item",
        position: "top",
        borderWidth: 0,
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
        formatter: (params: any) => {
          const { value, seriesName, color } = params;
          const startStr = format(new Date(value[0]), "HH:mm:ss");
          const endStr = format(new Date(value[1]), "HH:mm:ss");

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
    <ReactECharts
      className={cn(className)}
      theme={isDark ? "dark" : undefined}
      option={option}
      notMerge={true}
    />
  );
};
