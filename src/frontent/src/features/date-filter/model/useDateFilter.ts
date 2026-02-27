import { useEffect, useRef } from "react";
import type { DateRange, TimeFrame } from "./schemas";
import { subDays } from "date-fns";

export type TimeFrameCache = Partial<Record<TimeFrame, DateRange>>;

export type UseDateFilterOptions = {
  currentTimeFrame: TimeFrame;
  currentDateRange: DateRange;
  onChange: (timeFrame: TimeFrame, range: DateRange) => void;
};

export const useDateFilter = ({
  currentTimeFrame,
  currentDateRange,
  onChange,
}: UseDateFilterOptions) => {
  const timeFrameCacheRef = useRef<TimeFrameCache>({});

  const getDefaultDateRange = (timeFrame: TimeFrame): DateRange => {
    const today = new Date();
    switch (timeFrame) {
      case "day":
        return { start: today, end: today };
      case "week":
        return { start: subDays(today, 6), end: today };
      case "month":
        return { start: subDays(today, 30), end: today };
      default: // custom
        return { start: today, end: today };
    }
  };

  useEffect(() => {
    timeFrameCacheRef.current[currentTimeFrame] = currentDateRange;
  }, [currentTimeFrame, currentDateRange]);

  const handleTimeFrameChange = (newTimeFrame: TimeFrame) => {
    const nextRange =
      timeFrameCacheRef.current[newTimeFrame] ||
      getDefaultDateRange(newTimeFrame);
    onChange(newTimeFrame, nextRange);
  };

  const handleDateRangeChange = (newRange: DateRange) => {
    timeFrameCacheRef.current[currentTimeFrame] = newRange;
    onChange(currentTimeFrame, newRange);
  };

  return {
    handleTimeFrameChange,
    handleDateRangeChange,
  };
};
