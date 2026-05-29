import { UnknowAppIcon } from "@/shared/ui/icons";
import { getAppIconUrl } from "../api/requests";
import { cn } from "@/shared/lib/shadcn";

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

  if (iconPath)
    return <img src={iconUrl} className={cn(className, "max-w-none")} />;
  else return <FallbackIcon className={className} />;
};
