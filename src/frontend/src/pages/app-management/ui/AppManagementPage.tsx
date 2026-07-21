import {
  AppIcon,
  appQueries,
  useDeleteApp,
  usePatchApp,
  type App,
} from "@/entities/app";
import DeleteIcon from "@mui/icons-material/Delete";
import { LazyColorField } from "@/shared/ui/LazyColorField";
import { useQuery } from "@tanstack/react-query";
import { useMemo, useState } from "react";
import Box from "@mui/material/Box";
import { useSnackbar } from "notistack";
import Switch from "@mui/material/Switch";
import IconButton from "@mui/material/IconButton";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogActions from "@mui/material/DialogActions";
import Button from "@mui/material/Button";
import {
  DataGrid,
  type GridColDef,
  type GridRenderCellParams,
  type GridRowsProp,
} from "@mui/x-data-grid";
import {
  appCategoryQueries,
  AppCategorySelecter,
  type AppCategory,
} from "@/entities/app-category";
import { LazyTextField } from "@/shared/ui/LazyTextField";
import dayjs from "@/shared/lib/dayjs";
import { useTranslation } from "react-i18next";

export const AppManagementPage = () => {
  const { t } = useTranslation(["page_appManagement", "shared"]);
  const { enqueueSnackbar } = useSnackbar();
  const { data: appsData, isLoading: isAppsDataLoading } = useQuery(
    appQueries.apps({}),
  ) as {
    isLoading: boolean;
    data?: App[];
  };
  const { mutateAsync: patchAppAsync } = usePatchApp();
  const { mutateAsync: deleteAppAsync } = useDeleteApp();
  const { data: appCategoriesData, isLoading: isAppCategoriesDataLoading } =
    useQuery(appCategoryQueries.appCategories({ fields: "id,name" })) as {
      isLoading: boolean;
      data?: Pick<AppCategory, "id" | "name">[];
    };
  const categoryMap = useMemo(() => {
    return new Map(
      appCategoriesData?.map((item) => [item.id, item.name]) ?? [],
    );
  }, [appCategoriesData]);

  const [deleteAppComfirmDialogOpen, setDeleteAppComfirmDialogOpen] =
    useState(false);
  const [appToDelete, setAppToDelete] = useState<App | null>(null);

  const stopGridKeyboardEvent = (e: React.KeyboardEvent) => {
    e.stopPropagation();
  };

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: t("columns.name"),
      width: 150,
      renderCell: (params: GridRenderCellParams<App, string>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <LazyTextField
            fullWidth
            size="small"
            value={params.value}
            onKeyDown={stopGridKeyboardEvent}
            onValueChange={async (value) => {
              try {
                await patchAppAsync({
                  id: params.row.id,
                  body: { name: value },
                });
              } catch {
                enqueueSnackbar(
                  t("errors.updateFailed", {
                    ns: "shared",
                    field: t("columns.name"),
                  }),
                  { variant: "error" },
                );
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "color",
      headerName: t("columns.color"),
      width: 70,
      renderCell: (params: GridRenderCellParams<App, string>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <LazyColorField
            size="small"
            sx={{ width: "3rem" }}
            value={params.value}
            onValueChange={async (value) => {
              try {
                await patchAppAsync({
                  id: params.row.id,
                  body: { color: value },
                });
              } catch {
                enqueueSnackbar(
                  t("errors.updateFailed", {
                    ns: "shared",
                    field: t("columns.color"),
                  }),
                  { variant: "error" },
                );
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "appCategory",
      headerName: t("columns.appCategory"),
      width: 210,
      valueGetter: (_, row) => {
        return categoryMap.get(row.appCategoryId) ?? "";
      },
      renderCell: (params: GridRenderCellParams<App>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <AppCategorySelecter
            value={params.row.appCategoryId}
            onValueChange={async (value) => {
              try {
                await patchAppAsync({
                  id: params.row.id,
                  body: { appCategoryId: value },
                });
              } catch {
                enqueueSnackbar(
                  t("errors.updateFailed", {
                    ns: "shared",
                    field: t("columns.appCategory"),
                  }),
                  { variant: "error" },
                );
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "icon",
      headerName: t("columns.icon"),
      width: 50,
      sortable: false,
      filterable: false,
      disableColumnMenu: true,
      renderCell: (params: GridRenderCellParams<App>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <AppIcon
            id={params.row.id}
            iconPath={params.row.iconPath}
            iconLastUpdatedAt={params.row.iconLastUpdatedAt}
            sx={{
              width: "2rem",
              height: "2rem",
            }}
          />
        </Box>
      ),
    },
    {
      field: "iconPath",
      headerName: t("columns.iconPath"),
      width: 150,
      renderCell: (params: GridRenderCellParams<App, string>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <LazyTextField
            fullWidth
            size="small"
            value={params.value || ""}
            onKeyDown={stopGridKeyboardEvent}
            onValueChange={async (value) => {
              try {
                await patchAppAsync({
                  id: params.row.id,
                  body: { iconPath: value === "" ? null : value },
                });
              } catch {
                enqueueSnackbar(
                  t("errors.updateFailed", {
                    ns: "shared",
                    field: t("columns.iconPath"),
                  }),
                  { variant: "error" },
                );
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "isAutoUpdateEnabled",
      headerName: t("columns.isAutoUpdateEnabled"),
      width: 80,
      renderCell: (params: GridRenderCellParams<App, boolean>) => (
        <Switch
          checked={params.value}
          onChange={async (event: React.ChangeEvent<HTMLInputElement>) => {
            try {
              await patchAppAsync({
                id: params.row.id,
                body: { isAutoUpdateEnabled: event.target.checked },
              });
            } catch {
              enqueueSnackbar(
                t("errors.updateFailed", {
                  ns: "shared",
                  field: t("columns.isAutoUpdateEnabled"),
                }),
                { variant: "error" },
              );
            }
          }}
        />
      ),
    },
    {
      field: "lastAutoUpdatedAt",
      headerName: t("columns.lastAutoUpdatedAt"),
      width: 140,
      valueFormatter: (value) => dayjs(value).format("L LT"),
    },
    {
      field: "processName",
      headerName: t("columns.processName"),
      width: 150,
    },
    {
      field: "executablePath",
      headerName: t("columns.executablePath"),
      width: 200,
    },
    {
      field: "action",
      headerName: t("columns.action"),
      width: 60,
      sortable: false,
      filterable: false,
      disableColumnMenu: true,
      renderCell: (params: GridRenderCellParams<App>) => (
        <IconButton
          disabled={params.row.isSystem}
          color="error"
          onClick={() => {
            setAppToDelete(params.row);
            setDeleteAppComfirmDialogOpen(true);
          }}
        >
          <DeleteIcon />
        </IconButton>
      ),
    },
  ];

  const rows: GridRowsProp = appsData || [];

  return (
    <>
      <Box sx={{ width: "100%" }}>
        <DataGrid
          showToolbar
          autoHeight
          initialState={{
            pagination: { paginationModel: { pageSize: 10 } },
            columns: {
              columnVisibilityModel: {
                lastAutoUpdated: false,
                processName: false,
                executablePath: false,
              },
            },
          }}
          pageSizeOptions={[5, 10, 20, 40]}
          loading={isAppsDataLoading || isAppCategoriesDataLoading}
          rows={rows}
          columns={columns}
        />
      </Box>

      <Dialog
        open={deleteAppComfirmDialogOpen}
        onClose={() => setDeleteAppComfirmDialogOpen(false)}
        role="alertdialog"
      >
        <DialogTitle>
          {t("deleteConfirm.title", { name: appToDelete?.name })}
        </DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            {t("deleteConfirm.description")}
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setDeleteAppComfirmDialogOpen(false)}
            autoFocus
          >
            {t("actions.cancel", { ns: "shared" })}
          </Button>
          <Button
            onClick={async () => {
              if (appToDelete == null) {
                enqueueSnackbar(t("errors.noAppSelected"), {
                  variant: "error",
                });
                return;
              }
              try {
                await deleteAppAsync(appToDelete.id);
              } catch {
                enqueueSnackbar(
                  t("errors.deleteFailed", {
                    ns: "shared",
                    field: t("entityName"),
                  }),
                  { variant: "error" },
                );
              } finally {
                setDeleteAppComfirmDialogOpen(false);
                setAppToDelete(null);
              }
            }}
            color="error"
          >
            {t("actions.confirm", { ns: "shared" })}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
