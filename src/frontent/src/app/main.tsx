// React19+TS+Vite+TanStack Router+TanStack Query+Axios+Zustand+shadcn+Tailwind+Zod+React Hook Form+Recharts
import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./styles/index.css";
import { AppRouterProvider } from "./provider/AppRouterProvider";
import { ThemeProvider } from "next-themes";
import { AppQueryClientProvider } from "./provider/AppQueryClientProvider";
import { TooltipProvider } from "@/shared/lib/shadcn";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <AppQueryClientProvider>
      <ThemeProvider attribute="class" defaultTheme="system" enableSystem>
        <TooltipProvider>
          <AppRouterProvider />
        </TooltipProvider>
      </ThemeProvider>
    </AppQueryClientProvider>
  </StrictMode>,
);
