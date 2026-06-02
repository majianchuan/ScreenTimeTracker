import { UnknownAppCategoryIcon } from "@/shared/ui/icons";
import { getAppCategoryIconUrl } from "../api/requests";
import { cn } from "@/shared/lib/shadcn";

export const AppCategoryIcon = ({
  className,
  id,
  iconPath,
}: {
  className?: string;
  id: string;
  iconPath: string | null;
}) => {
  const iconUrl = getAppCategoryIconUrl(id);
  const FallbackIcon = UnknownAppCategoryIcon;

  if (iconPath)
    return <img src={iconUrl} className={cn(className, "max-w-none")} />;
  else return <FallbackIcon className={className} />;
};
