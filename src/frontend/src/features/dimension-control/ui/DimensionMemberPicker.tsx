import { AppPicker } from "@/entities/app";
import type { Dimension } from "../model/schemas";
import { AppCategoryPicker } from "@/entities/app-category";

export type DimensionMemberPickerProps = {
  className?: string;
  mode: "multiple" | "single";
  dimension: Dimension;
  value: string[];
  onValueChange: (value: string[]) => void;
  placeholder?: string;
};
export const DimensionMemberPicker = ({
  className,
  mode,
  dimension,
  value,
  onValueChange,
  placeholder,
}: DimensionMemberPickerProps) => {
  return dimension === "app" ? (
    <AppPicker
      className={className}
      mode={mode}
      value={value}
      onValueChange={onValueChange}
      placeholder={placeholder}
    />
  ) : (
    <AppCategoryPicker
      className={className}
      mode={mode}
      value={value}
      onValueChange={onValueChange}
      placeholder={placeholder}
    />
  );
};
