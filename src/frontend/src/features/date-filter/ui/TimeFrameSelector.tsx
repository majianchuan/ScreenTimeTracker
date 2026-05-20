import { cn, ToggleGroup, ToggleGroupItem } from "@/shared/lib/shadcn";
import type { TimeFrame } from "../model/schemas";

export type PeriodTypeSelectorProps = {
  className?: string;
  value: TimeFrame;
  onValueChange: (value: TimeFrame) => void;
};

export const PeriodTypeSelector = ({
  className,
  value,
  onValueChange,
}: PeriodTypeSelectorProps) => {
  type PeriodTypeOption = { value: TimeFrame; label: string };
  const options: PeriodTypeOption[] = [
    {
      value: "day",
      label: "日",
    },
    {
      value: "week",
      label: "周",
    },
    {
      value: "month",
      label: "月",
    },
    {
      value: "custom",
      label: "自定义",
    },
  ];

  return (
    <ToggleGroup
      className={cn(className)}
      variant="outline"
      type="single"
      value={value}
      onValueChange={(v) => {
        if (v === "") return;
        onValueChange(v as TimeFrame);
      }}
    >
      {options.map((item) => (
        <ToggleGroupItem key={item.value} value={item.value}>
          {item.label}
        </ToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};
