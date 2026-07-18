import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./styles/index.css";
import { AppRouterProvider } from "./provider/AppRouterProvider";
import { AppQueryClientProvider } from "./provider/AppQueryClientProvider";
import CssBaseline from "@mui/material/CssBaseline";
import { AppThemeProvider } from "./provider/AppThemeProvider";
import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";
import { AppLocalizationProvider } from "./provider/AppLocalizationProvider";
import dayjs from "@/shared/lib/dayjs";
import "dayjs/locale/zh-cn";
import { SnackbarProvider } from "notistack";

dayjs.locale("zh-cn");

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <SnackbarProvider>
      <AppLocalizationProvider>
        <AppQueryClientProvider>
          <AppThemeProvider>
            <CssBaseline />
            <AppRouterProvider />
          </AppThemeProvider>
        </AppQueryClientProvider>
      </AppLocalizationProvider>
    </SnackbarProvider>
  </StrictMode>,
);
