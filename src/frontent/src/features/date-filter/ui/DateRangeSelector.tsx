import {
  Button,
  Calendar,
  Popover,
  PopoverTrigger,
  PopoverContent,
  cn,
} from "@/shared/lib/shadcn";
import { CalendarIcon, ArrowLeftIcon, ArrowRightIcon } from "lucide-react";
import type { DateRange, TimeFrame } from "../model/schemas";
import type { DateRange as DayPickerRange } from "react-day-picker";
import { useMemo } from "react";
import {
  addDays,
  addMonths,
  addWeeks,
  endOfISOWeek,
  endOfMonth,
  isAfter,
  isSameDay,
  startOfISOWeek,
  startOfMonth,
  subDays,
  subMonths,
  subWeeks,
} from "date-fns";

export type DateRangeSelectorProps = {
  className?: string;
  timeFrame: TimeFrame;
  value: DateRange;
  onChange: (value: DateRange) => void;
};
export const DateRangeSelector = ({
  className,
  timeFrame,
  value,
  onChange,
}: DateRangeSelectorProps) => {
  const shiftDateRange = (direction: "forward" | "backward") => {
    const today = new Date();
    const isBackward = direction === "backward";
    const isEndToday = isSameDay(value.end, today);

    let newStartDate: Date;
    let newEndDate: Date;

    // 日视图逻辑
    switch (timeFrame) {
      case "day": {
        newEndDate = isBackward ? subDays(value.end, 1) : addDays(value.end, 1);
        // 如果新计算的结束日期 > 今天，强制回到今天
        if (isAfter(newEndDate, today)) newEndDate = today;
        newStartDate = newEndDate;
        break;
      }
      // 周视图逻辑
      case "week": {
        if (isBackward) {
          // 向过去移动
          if (isEndToday) {
            // 从“近 7 天”向以前跳到“上一个完整自然周”
            const lastWeek = subWeeks(today, 1);
            newStartDate = startOfISOWeek(lastWeek);
            newEndDate = endOfISOWeek(lastWeek);
          } else {
            // 普通向以前移动 7 天
            newStartDate = subWeeks(value.start, 1);
            newEndDate = subWeeks(value.end, 1);
          }
        } else {
          // 向未来移动
          newStartDate = addWeeks(value.end, 1);
          newEndDate = addWeeks(value.end, 1);

          // 如果新计算的结束日期 >= 今天，回归“近 7 天”
          if (newEndDate >= today || isSameDay(newEndDate, today)) {
            newEndDate = today;
            newStartDate = subDays(today, 6);
          }
        }
        break;
      }
      // 月视图逻辑
      case "month": {
        if (isBackward) {
          // 向过去移动
          if (isEndToday) {
            // 从“近 31 天”向以前跳到“上个完整自然月”
            const lastMonth = subMonths(today, 1);
            newStartDate = startOfMonth(lastMonth);
            newEndDate = endOfMonth(lastMonth);
          } else {
            // 普通向以前移动 31 天
            const prevMonthBase = subMonths(value.end, 1);
            newStartDate = startOfMonth(prevMonthBase);
            newEndDate = endOfMonth(prevMonthBase);
          }
        } else {
          // 向未来移动
          const nextMonthBase = addMonths(value.end, 1);
          newStartDate = startOfMonth(nextMonthBase);
          newEndDate = endOfMonth(nextMonthBase);

          // 如果新月度的结束日期 >= 今天，回归“近 31 天”
          if (newEndDate >= today || isSameDay(newEndDate, today)) {
            newEndDate = today;
            newStartDate = subDays(today, 30);
          }
        }
        break;
      }
      default:
        return;
    }

    onChange({ start: newStartDate, end: newEndDate });
  };

  const dateLabel = useMemo(() => {
    const today = new Date();
    const isEndToday = isSameDay(value.end, today);

    switch (timeFrame) {
      case "day": {
        if (isEndToday) return "今天";
        if (isSameDay(value.end, subDays(today, 1))) return "昨天";
        return value.end.toLocaleDateString();
      }
      case "week": {
        if (isEndToday) return "近 7 天";
        const lastWeekEnd = endOfISOWeek(subWeeks(today, 1));
        if (isSameDay(value.end, lastWeekEnd)) return "上周";
        return `${value.start.toLocaleDateString()}~${value.end.toLocaleDateString()}`;
      }
      case "month": {
        if (isEndToday) return "近 31 天";
        const lastMonthEnd = endOfMonth(subMonths(today, 1));
        if (isSameDay(value.end, lastMonthEnd)) return "上个月";
        return `${value.start.toLocaleDateString()}~${value.end.toLocaleDateString()}`;
      }
      default:
        return "";
    }
  }, [timeFrame, value]);

  return (
    <div className={cn(className, "flex justify-center")}>
      {/* 预定义日期范围选择 */}
      <div
        className={cn(
          "border-input flex min-w-65 rounded-lg border",
          timeFrame === "custom" ? "hidden" : "",
        )}
      >
        <Button variant="ghost" onClick={() => shiftDateRange("backward")}>
          <ArrowLeftIcon />
        </Button>
        <div className="flex flex-1 items-center justify-center">
          {dateLabel}
        </div>
        <Button variant="ghost" onClick={() => shiftDateRange("forward")}>
          <ArrowRightIcon />
        </Button>
      </div>
      {/* 自定义日期范围选择 */}
      <div
        className={cn(
          "border-input rounded-lg border",
          timeFrame === "custom" ? "" : "hidden",
        )}
      >
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="ghost"
              id="date-picker-range"
              className="justify-start px-2.5 font-normal"
            >
              <CalendarIcon />
              {`${value.start.toLocaleDateString()} ~ ${value.end.toLocaleDateString()}`}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start">
            <Calendar
              mode="range"
              defaultMonth={value.start}
              selected={{
                from: value.start,
                to: value.end,
              }}
              onSelect={(range: DayPickerRange | undefined) =>
                range?.from && range?.to
                  ? onChange({ start: range.from, end: range.to })
                  : undefined
              }
              numberOfMonths={2}
            />
          </PopoverContent>
        </Popover>
      </div>
    </div>
  );
};
