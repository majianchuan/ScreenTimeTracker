import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { UsageSummaryPage, searchParamsSchema } from "@/pages/usage-summary";
import type { SearchParams } from "@/pages/usage-summary";

export const Route = createFileRoute("/usage/summary")({
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
    <UsageSummaryPage search={search} onSearchChange={handleSearchChange} />
  );
}
