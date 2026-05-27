import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";
import { UsageSummaryPage, searchParamsSchema } from "@/pages/usage-summary";
import type { SearchParams } from "@/pages/usage-summary";
import { userSettingsQueries } from "@/entities/user-settings";
import { startOfDay, subHours } from "date-fns";
import { dateToDateOnly } from "@/shared/lib/date-only";

export const Route = createFileRoute("/usage/summary")({
  component: RouteComponent,
  validateSearch: searchParamsSchema.partial({
    startDate: true,
    endDate: true,
  }),
  pendingComponent: () => (
    <div className="flex h-full items-center justify-center">
      <span>正在获取数据，加载中。。。</span>
    </div>
  ),
  errorComponent: () => (
    <div className="flex h-full items-center justify-center">
      <span>无法获取数据，请检查网络</span>
    </div>
  ),
  beforeLoad: async ({ context, search }) => {
    if (search.startDate && search.endDate) return;

    const { dayCutoffHour } = await context.queryClient.ensureQueryData(
      userSettingsQueries.userSettings(),
    );
    const logicalToday = startOfDay(subHours(new Date(), dayCutoffHour));
    const logicalTodayDateOnly = dateToDateOnly(logicalToday);

    throw redirect({
      to: "/usage/summary",
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
    <UsageSummaryPage search={search} onSearchChange={handleSearchChange} />
  );
}
