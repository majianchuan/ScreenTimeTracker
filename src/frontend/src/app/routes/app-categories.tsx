import { AppCategoryManagementPage } from "@/pages/app-category-management";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/app-categories")({
  component: RouteComponent,
});

// eslint-disable-next-line react-refresh/only-export-components
function RouteComponent() {
  return <AppCategoryManagementPage />;
}
