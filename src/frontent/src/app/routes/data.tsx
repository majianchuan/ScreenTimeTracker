import { DataManagementPage } from "@/pages/data-management";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/data")({
  component: RouteComponent,
});

function RouteComponent() {
  return <DataManagementPage />;
}
