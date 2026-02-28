import {
  Combobox,
  ComboboxChip,
  ComboboxChips,
  ComboboxChipsInput,
  ComboboxContent,
  ComboboxEmpty,
  ComboboxItem,
  ComboboxList,
  ComboboxValue,
  useComboboxAnchor,
} from "@/shared/lib/shadcn";
import {
  AppCategoryIcon,
  appCategoryQueries,
  type AppCategory,
} from "@/entities/app-category";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useMemo } from "react";

export type AppCategoryPickerProps = {
  className?: string;
  mode: "multiple" | "single";
  value: string[];
  onChange: (value: string[]) => void;
  placeholder?: string;
};

export const AppCategoryPicker = ({
  mode,
  value,
  onChange,
  placeholder,
}: AppCategoryPickerProps) => {
  const anchor = useComboboxAnchor();
  type Item = Pick<AppCategory, "id" | "name" | "iconPath">;
  const { data: appCategoriesData } = useQuery(
    appCategoryQueries.appCategories({ fields: "id,name,iconPath" }),
  ) as {
    data?: Item[];
  };

  const selectedEntities = useMemo(() => {
    if (!appCategoriesData) return [];
    const valueSet = new Set(value);
    return appCategoriesData.filter((entity) => valueSet.has(entity.id));
  }, [appCategoriesData, value]);

  useEffect(() => {
    if (!appCategoriesData) return;

    const validIdSet = new Set(appCategoriesData.map((e) => e.id));

    const nextValue = value.filter((id) => validIdSet.has(id));

    // 有变化才触发
    if (nextValue.length !== value.length) {
      console.log(value);
      onChange(nextValue);
    }
  }, [appCategoriesData, value, onChange]);

  const handleSelectedChange = (value: Item[]) => {
    if (mode === "single" && value.length >= 1)
      onChange([value[value.length - 1].id]);
    else onChange(value.map((v) => v.id));
  };

  return (
    // 用[]而不是null传给value表示无选择项会导致问题
    <Combobox
      multiple
      items={appCategoriesData || []}
      value={selectedEntities?.length !== 0 ? selectedEntities : null}
      onValueChange={handleSelectedChange}
    >
      <ComboboxChips ref={anchor}>
        <ComboboxValue>
          {(val) => (
            <>
              {val?.map((v: Item) => (
                <ComboboxChip key={`${v.id}`}>{v.name}</ComboboxChip>
              ))}
              <ComboboxChipsInput
                placeholder={val && val.length !== 0 ? undefined : placeholder}
              />
            </>
          )}
        </ComboboxValue>
      </ComboboxChips>
      <ComboboxContent anchor={anchor}>
        <ComboboxEmpty>暂无</ComboboxEmpty>
        <ComboboxList>
          {(item) => (
            <ComboboxItem key={item.id} value={item}>
              <AppCategoryIcon
                className="size-5"
                id={item.id}
                iconPath={item.iconPath}
              />
              <span className="wrap-anywhere">{item.name}</span>
            </ComboboxItem>
          )}
        </ComboboxList>
      </ComboboxContent>
    </Combobox>
  );
};
