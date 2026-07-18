import { AppPicker } from "@/entities/app";
import { AppCategoryPicker } from "@/entities/app-category";
import type { Dimension } from "../model/schemas";
import type { Theme } from "@emotion/react";
import type { SxProps } from "@mui/material/styles";

export type DimensionMemberPickerProps = {
  className?: string;
  sx?: SxProps<Theme>;
  dimension: Dimension;
  placeholder?: string;
} & (
  | {
      mode: "single";
      value: string | null;
      onValueChange: (value: string | null) => void;
    }
  | {
      mode: "multiple";
      value: string[];
      onValueChange: (value: string[]) => void;
    }
);

export const DimensionMemberPicker = (props: DimensionMemberPickerProps) => {
  const { className, sx, dimension, placeholder, ...pickerProps } = props;

  return dimension === "app" ? (
    <AppPicker
      className={className}
      sx={sx}
      placeholder={placeholder}
      {...pickerProps}
    />
  ) : (
    <AppCategoryPicker
      className={className}
      sx={sx}
      placeholder={placeholder}
      {...pickerProps}
    />
  );
};
