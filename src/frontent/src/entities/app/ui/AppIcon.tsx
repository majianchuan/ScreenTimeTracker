import { UnknowAppIcon } from "@/shared/ui/icons";
import { getAppIconUrl } from "../api/requests";

export const AppIcon = ({
  className,
  id,
  iconPath,
}: {
  className?: string;
  id: string;
  iconPath: string | null;
}) => {
  const iconUrl = getAppIconUrl(id);
  const FallbackIcon = UnknowAppIcon;

  if (iconPath) return <img src={iconUrl} className={className} />;
  else return <FallbackIcon className={className} />;
};
