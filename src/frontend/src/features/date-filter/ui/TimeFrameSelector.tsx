import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import type { TimeFrame } from "../model/schemas";
import type { Theme } from "@emotion/react";
import type { SxProps } from "@mui/material/styles";
import ToggleButton from "@mui/material/ToggleButton";
import { useTranslation } from "react-i18next";

type TimeFrameOption = { value: TimeFrame; labelKey: string };

export type TimeFrameSelectorProps = {
  sx?: SxProps<Theme>;
  className?: string;
  value: TimeFrame;
  onValueChange: (value: TimeFrame) => void;
};

export const TimeFrameSelector = ({
  sx,
  className,
  value,
  onValueChange,
}: TimeFrameSelectorProps) => {
  const { t } = useTranslation(["shared"]);
  const options: TimeFrameOption[] = [
    {
      value: "day",
      labelKey: "timeFrame.day",
    },
    {
      value: "week",
      labelKey: "timeFrame.week",
    },
    {
      value: "month",
      labelKey: "timeFrame.month",
    },
    {
      value: "custom",
      labelKey: "timeFrame.custom",
    },
  ];

  return (
    <ToggleButtonGroup
      size="small"
      exclusive
      sx={sx}
      className={className}
      value={value}
      onChange={(_, newTimeFrame: TimeFrame | null) => {
        if (newTimeFrame !== null) {
          onValueChange(newTimeFrame);
        }
      }}
    >
      {options.map((item) => (
        <ToggleButton key={item.value} value={item.value}>
          {t(item.labelKey)}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
};
