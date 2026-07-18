import { useCallback, useEffect, useRef } from "react";
import type { DateRange, TimeFrame } from "./schemas";
import { useQuery } from "@tanstack/react-query";
import { userSettingsQueries } from "@/entities/user-settings";
import dayjs from "@/shared/lib/dayjs";

export type TimeFrameCache = Partial<Record<TimeFrame, DateRange>>;

export type UseDateFilterOptions = {
  currentTimeFrame: TimeFrame;
  currentDateRange: DateRange;
  onValueChange: (timeFrame: TimeFrame, range: DateRange) => void;
};

export const useDateFilter = ({
  currentTimeFrame,
  currentDateRange,
  onValueChange,
}: UseDateFilterOptions) => {
  const timeFrameCacheRef = useRef<TimeFrameCache>({});
  const { data, isLoading } = useQuery({
    ...userSettingsQueries.userSettings(),
  });
  const dayCutoffHour = data?.dayCutoffHour || 5;
  const logicalToday = dayjs()
    .subtract(dayCutoffHour, "hour")
    .startOf("day")
    .toDate();

  const getDefaultDateRange = useCallback(
    (timeFrame: TimeFrame): DateRange => {
      switch (timeFrame) {
        case "day":
          return { start: logicalToday, end: logicalToday };
        case "week":
          return {
            start: dayjs(logicalToday).subtract(6, "day").toDate(),
            end: logicalToday,
          };
        case "month":
          return {
            start: dayjs(logicalToday).subtract(30, "day").toDate(),
            end: logicalToday,
          };
        default:
          return { start: logicalToday, end: logicalToday };
      }
    },
    [logicalToday],
  );

  useEffect(() => {
    timeFrameCacheRef.current[currentTimeFrame] = currentDateRange;
  }, [currentTimeFrame, currentDateRange]);

  const handleTimeFrameChange = (newTimeFrame: TimeFrame) => {
    const nextRange =
      timeFrameCacheRef.current[newTimeFrame] ||
      getDefaultDateRange(newTimeFrame);
    onValueChange(newTimeFrame, nextRange);
  };

  const handleDateRangeChange = (newRange: DateRange) => {
    timeFrameCacheRef.current[currentTimeFrame] = newRange;
    onValueChange(currentTimeFrame, newRange);
  };

  return {
    handleTimeFrameChange,
    handleDateRangeChange,
    isLoading,
  };
};
