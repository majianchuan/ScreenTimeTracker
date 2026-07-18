import { useQuery } from "@tanstack/react-query";
import type { DateOnly } from "@/shared/lib/date-only";
import { AppIcon } from "@/entities/app";
import { formatSecondsDuration } from "@/shared/lib/time";
import { AppCategoryIcon } from "@/entities/app-category";
import Box from "@mui/material/Box";
import LinearProgress from "@mui/material/LinearProgress";
import {
  appCategoryUsageDistributionQueryOptions,
  appUsageDistributionQueryOptions,
} from "../api/queries";
import type { Theme } from "@emotion/react";
import { type SxProps } from "@mui/material/styles";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import List from "@mui/material/List";
import Typography from "@mui/material/Typography";

export type UsageDistributionListProps = {
  className?: string;
  sx?: SxProps<Theme>;
  type: "app" | "app-category";
  startDate: DateOnly;
  endDate: DateOnly;
  topN: number;
  excludedIds?: string[];
  onItemClick?: (id: string) => void;
};

export const UsageDistributionList = ({
  className,
  sx,
  type,
  startDate,
  endDate,
  topN,
  excludedIds,
  onItemClick,
}: UsageDistributionListProps) => {
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

  return (
    <List className={className} sx={[{}, ...(Array.isArray(sx) ? sx : [sx])]}>
      {usageDistributiondata?.items.map((item) => {
        const percentage =
          (item.durationSeconds / usageDistributiondata.totalDurationSeconds) *
          100;
        return (
          <ListItem disablePadding key={`${type}-${item.id}`}>
            <ListItemButton
              onClick={() => {
                onItemClick?.(item.id);
              }}
            >
              <ListItemIcon>
                {type === "app" ? (
                  <AppIcon
                    sx={{
                      width: "2rem",
                      height: "2rem",
                    }}
                    id={item.id}
                    iconPath={item.iconPath}
                    iconLastUpdatedAt={item.iconLastUpdatedAt}
                  />
                ) : (
                  <AppCategoryIcon
                    sx={{
                      width: "2rem",
                      height: "2rem",
                    }}
                    id={item.id}
                    iconPath={item.iconPath}
                    iconLastUpdatedAt={item.iconLastUpdatedAt}
                  />
                )}
              </ListItemIcon>
              <Box sx={{ flex: 1, ml: 0.5 }}>
                <Box sx={{ display: "flex", justifyContent: "space-between" }}>
                  <Typography>{item.name}</Typography>
                  <Typography>
                    {formatSecondsDuration(item.durationSeconds)}
                  </Typography>
                </Box>
                <Box sx={{ display: "flex", alignItems: "center" }}>
                  <Box sx={{ width: "100%", mr: 1 }}>
                    <LinearProgress variant="determinate" value={percentage} />
                  </Box>
                  <Box
                    sx={{
                      minWidth: "4ch",
                      textAlign: "right",
                    }}
                  >
                    <Typography
                      variant="body2"
                      sx={{ color: "text.secondary" }}
                    >
                      {`${Math.round(percentage)}%`}
                    </Typography>
                  </Box>
                </Box>
              </Box>
            </ListItemButton>
          </ListItem>
        );
      })}
    </List>
  );
};
