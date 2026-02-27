import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { UsageDetailsPage, searchParamsSchema } from "@/pages/usage-details";
import type { SearchParams } from "@/pages/usage-details";

export const Route = createFileRoute("/usage/details")({
  component: RouteComponent,
  validateSearch: searchParamsSchema,
});

function RouteComponent() {
  const search = Route.useSearch();
  const navigate = useNavigate({ from: Route.fullPath });
  const handleSearchChange = (newParams: Partial<SearchParams>) => {
    navigate({ search: (prev) => ({ ...prev, ...newParams }) });
  };

  return (
    <UsageDetailsPage search={search} onSearchChange={handleSearchChange} />
  );
}
