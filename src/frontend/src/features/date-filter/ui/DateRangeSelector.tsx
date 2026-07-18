import type { DateRange, TimeFrame } from "../model/schemas";
import { useQuery } from "@tanstack/react-query";
import { userSettingsQueries } from "@/entities/user-settings";
import Box from "@mui/material/Box";
import type { Theme } from "@emotion/react";
import type { SxProps } from "@mui/material/styles";
import Button from "@mui/material/Button";
import ChevronLeftIcon from "@mui/icons-material/ChevronLeft";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import dayjs from "@/shared/lib/dayjs";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";

export type DateRangeSelectorProps = {
  className?: string;
  sx?: SxProps<Theme>;
  timeFrame: TimeFrame;
  value: DateRange;
  onValueChange: (value: DateRange) => void;
};

export const DateRangeSelector = ({
  className,
  sx,
  timeFrame,
  value,
  onValueChange,
}: DateRangeSelectorProps) => {
  const { data: userSettingsDtoData } = useQuery(
    userSettingsQueries.userSettings(),
  );
  if (!userSettingsDtoData) return null;

  const dayCutoffHour = userSettingsDtoData.dayCutoffHour;
  const logicalToday = dayjs().subtract(dayCutoffHour, "hour").startOf("day");
  const isEndToday = dayjs(value.end).isSame(logicalToday, "day");
  const dateLabel = (() => {
    switch (timeFrame) {
      case "day": {
        if (isEndToday) return "今天";
        if (dayjs(value.end).isSame(logicalToday.subtract(1, "day"), "day"))
          return "昨天";
        return new Date(value.end).toLocaleDateString();
      }
      case "week": {
        if (isEndToday) return "近 7 天";
        const lastWeekEnd = logicalToday.subtract(1, "week").endOf("isoWeek");
        if (dayjs(value.end).isSame(lastWeekEnd, "day")) return "上周";
        return `${new Date(value.start).toLocaleDateString()}~${new Date(value.end).toLocaleDateString()}`;
      }
      case "month": {
        if (isEndToday) return "近 31 天";
        const lastMonthEnd = logicalToday.subtract(1, "month").endOf("month");
        if (dayjs(value.end).isSame(lastMonthEnd, "day")) return "上个月";
        return `${new Date(value.start).toLocaleDateString()}~${new Date(value.end).toLocaleDateString()}`;
      }
      default:
        return "";
    }
  })();

  const shiftDateRange = (direction: "forward" | "backward") => {
    const isForward = direction === "forward";
    const isEndToday = dayjs(value.end).isSame(logicalToday, "day");

    let newStartDate: Date;
    let newEndDate: Date;

    switch (timeFrame) {
      case "day": {
        if (isForward) {
          let newEnd = dayjs(value.end).add(1, "day");
          if (newEnd.isSameOrAfter(logicalToday, "day")) newEnd = logicalToday;
          newEndDate = newEnd.toDate();
          newStartDate = newEndDate;
        } else {
          const newEnd = dayjs(value.end).subtract(1, "day");
          newEndDate = newEnd.toDate();
          newStartDate = newEndDate;
        }
        break;
      }
      case "week": {
        if (isForward) {
          let newEnd = logicalToday.add(1, "week");
          if (newEnd.isSameOrAfter(logicalToday, "day")) newEnd = logicalToday;
          newEndDate = newEnd.toDate();
          newStartDate = newEnd.subtract(6, "day").toDate();
        } else {
          if (isEndToday) {
            const lastWeek = logicalToday.subtract(1, "week");
            newStartDate = lastWeek.startOf("isoWeek").toDate();
            newEndDate = lastWeek.endOf("isoWeek").toDate();
          } else {
            const newEnd = logicalToday.subtract(1, "week");
            newEndDate = newEnd.toDate();
            newStartDate = newEnd.subtract(6, "day").toDate();
          }
        }
        break;
      }
      case "month": {
        if (isForward) {
          const newEnd = dayjs(value.end).add(1, "month");
          if (newEnd.isSameOrAfter(logicalToday, "day")) {
            newEndDate = logicalToday.toDate();
            newStartDate = logicalToday.subtract(30, "day").toDate();
          } else {
            newEndDate = newEnd.toDate();
            newStartDate = newEnd.startOf("month").toDate();
          }
        } else {
          if (isEndToday) {
            const lastMonth = logicalToday.subtract(1, "month");
            newStartDate = lastMonth.startOf("month").toDate();
            newEndDate = lastMonth.endOf("month").toDate();
          } else {
            const prevMonthBase = dayjs(value.end).subtract(1, "month");
            newStartDate = prevMonthBase.startOf("month").toDate();
            newEndDate = prevMonthBase.endOf("month").toDate();
          }
        }
        break;
      }
      default:
        return;
    }

    onValueChange({ start: newStartDate, end: newEndDate });
  };

  return (
    <>
      {/* 预定义日期范围选择 */}
      <Box
        sx={[
          {
            display: timeFrame === "custom" ? "none" : "flex",
            alignItems: "center",
            border: 1,
            borderColor: "divider",
            borderRadius: 1,
          },
          ...(Array.isArray(sx) ? sx : [sx]),
        ]}
        className={className}
      >
        <Button
          // size="small"
          variant="text"
          onClick={() => shiftDateRange("backward")}
        >
          <ChevronLeftIcon />
        </Button>
        <Box
          sx={{
            flex: 1,
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          {dateLabel}
        </Box>
        <Button
          // size="small"
          variant="text"
          onClick={() => shiftDateRange("forward")}
          disabled={isEndToday}
        >
          <ChevronRightIcon />
        </Button>
      </Box>

      {/* 自定义日期范围选择 */}
      <Box
        sx={[
          {
            display: timeFrame === "custom" ? "flex" : "none",
          },
          ...(Array.isArray(sx) ? sx : [sx]),
        ]}
      >
        <DatePicker
          value={dayjs(value.start)}
          sx={{ flex: "1" }}
          onChange={(newdate) =>
            newdate !== null &&
            onValueChange({ start: newdate.toDate(), end: value.end })
          }
          slotProps={{
            textField: {
              size: "small",
            },
          }}
        />
        <DatePicker
          value={dayjs(value.end)}
          sx={{ flex: "1" }}
          onChange={(newdate) =>
            newdate !== null &&
            onValueChange({ start: value.start, end: newdate.toDate() })
          }
          slotProps={{
            textField: {
              size: "small",
            },
          }}
        />
      </Box>
    </>
  );
};
