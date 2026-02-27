import { AppManagementPage } from "@/pages/app-management";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/apps")({
  component: RouteComponent,
});

function RouteComponent() {
  return <AppManagementPage />;
}
