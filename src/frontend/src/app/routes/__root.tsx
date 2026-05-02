import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";
import { useState } from "react";
import {
  AppWindow,
  ChartColumn,
  ChartColumnBig,
  Database,
  LayoutPanelLeft,
  Settings,
} from "lucide-react";
import { NavMenu, type NavMenuItem } from "@/shared/ui/NavMenu";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";
import { AppHeader } from "@/shared/ui/AppHeader";
import { Toaster } from "@/shared/lib/shadcn";
import type { QueryClient } from "@tanstack/react-query";

interface RouterContext {
  queryClient: QueryClient;
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
});

function RootComponent() {
  const [headerHeightNum] = useState<number>(50);
  const headerHeight = `${headerHeightNum}px`;
  const [leftDrawerWidthNum] = useState<number>(190);
  const leftDrawerWidth = `${leftDrawerWidthNum}px`;

  const navMenuItems: NavMenuItem[] = [
    {
      to: "/usage/summary",
      icon: ChartColumnBig,
      label: "使用汇总",
    },
    {
      to: "/usage/details",
      icon: ChartColumn,
      label: "使用详情",
    },
    {
      to: "/apps",
      icon: AppWindow,
      label: "应用管理",
    },
    {
      to: "/app-categories",
      icon: LayoutPanelLeft,
      label: "类别管理",
    },
    {
      to: "/data",
      icon: Database,
      label: "数据管理",
    },
    {
      to: "/settings",
      icon: Settings,
      label: "设置",
    },
  ];

  return (
    <>
      <TanStackRouterDevtools />
      <div className="layout">
        <div
          className="page-container min-h-screen"
          style={{ paddingLeft: leftDrawerWidth, paddingTop: headerHeight }}
        >
          <main className="overflow-auto p-4">
            <Outlet />
            <Toaster closeButton />
          </main>
        </div>

        <div
          className="drawer-container border-border fixed bottom-0 left-0 border-r"
          style={{ width: leftDrawerWidth, top: headerHeight }}
        >
          <aside className="flex h-full flex-col overflow-auto px-2 py-3">
            <NavMenu items={navMenuItems} />
          </aside>
        </div>

        <div
          className="header-container border-border fixed top-0 right-0 left-0 border-b"
          style={{ height: headerHeight }}
        >
          <AppHeader />
        </div>
      </div>
    </>
  );
}
