import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { UsageDetailsPage, searchParamsSchema } from "@/pages/usage-details";
import type { SearchParams } from "@/pages/usage-details";
import { userSettingsQueries } from "@/entities/user-settings";
import { dateToDateOnly } from "@/shared/lib/date-only";
import Box from "@mui/material/Box";
import CircularProgress from "@mui/material/CircularProgress";
import dayjs from "@/shared/lib/dayjs";

export const Route = createFileRoute("/usage/details")({
  component: RouteComponent,
  validateSearch: searchParamsSchema.partial({
    startDate: true,
    endDate: true,
  }),
  pendingComponent: () => (
    <Box
      sx={{
        display: "flex",
        justifyContent: "center",
      }}
    >
      <CircularProgress />
    </Box>
  ),
  beforeLoad: async ({ context, search }) => {
    if (search.startDate && search.endDate) return;

    const { dayCutoffHour } = await context.queryClient.ensureQueryData(
      userSettingsQueries.userSettings(),
    );
    const logicalToday = dayjs()
      .subtract(dayCutoffHour, "hour")
      .startOf("day")
      .toDate();
    const logicalTodayDateOnly = dateToDateOnly(logicalToday);

    throw redirect({
      to: "/usage/details",
      search: {
        startDate: logicalTodayDateOnly,
        endDate: logicalTodayDateOnly,
      },
      replace: true,
    });
  },
});

// eslint-disable-next-line react-refresh/only-export-components
function RouteComponent() {
  const search = Route.useSearch() as SearchParams;
  const navigate = useNavigate({ from: Route.fullPath });
  const handleSearchChange = (newParams: Partial<SearchParams>) => {
    navigate({ search: (prev) => ({ ...prev, ...newParams }) });
  };

  return (
    <UsageDetailsPage search={search} onSearchChange={handleSearchChange} />
  );
}
