import type { Dimension } from "../model/schemas";
import type { Theme } from "@emotion/react";
import type { SxProps } from "@mui/material/styles";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";

type DimensionOption = {
  value: Dimension;
  label: string;
};

export type DateRangeSelectorProps = {
  className?: string;
  sx?: SxProps<Theme>;
  value: Dimension;
  onValueChange: (value: Dimension) => void;
};

export const DimensionTypeSelector = ({
  className,
  sx,
  value,
  onValueChange,
}: DateRangeSelectorProps) => {
  const dimensionOptions: DimensionOption[] = [
    {
      value: "app",
      label: "应用",
    },
    {
      value: "app-category",
      label: "类别",
    },
  ];

  return (
    <ToggleButtonGroup
      size="small"
      exclusive
      sx={sx}
      className={className}
      value={value}
      onChange={(_, newDimension: Dimension | null) => {
        if (newDimension !== null) {
          onValueChange(newDimension);
        }
      }}
    >
      {dimensionOptions.map((item) => (
        <ToggleButton key={item.value} value={item.value}>
          {item.label}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
};
