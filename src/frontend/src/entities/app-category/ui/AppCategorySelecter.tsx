import {
  AppCategoryIcon,
  appCategoryQueries,
  type AppCategory,
} from "@/entities/app-category";
import Box from "@mui/material/Box";
import MenuItem from "@mui/material/MenuItem";
import Select, { type SelectChangeEvent } from "@mui/material/Select";
import type { SxProps, Theme } from "@mui/material/styles";
import Typography from "@mui/material/Typography";
import { useQuery } from "@tanstack/react-query";

type Item = Pick<AppCategory, "id" | "name" | "iconPath" | "iconLastUpdatedAt">;

export type AppCategorySelecterProps = {
  className?: string;
  sx?: SxProps<Theme>;
  value: string;
  onValueChange: (value: string) => void;
};

export const AppCategorySelecter = ({
  className,
  sx,
  value,
  onValueChange,
}: AppCategorySelecterProps) => {
  const { data } = useQuery(
    appCategoryQueries.appCategories({
      fields: "id,name,iconPath,iconLastUpdatedAt",
    }),
  ) as {
    data?: Item[];
    isLoading: boolean;
  };

  const handleChange = (event: SelectChangeEvent) => {
    onValueChange(event.target.value);
  };

  if (!data) return null;

  return (
    <Select
      size="small"
      className={className}
      sx={[{}, ...(Array.isArray(sx) ? sx : [sx])]}
      value={value}
      onChange={handleChange}
    >
      {data?.map((item) => {
        return (
          <MenuItem value={item.id} key={item.id}>
            <Box sx={{ display: "flex", alignItems: "center" }}>
              <AppCategoryIcon
                id={item.id}
                iconPath={item.iconPath}
                iconLastUpdatedAt={item.iconLastUpdatedAt}
                sx={{ width: "1.5rem", height: "1.5rem", mr: 1 }}
              />
              <Typography>{item.name}</Typography>
            </Box>
          </MenuItem>
        );
      })}
    </Select>
  );
};
