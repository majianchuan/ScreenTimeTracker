import { useQuery } from "@tanstack/react-query";
import { appCategoryUsageRanking, appUsageRanking } from "../api/queries";
import type { DateOnly } from "@/shared/lib/date-only";
import { Button, cn, Progress } from "@/shared/lib/shadcn";
import { AppIcon } from "@/entities/app";
import { formatSecondsDuration } from "@/shared/lib/time";
import { AppCategoryIcon } from "@/entities/app-category";

export type UsageRankingProps = {
  className?: string;
  type: "app" | "app-category";
  startDate: DateOnly;
  endDate: DateOnly;
  topN: number;
  excludedIds?: string[];
  onItemClick?: (id: string) => void;
};

export const UsageRanking = ({
  className,
  type,
  startDate,
  endDate,
  topN,
  excludedIds,
  onItemClick,
}: UsageRankingProps) => {
  const { data: appUsageRankingData } = useQuery({
    ...appUsageRanking({
      startDate: startDate,
      endDate: endDate,
      topN: topN,
      excludedIds: excludedIds,
    }),
    enabled: type === "app",
  });

  const { data: appCategoryUsageRankingData } = useQuery({
    ...appCategoryUsageRanking({
      startDate: startDate,
      endDate: endDate,
      topN: topN,
      excludedIds: excludedIds,
    }),
    enabled: type === "app-category",
  });

  return type === "app" ? (
    <div className={cn(className, "flex flex-col gap-1")}>
      {appUsageRankingData?.map((item) => (
        <Button
          variant="ghost"
          className="flex h-auto w-full flex-row px-2.5 py-1"
          key={item.id}
          onClick={() => onItemClick?.(item.id)}
        >
          <AppIcon className="size-8" id={item.id} iconPath={item.iconPath} />
          <div className="flex flex-1 flex-col">
            <div className="flex w-full flex-1 flex-row items-center justify-between">
              <div className="text-base">{item.name}</div>
              <div>{formatSecondsDuration(item.durationSeconds)}</div>
            </div>
            <div className="flex w-full flex-row items-center gap-2">
              <Progress value={item.percentage} className="flex-1" />
              <div>{item.percentage}%</div>
            </div>
          </div>
        </Button>
      ))}
    </div>
  ) : (
    <div className={cn(className, "flex flex-col gap-1")}>
      {appCategoryUsageRankingData?.map((item) => (
        <Button
          variant="ghost"
          className="flex h-auto w-full flex-row px-2.5 py-1"
          key={item.id}
          onClick={() => onItemClick?.(item.id)}
        >
          <AppCategoryIcon
            className="size-8"
            id={item.id}
            iconPath={item.iconPath}
          />
          <div className="flex flex-1 flex-col">
            <div className="flex w-full flex-1 flex-row items-center justify-between">
              <div className="text-base">{item.name}</div>
              <div>{formatSecondsDuration(item.durationSeconds)}</div>
            </div>
            <div className="flex w-full flex-row items-center gap-2">
              <Progress value={item.percentage} className="flex-1" />
              <div>{item.percentage}%</div>
            </div>
          </div>
        </Button>
      ))}
    </div>
  );
};
