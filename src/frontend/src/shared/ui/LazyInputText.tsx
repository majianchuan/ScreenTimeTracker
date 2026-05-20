import { useEffect, useState } from "react";
import { Input } from "../lib/shadcn";
import React from "react";

export interface LazyInputTextProps extends Omit<
  React.ComponentProps<"input">,
  "value" | "onChange" | "defaultValue" | "type"
> {
  value?: string;
  defaultValue?: string;
  onValueChange?: (value: string) => void;
}

export const LazyInputText = React.forwardRef<
  HTMLInputElement,
  LazyInputTextProps
>(
  (
    {
      className,
      value,
      defaultValue,
      onValueChange,
      onBlur,
      onKeyDown,
      ...props
    },
    ref,
  ) => {
    const [localValue, setLocalValue] = useState(value ?? defaultValue ?? "");

    useEffect(() => {
      if (value !== undefined) setLocalValue(value);
    }, [value]);

    const handleBlur: React.FocusEventHandler<HTMLInputElement> = (e) => {
      if (localValue !== value) {
        onValueChange?.(localValue);
      }
      onBlur?.(e);
    };

    const handleKeyDown: React.KeyboardEventHandler<HTMLInputElement> = (e) => {
      if (e.key === "Enter") {
        e.currentTarget.blur();
      }
      onKeyDown?.(e);
    };

    return (
      <Input
        ref={ref}
        className={className}
        type="text"
        value={localValue}
        onChange={(e) => setLocalValue(e.target.value)}
        onBlur={handleBlur}
        onKeyDown={handleKeyDown}
        {...props}
      />
    );
  },
);
