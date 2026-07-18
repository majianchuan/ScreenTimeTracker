import { getAppIconUrl } from "../api/requests";
import type { SxProps, Theme } from "@mui/material/styles";
import Box from "@mui/material/Box";
import UnknownApp from "@/shared/ui/UnknownApp.svg";

export const AppIcon = ({
  sx,
  className,
  id,
  iconPath,
  iconLastUpdatedAt,
}: {
  sx?: SxProps<Theme>;
  className?: string;
  id: string;
  iconPath: string | null;
  iconLastUpdatedAt: Date;
}) => {
  return (
    <Box
      component="img"
      src={
        iconPath === null ? UnknownApp : getAppIconUrl(id, iconLastUpdatedAt)
      }
      className={className}
      sx={[{}, ...(Array.isArray(sx) ? sx : [sx])]}
    />
  );
};
