import { AppPicker } from "@/entities/app";
import type { Dimension } from "../model/schemas";
import { AppCategoryPicker } from "@/entities/app-category";

export type DimensionMemberPickerProps = {
  className?: string;
  mode: "multiple" | "single";
  dimension: Dimension;
  value: string[];
  onChange: (value: string[]) => void;
  placeholder?: string;
};
export const DimensionMemberPicker = ({
  className,
  mode,
  dimension,
  value,
  onChange,
  placeholder,
}: DimensionMemberPickerProps) => {
  return dimension === "app" ? (
    <AppPicker
      className={className}
      mode={mode}
      value={value}
      onChange={onChange}
      placeholder={placeholder}
    />
  ) : (
    <AppCategoryPicker
      className={className}
      mode={mode}
      value={value}
      onChange={onChange}
      placeholder={placeholder}
    />
  );
};
