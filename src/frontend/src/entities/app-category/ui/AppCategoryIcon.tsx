import { UnknowAppCategoryIcon } from "@/shared/ui/icons";
import { getAppCategoryIconUrl } from "../api/requests";

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
  const FallbackIcon = UnknowAppCategoryIcon;

  if (iconPath) return <img src={iconUrl} className={className} />;
  else return <FallbackIcon className={className} />;
};
