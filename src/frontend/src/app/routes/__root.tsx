import {
  createRootRouteWithContext,
  Link,
  Outlet,
  useLocation,
  useNavigate,
} from "@tanstack/react-router";
import { TanStackRouterDevtools } from "@tanstack/react-router-devtools";
import type { QueryClient } from "@tanstack/react-query";
import Box from "@mui/material/Box";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Logo from "@/shared/ui/Logo.svg?react";
import Typography from "@mui/material/Typography";
import IconButton from "@mui/material/IconButton";
import RefreshIcon from "@mui/icons-material/Refresh";
import DarkModeIcon from "@mui/icons-material/DarkMode";
import LightModeIcon from "@mui/icons-material/LightMode";
import DashboardOutlinedIcon from "@mui/icons-material/DashboardOutlined";
import BarChartOutlinedIcon from "@mui/icons-material/BarChartOutlined";
import AppsOutlinedIcon from "@mui/icons-material/AppsOutlined";
import CategoryOutlinedIcon from "@mui/icons-material/CategoryOutlined";
import StorageOutlinedIcon from "@mui/icons-material/StorageOutlined";
import SettingsOutlinedIcon from "@mui/icons-material/SettingsOutlined";
import Divider from "@mui/material/Divider";
import { SiDiscord, SiGithub, SiQq } from "@icons-pack/react-simple-icons";
import Drawer from "@mui/material/Drawer";
import List from "@mui/material/List";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import ListItemText from "@mui/material/ListItemText";
import { useColorScheme } from "@mui/material/styles";
import { useState } from "react";
import Stack from "@mui/material/Stack";

interface RouterContext {
  queryClient: QueryClient;
}

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootComponent,
});

// eslint-disable-next-line react-refresh/only-export-components
function RootComponent() {
  const { mode, systemMode, setMode } = useColorScheme();

  const drawerWidth = 200;
  const navMenuItems = [
    {
      to: "/usage/summary",
      icon: <DashboardOutlinedIcon />,
      label: "使用汇总",
    },
    {
      to: "/usage/details",
      icon: <BarChartOutlinedIcon />,
      label: "使用详情",
    },
    {
      to: "/apps",
      icon: <AppsOutlinedIcon />,
      label: "应用管理",
    },
    {
      to: "/app-categories",
      icon: <CategoryOutlinedIcon />,
      label: "类别管理",
    },
    {
      to: "/data",
      icon: <StorageOutlinedIcon />,
      label: "数据管理",
    },
    {
      to: "/settings",
      icon: <SettingsOutlinedIcon />,
      label: "设置",
    },
  ];

  const location = useLocation();
  const navigate = useNavigate();
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

  if (!mode) return null;
  const isDark = (mode === "system" ? systemMode : mode) === "dark";

  const toogleMode = () => {
    const newMode = isDark ? "light" : "dark";
    setMode(newMode === systemMode ? "system" : newMode);
  };

  return (
    <>
      <TanStackRouterDevtools />
      <Box sx={{ display: "flex" }}>
        <AppBar
          position="fixed"
          sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}
        >
          <Toolbar variant="dense" sx={{ justifyContent: "space-between" }}>
            <Box
              component={Link}
              to="/"
              sx={{
                display: "flex",
                alignItems: "center",
                textDecoration: "none",
                color: "inherit",
              }}
            >
              <Logo height="28px" />
              <Typography variant="h6" noWrap component="div" sx={{ ml: 1 }}>
                Screen Time Tracker
              </Typography>
            </Box>

            <Stack direction="row" spacing={1}>
              <IconButton
                color="inherit"
                size="small"
                onClick={() => window.location.reload()}
              >
                <RefreshIcon fontSize="inherit" />
              </IconButton>
              <Divider orientation="vertical" flexItem />
              <IconButton
                color="inherit"
                size="small"
                href="https://qm.qq.com/q/uiwJZiQRAm"
                target="_blank"
              >
                <SiQq size="1em" />
              </IconButton>
              <IconButton
                color="inherit"
                size="small"
                href="https://github.com/majianchuan/ScreenTimeTracker"
                target="_blank"
              >
                <SiGithub size="1em" />
              </IconButton>
              <IconButton
                color="inherit"
                size="small"
                href="https://discord.gg/PxqGwcsVuh"
                target="_blank"
              >
                <SiDiscord size="1em" />
              </IconButton>
              <Divider orientation="vertical" flexItem />
              <IconButton color="inherit" size="small" onClick={toogleMode}>
                {isDark ? (
                  <DarkModeIcon fontSize="inherit" />
                ) : (
                  <LightModeIcon fontSize="inherit" />
                )}
              </IconButton>
            </Stack>
          </Toolbar>
        </AppBar>

        <Drawer
          variant="permanent"
          sx={{
            width: drawerWidth,
            flexShrink: 0,
            [`& .MuiDrawer-paper`]: {
              width: drawerWidth,
              boxSizing: "border-box",
            },
          }}
        >
          <Toolbar variant="dense" />
          <Box sx={{ overflow: "auto" }}>
            <List>
              {navMenuItems.map((item) => (
                <ListItem key={item.to} disablePadding>
                  <ListItemButton
                    selected={location.pathname === item.to}
                    onClick={() => {
                      navigate({
                        to: item.to as never,
                        search: searchMap[item.to] as never,
                      });
                    }}
                  >
                    <ListItemIcon>{item.icon}</ListItemIcon>
                    <ListItemText
                      primary={item.label}
                      sx={{
                        textAlign: "center",
                        pr: 2,
                      }}
                      slotProps={{
                        primary: {
                          sx: { fontWeight: "bold" },
                        },
                      }}
                    />
                  </ListItemButton>
                </ListItem>
              ))}
            </List>
          </Box>
        </Drawer>

        <Box component="main" sx={{ flexGrow: 1, minWidth: 0, p: 2 }}>
          <Toolbar variant="dense" />
          <Outlet />
        </Box>
      </Box>
    </>
  );
}
