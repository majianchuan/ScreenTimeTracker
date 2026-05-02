import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { UsageDetailsPage, searchParamsSchema } from "@/pages/usage-details";
import type { SearchParams } from "@/pages/usage-details";
import { screenTimeUserSettingsQueries } from "@/entities/screen-time-user-settings";
import { startOfDay, subHours } from "date-fns";
import { dateToDateOnly } from "@/shared/lib/date-only";

export const Route = createFileRoute("/usage/details")({
  component: RouteComponent,
  validateSearch: searchParamsSchema.partial({
    startDate: true,
    endDate: true,
  }),
  beforeLoad: async ({ context, search }) => {
    if (search.startDate && search.endDate) return;

    const { dayCutoffHour } = await context.queryClient.ensureQueryData(
      screenTimeUserSettingsQueries.screenTimeUserSettings(),
    );
    const logicalToday = startOfDay(subHours(new Date(), dayCutoffHour));
    const logicalTodayDateOnly = dateToDateOnly(logicalToday);

    throw redirect({
      to: "/usage/details",
      search: {
        startDate: logicalTodayDateOnly,
        endDate: logicalTodayDateOnly,
      },
    });
  },
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
