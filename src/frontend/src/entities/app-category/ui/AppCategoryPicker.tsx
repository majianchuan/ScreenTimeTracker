import {
  AppCategoryIcon,
  appCategoryQueries,
  type AppCategory,
} from "@/entities/app-category";
import Autocomplete from "@mui/material/Autocomplete";
import Box from "@mui/material/Box";
import Chip from "@mui/material/Chip";
import type { SxProps, Theme } from "@mui/material/styles";
import TextField from "@mui/material/TextField";
import Typography from "@mui/material/Typography";
import { useQuery } from "@tanstack/react-query";
import { useEffect, useMemo, type SyntheticEvent } from "react";

type Item = Pick<AppCategory, "id" | "name" | "iconPath" | "iconLastUpdatedAt">;

export type AppCategoryPickerProps = {
  className?: string;
  sx?: SxProps<Theme>;
  placeholder?: string;
} & (
  | {
      mode: "single";
      value: string | null;
      onValueChange: (value: string | null) => void;
    }
  | {
      mode: "multiple";
      value: string[];
      onValueChange: (value: string[]) => void;
    }
);

export const AppCategoryPicker = (props: AppCategoryPickerProps) => {
  const { className, sx, mode, value, onValueChange, placeholder } = props;
  const { data, isLoading } = useQuery(
    appCategoryQueries.appCategories({
      fields: "id,name,iconPath,iconLastUpdatedAt",
    }),
  ) as {
    data?: Item[];
    isLoading: boolean;
  };

  // 将外部的 string/string[] 转换为 Autocomplete 需要的对象数组
  const selectedOptions = useMemo(() => {
    if (!data) return [];

    if (mode === "single") {
      const found = data.find((item) => item.id === value);
      return found ? [found] : [];
    } else {
      const valueSet = new Set(value as string[]);
      return data.filter((item) => valueSet.has(item.id));
    }
  }, [data, value, mode]);

  // 数据校验：自动移除不存在的 ID
  useEffect(() => {
    if (!data) return;
    const validIdSet = new Set(data.map((e) => e.id));

    if (mode === "single") {
      if (value && !validIdSet.has(value as string)) onValueChange(null);
    } else {
      const nextValue = (value as string[]).filter((id) => validIdSet.has(id));
      if (nextValue.length !== (value as string[]).length)
        onValueChange(nextValue);
    }
  }, [data, value, mode, onValueChange]);

  const handleChange = (_: SyntheticEvent, newValue: Item[]) => {
    if (mode === "single") {
      const lastSelected =
        newValue.length > 0 ? newValue[newValue.length - 1] : null;
      onValueChange(lastSelected ? lastSelected.id : null);
    } else {
      onValueChange(newValue.map((v) => v.id));
    }
  };

  return (
    <Autocomplete
      multiple
      className={className}
      sx={[{}, ...(Array.isArray(sx) ? sx : [sx])]}
      options={data || []}
      loading={isLoading}
      value={selectedOptions}
      onChange={handleChange}
      getOptionLabel={(option) => option.name}
      isOptionEqualToValue={(option, v) => option.id === v.id}
      renderValue={(value: readonly Item[], getItemProps) =>
        value.map((option: Item, index: number) => {
          const { key, ...itemProps } = getItemProps({ index });
          void key;
          return (
            <Chip
              variant="outlined"
              size="small"
              label={option.name}
              key={option.id}
              {...itemProps}
            />
          );
        })
      }
      renderOption={(props, option) => {
        const { key, ...otherProps } = props;
        void key;
        return (
          <Box component="li" key={option.id} {...otherProps}>
            <AppCategoryIcon
              sx={{
                width: "1.5rem",
                height: "1.r5em",
                mr: 1,
              }}
              id={option.id}
              iconPath={option.iconPath}
              iconLastUpdatedAt={option.iconLastUpdatedAt}
            />
            <Typography>{option.name}</Typography>
          </Box>
        );
      }}
      renderInput={(params) => (
        <TextField {...params} size="small" placeholder={placeholder} />
      )}
      disableCloseOnSelect={mode === "multiple"}
      noOptionsText="暂无数据"
      fullWidth
    />
  );
};
