import TextField, { type TextFieldProps } from "@mui/material/TextField";
import { useEffect, useRef, useState } from "react";

export interface LazyTextFieldProps extends Omit<
  TextFieldProps,
  "value" | "defaultValue" | "onChange"
> {
  value?: string;
  defaultValue?: string;
  onValueChange?: (value: string) => void;
}

export const LazyTextField = ({
  value,
  defaultValue,
  onValueChange,
  onBlur,
  onKeyDown,
  ...props
}: LazyTextFieldProps) => {
  const [localValue, setLocalValue] = useState(value ?? defaultValue ?? "");
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    if (value !== undefined) setLocalValue(value);
  }, [value]);

  const handleBlur: TextFieldProps["onBlur"] = (e) => {
    if (localValue !== value) {
      onValueChange?.(localValue);
    }
    onBlur?.(e);
  };

  const handleKeyDown: TextFieldProps["onKeyDown"] = (e) => {
    if (e.key === "Enter") {
      inputRef.current?.blur();
    }
    onKeyDown?.(e);
  };

  return (
    <TextField
      inputRef={inputRef}
      value={localValue}
      onChange={(e) => setLocalValue(e.target.value)}
      onBlur={handleBlur}
      onKeyDown={handleKeyDown}
      {...props}
    />
  );
};
