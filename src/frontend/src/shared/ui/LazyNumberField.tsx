import TextField, { type TextFieldProps } from "@mui/material/TextField";
import { useEffect, useRef, useState } from "react";

export interface LazyNumberFieldProps extends Omit<
  TextFieldProps,
  "value" | "defaultValue" | "onChange" | "type"
> {
  value?: number;
  defaultValue?: number;
  onValueChange?: (value: number | undefined) => void;
  /** 允许负数，默认 true */
  allowNegative?: boolean;
  /** 允许小数，默认 true */
  allowDecimal?: boolean;
  /** 小数位数限制，失焦时会 round 到该精度；不传则不限制 */
  decimalScale?: number;
  /** 最小值，失焦时会 clamp */
  min?: number;
  /** 最大值，失焦时会 clamp */
  max?: number;
  /** 是否允许空值（对外抛 undefined），默认 true */
  allowEmpty?: boolean;
}

const formatNumber = (num: number, decimalScale?: number): string => {
  if (decimalScale === undefined) return String(num);
  // 避免出现 1.00 这种多余的 0，用 parseFloat 去掉尾部 0，再控制精度上限
  return String(parseFloat(num.toFixed(decimalScale)));
};

export const LazyNumberField = ({
  value,
  defaultValue,
  onValueChange,
  onBlur,
  onKeyDown,
  allowNegative = true,
  allowDecimal = true,
  decimalScale,
  min,
  max,
  allowEmpty = true,
  ...props
}: LazyNumberFieldProps) => {
  const [localValue, setLocalValue] = useState(
    value !== undefined
      ? formatNumber(value, decimalScale)
      : defaultValue !== undefined
        ? formatNumber(defaultValue, decimalScale)
        : "",
  );
  const inputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    // eslint-disable-next-line react-hooks/set-state-in-effect
    if (value !== undefined) setLocalValue(formatNumber(value, decimalScale));
    // eslint-disable-next-line react-hooks/set-state-in-effect
    else if (value === undefined && allowEmpty) setLocalValue("");
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [value]);

  // 允许输入过程中的中间态（比如 "-"、"1."、""），只做字符层面的合法性过滤
  const isValidPartialInput = (str: string): boolean => {
    if (str === "") return true;
    let pattern: string;
    if (allowNegative && allowDecimal) pattern = "^-?\\d*\\.?\\d*$";
    else if (allowNegative) pattern = "^-?\\d*$";
    else if (allowDecimal) pattern = "^\\d*\\.?\\d*$";
    else pattern = "^\\d*$";
    if (!new RegExp(pattern).test(str)) return false;

    if (decimalScale !== undefined && allowDecimal) {
      const decimalPart = str.split(".")[1];
      if (decimalPart && decimalPart.length > decimalScale) return false;
    }
    return true;
  };

  const handleChange: TextFieldProps["onChange"] = (e) => {
    const next = e.target.value;
    if (isValidPartialInput(next)) setLocalValue(next);
  };

  const commitValue = () => {
    if (localValue === "" || localValue === "-") {
      if (allowEmpty) {
        setLocalValue("");
        if (value !== undefined) onValueChange?.(undefined);
        return;
      }
      // 不允许空值，回退到上一个合法值
      setLocalValue(
        value !== undefined ? formatNumber(value, decimalScale) : "0",
      );
      return;
    }

    let parsed = parseFloat(localValue);
    if (Number.isNaN(parsed)) {
      setLocalValue(
        value !== undefined ? formatNumber(value, decimalScale) : "",
      );
      return;
    }

    if (min !== undefined) parsed = Math.max(min, parsed);
    if (max !== undefined) parsed = Math.min(max, parsed);
    if (decimalScale !== undefined)
      parsed = parseFloat(parsed.toFixed(decimalScale));

    const formatted = formatNumber(parsed, decimalScale);
    setLocalValue(formatted);
    if (parsed !== value) onValueChange?.(parsed);
  };

  const handleBlur: TextFieldProps["onBlur"] = (e) => {
    commitValue();
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
      onChange={handleChange}
      onBlur={handleBlur}
      onKeyDown={handleKeyDown}
      slotProps={{
        htmlInput: {
          inputMode: allowDecimal ? "decimal" : "numeric",
          ...props.slotProps?.htmlInput,
        },
        ...props.slotProps,
      }}
      {...props}
    />
  );
};
