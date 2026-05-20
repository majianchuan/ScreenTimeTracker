import { useEffect, useState } from "react";
import { Input } from "../lib/shadcn";
import React from "react";

export interface LazyInputNumberProps extends Omit<
  React.ComponentProps<"input">,
  "value" | "onChange" | "defaultValue" | "type"
> {
  value?: number;
  defaultValue?: number;
  onValueChange?: (value: number) => void;
}

export const LazyInputNumber = React.forwardRef<
  HTMLInputElement,
  LazyInputNumberProps
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
    const [localValue, setLocalValue] = useState(
      (value ?? defaultValue ?? "").toString(),
    );

    useEffect(() => {
      if (value !== undefined) setLocalValue(value.toString());
    }, [value]);

    const handleBlur: React.FocusEventHandler<HTMLInputElement> = (e) => {
      const num = parseFloat(localValue);
      if (!isNaN(num) && num !== value) {
        onValueChange?.(num);
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
        type="number"
        value={localValue}
        onChange={(e) => setLocalValue(e.target.value)}
        onBlur={handleBlur}
        onKeyDown={handleKeyDown}
        {...props}
      />
    );
  },
);
