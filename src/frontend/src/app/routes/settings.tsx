import { SettingsManagementPage } from "@/pages/settings-management/ui/SettingsManagementPage";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/settings")({
  component: RouteComponent,
});

function RouteComponent() {
  return <SettingsManagementPage />;
}
