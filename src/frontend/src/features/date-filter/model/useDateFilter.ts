import { useCallback, useEffect, useRef } from "react";
import type { DateRange, TimeFrame } from "./schemas";
import { startOfDay, subDays, subHours } from "date-fns";
import { useQuery } from "@tanstack/react-query";
import { userSettingsQueries } from "@/entities/user-settings";

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
  const { data } = useQuery({
    ...userSettingsQueries.userSettings(),
  });
  const dayCutoffHour = data?.dayCutoffHour || 0;
  const logicalToday = startOfDay(subHours(new Date(), dayCutoffHour));

  const getDefaultDateRange = useCallback(
    (timeFrame: TimeFrame): DateRange => {
      switch (timeFrame) {
        case "day":
          return { start: logicalToday, end: logicalToday };
        case "week":
          return { start: subDays(logicalToday, 6), end: logicalToday };
        case "month":
          return { start: subDays(logicalToday, 30), end: logicalToday };
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
  };
};
