import type { Dimension } from "../model/schemas";
import type { Theme } from "@emotion/react";
import type { SxProps } from "@mui/material/styles";
import ToggleButton from "@mui/material/ToggleButton";
import ToggleButtonGroup from "@mui/material/ToggleButtonGroup";
import { useTranslation } from "react-i18next";

type DimensionOption = {
  value: Dimension;
  labelKey: string;
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
  const { t } = useTranslation("feature_dimensionControl");
  const dimensionOptions: DimensionOption[] = [
    {
      value: "app",
      labelKey: "DimensionTypeSelector.options.app",
    },
    {
      value: "app-category",
      labelKey: "DimensionTypeSelector.options.app-category",
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
          {t(item.labelKey)}
        </ToggleButton>
      ))}
    </ToggleButtonGroup>
  );
};
