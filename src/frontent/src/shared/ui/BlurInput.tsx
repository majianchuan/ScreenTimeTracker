import { useState } from "react";
import { Input } from "../lib/shadcn";

type InputValue = React.ComponentProps<"input">["value"];

export interface BlurInputProps extends Omit<
  React.ComponentProps<"input">,
  "onChange" | "onBlur"
> {
  onBlurUpdate(value: InputValue): void;
}

export const BlurInput = ({
  className,
  value,
  onBlurUpdate,
  ...props
}: BlurInputProps) => {
  const [localValue, setLocalValue] = useState(value);
  const [prevValueProp, setPrevValueProp] = useState(value);

  if (value !== prevValueProp) {
    setPrevValueProp(value);
    setLocalValue(value);
  }

  const handleBlur = () => {
    // 只有当当前输入的值与原本的值不相等时，才触发更新函数
    if (localValue !== value) {
      onBlurUpdate(localValue);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    // 按下回车键时，主动让输入框失去焦点，从而触发 handleBlur 逻辑
    if (e.key === "Enter") {
      e.currentTarget.blur();
    }
  };

  return (
    <Input
      className={className}
      value={localValue}
      onChange={(e) => setLocalValue(e.target.value)}
      onBlur={handleBlur}
      onKeyDown={handleKeyDown}
      {...props}
    />
  );
};
