import { AppManagementPage } from "@/pages/app-management";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/apps")({
  component: RouteComponent,
});

// eslint-disable-next-line react-refresh/only-export-components
function RouteComponent() {
  return <AppManagementPage />;
}
