import {
  ToggleGroup,
  ToggleGroupItem,
} from "@/shared/lib/shadcn/components/toggle-group";
import { Link, useLocation } from "@tanstack/react-router";
import { useState, type ComponentType } from "react";

export interface NavMenuItem {
  to: string;
  icon: ComponentType<{ className?: string }>;
  label: string;
}

interface NavMenuProps {
  items: NavMenuItem[];
}

export const NavMenu = ({ items }: NavMenuProps) => {
  const location = useLocation();

  const [searchMap, setSearchMap] = useState<
    Record<string, Record<string, unknown>>
  >({});

  const savedSearch = searchMap[location.pathname];

  if (JSON.stringify(savedSearch) !== JSON.stringify(location.search)) {
    setSearchMap((prev) => ({
      ...prev,
      [location.pathname]: location.search,
    }));
  }

  return (
    <ToggleGroup
      type="single"
      value={location.pathname}
      orientation="vertical"
      spacing={1}
      className={"w-full"}
    >
      {items.map((item) => (
        <ToggleGroupItem key={item.to} value={item.to} asChild>
          <Link
            to={item.to}
            search={searchMap[item.to]}
            className="flex h-10 w-full flex-row px-3"
          >
            <item.icon className="h-5! w-5!" />
            <span className="flex-1 text-center text-base">{item.label}</span>
          </Link>
        </ToggleGroupItem>
      ))}
    </ToggleGroup>
  );
};
