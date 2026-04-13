import { cn, ToggleGroup, ToggleGroupItem } from "@/shared/lib/shadcn";
import type { Dimension } from "../model/schemas";

export type DateRangeSelectorProps = {
  className?: string;
  value: Dimension;
  onChange: (value: Dimension) => void;
};
export const DimensionTypeSelector = ({
  className,
  value,
  onChange,
}: DateRangeSelectorProps) => {
  type DimensionOption = {
    value: Dimension;
    label: string;
  };

  const dimensionOptions: DimensionOption[] = [
    {
      value: "app",
      label: "应用",
    },
    {
      value: "app-category",
      label: "类别",
    },
  ];

  return (
    <ToggleGroup
      variant="outline"
      type="single"
      value={value}
      onValueChange={(dimension) => {
        if (dimension === "") return;
        onChange(dimension as Dimension);
      }}
      className={cn(className)}
    >
      {dimensionOptions.map((item) => (
        <ToggleGroupItem key={item.value} value={item.value}>
          {item.label}
        </ToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};
