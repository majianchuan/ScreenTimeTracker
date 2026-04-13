import { AppCategoryManagementPage } from "@/pages/app-category-management";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/app-categories")({
  component: RouteComponent,
});

function RouteComponent() {
  return <AppCategoryManagementPage />;
}
