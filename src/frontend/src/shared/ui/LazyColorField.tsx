import TextField, { type TextFieldProps } from "@mui/material/TextField";
import { useEffect, useState } from "react";

export interface LazyColorFieldProps extends Omit<
  TextFieldProps,
  "value" | "defaultValue" | "onChange"
> {
  value?: string;
  defaultValue?: string;
  onValueChange?: (value: string) => void;
}

export const LazyColorField = ({
  value,
  defaultValue,
  onValueChange,
  onBlur,
  onKeyDown,
  ...props
}: LazyColorFieldProps) => {
  const [localValue, setLocalValue] = useState(value ?? defaultValue ?? "#000");

  useEffect(() => {
    if (value !== undefined) setLocalValue(value);
  }, [value]);

  const handleBlur: TextFieldProps["onBlur"] = (e) => {
    if (localValue !== value) {
      onValueChange?.(localValue);
    }
    onBlur?.(e);
  };

  return (
    <TextField
      slotProps={{
        htmlInput: {
          type: "color",
          name: "color",
          value: localValue,
        },
        inputLabel: {
          shrink: true,
        },
      }}
      onChange={(e) => setLocalValue(e.target.value)}
      onBlur={handleBlur}
      {...props}
    />
  );
};
