import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import type { TimeFrame } from "../model/schemas";
import type { Theme } from "@emotion/react";
import type { SxProps } from "@mui/material/styles";
import ToggleButton from "@mui/material/ToggleButton";

type TimeFrameOption = { value: TimeFrame; label: string };

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
  const options: TimeFrameOption[] = [
    {
      value: "day",
      label: "日",
    },
    {
      value: "week",
      label: "周",
    },
    {
      value: "month",
      label: "月",
    },
    {
      value: "custom",
      label: "自定义",
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
          {item.label}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
};
