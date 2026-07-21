import {
  AppCategoryIcon,
  appCategoryQueries,
  useCreateAppCategory,
  useDeleteAppCategory,
  usePatchAppCategory,
  type AppCategory,
} from "@/entities/app-category";
import { useQuery } from "@tanstack/react-query";
import { LazyColorField } from "@/shared/ui/LazyColorField";
import {
  DataGrid,
  type GridColDef,
  type GridRenderCellParams,
  type GridRowsProp,
} from "@mui/x-data-grid";
import Box from "@mui/material/Box";
import { useSnackbar } from "notistack";
import IconButton from "@mui/material/IconButton";
import DeleteIcon from "@mui/icons-material/Delete";
import Button from "@mui/material/Button";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogContent from "@mui/material/DialogContent";
import DialogContentText from "@mui/material/DialogContentText";
import DialogTitle from "@mui/material/DialogTitle";
import { useState } from "react";
import TextField from "@mui/material/TextField";
import { LazyTextField } from "@/shared/ui/LazyTextField";
import { useTranslation } from "react-i18next";

export const AppCategoryManagementPage = () => {
  const { t } = useTranslation([
    "page_appCategoryManagement",
    "shared",
    "entity_appCategory",
  ]);
  const { enqueueSnackbar } = useSnackbar();
  const { data: appCategoriesData, isLoading } = useQuery(
    appCategoryQueries.appCategories({}),
  ) as {
    data?: AppCategory[];
    isLoading: boolean;
  };
  const { mutateAsync: createAppCategoryAsync } = useCreateAppCategory();
  const { mutateAsync: patchAppCategoryAsync } = usePatchAppCategory();
  const { mutateAsync: deleteAppCategoryAsync } = useDeleteAppCategory();

  const [
    deleteAppCategoryComfirmDialogOpen,
    setDeleteAppCategoryComfirmDialogOpen,
  ] = useState(false);
  const [createAppCategoryDialogOpen, setCreateAppCategoryDialogOpen] =
    useState(false);
  const [appCategoryToDelete, setAppCategoryToDelete] =
    useState<AppCategory | null>(null);

  const stopGridKeyboardEvent = (e: React.KeyboardEvent) => {
    e.stopPropagation();
  };

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: t("columns.name"),
      width: 150,
      renderCell: (params: GridRenderCellParams<AppCategory, string>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <LazyTextField
            fullWidth
            size="small"
            value={params.value}
            onKeyDown={stopGridKeyboardEvent}
            onValueChange={async (value) => {
              try {
                await patchAppCategoryAsync({
                  id: params.row.id,
                  body: { name: value },
                });
              } catch {
                enqueueSnackbar(
                  t("messages.error.updateFailed", {
                    ns: "shared",
                    field: t("fields.name"),
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
      renderCell: (params: GridRenderCellParams<AppCategory, string>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <LazyColorField
            value={params.value}
            size="small"
            sx={{ width: "3rem" }}
            onValueChange={async (value) => {
              try {
                await patchAppCategoryAsync({
                  id: params.row.id,
                  body: { color: value },
                });
              } catch {
                enqueueSnackbar(
                  t("messages.error.updateFailed", {
                    ns: "shared",
                    field: t("fields.color"),
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
      renderCell: (params: GridRenderCellParams<AppCategory>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <AppCategoryIcon
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
      width: 200,
      renderCell: (params: GridRenderCellParams<AppCategory, string>) => (
        <Box sx={{ display: "flex", alignItems: "center", height: "100%" }}>
          <LazyTextField
            fullWidth
            size="small"
            value={params.value || ""}
            onKeyDown={stopGridKeyboardEvent}
            onValueChange={async (value) => {
              try {
                await patchAppCategoryAsync({
                  id: params.row.id,
                  body: { iconPath: value === "" ? null : value },
                });
              } catch {
                enqueueSnackbar(
                  t("messages.error.updateFailed", {
                    ns: "shared",
                    field: t("fields.iconPath"),
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
      field: "action",
      headerName: t("columns.action"),
      width: 60,
      sortable: false,
      filterable: false,
      disableColumnMenu: true,
      renderCell: (params: GridRenderCellParams<AppCategory>) => (
        <IconButton
          disabled={params.row.isSystem}
          color="error"
          onClick={() => {
            setAppCategoryToDelete(params.row);
            setDeleteAppCategoryComfirmDialogOpen(true);
          }}
        >
          <DeleteIcon />
        </IconButton>
      ),
    },
  ];

  const rows: GridRowsProp = appCategoriesData || [];

  return (
    <>
      <Box sx={{ display: "flex", justifyContent: "end" }}>
        <Button
          variant="contained"
          onClick={() => setCreateAppCategoryDialogOpen(true)}
        >
          {t("buttons.createCategory")}
        </Button>
      </Box>
      <Box sx={{ width: "100%", mt: 1 }}>
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
          loading={isLoading}
          rows={rows}
          columns={columns}
        />
      </Box>

      <Dialog
        open={deleteAppCategoryComfirmDialogOpen}
        onClose={() => setDeleteAppCategoryComfirmDialogOpen(false)}
        role="alertdialog"
      >
        <DialogTitle>
          {t("dialogs.delete.title", { name: appCategoryToDelete?.name })}
        </DialogTitle>
        <DialogContent>
          <DialogContentText>
            {t("dialogs.delete.description")}
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setDeleteAppCategoryComfirmDialogOpen(false)}
            autoFocus
          >
            {t("actions.cancel", { ns: "shared" })}
          </Button>
          <Button
            onClick={async () => {
              if (appCategoryToDelete == null) {
                enqueueSnackbar(t("messages.error.noCategorySelected"), {
                  variant: "error",
                });
                return;
              }
              try {
                await deleteAppCategoryAsync(appCategoryToDelete.id);
              } catch {
                enqueueSnackbar(
                  t("messages.error.deleteFailed", {
                    ns: "shared",
                    field: t("fields.category"),
                  }),
                  { variant: "error" },
                );
              } finally {
                setDeleteAppCategoryComfirmDialogOpen(false);
                setAppCategoryToDelete(null);
              }
            }}
            color="error"
          >
            {t("actions.confirm", { ns: "shared" })}
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={createAppCategoryDialogOpen}
        onClose={() => setCreateAppCategoryDialogOpen(false)}
      >
        <DialogTitle>{t("dialogs.create.title")}</DialogTitle>
        <DialogContent>
          <form
            onSubmit={async (event: React.SyntheticEvent<HTMLFormElement>) => {
              event.preventDefault();
              const formData = new FormData(event.currentTarget);
              const data = Object.fromEntries(formData.entries()) as {
                name: string;
                color: string;
                iconPath: string;
              };
              try {
                await createAppCategoryAsync(data);
                setCreateAppCategoryDialogOpen(false);
              } catch {
                enqueueSnackbar(
                  t("messages.error.createFailed", {
                    ns: "shared",
                    field: t("fields.category"),
                  }),
                  { variant: "error" },
                );
              }
            }}
            id="create-form"
          >
            <TextField
              autoFocus
              required
              margin="dense"
              variant="outlined"
              id="name"
              name="name"
              label={t("fields.name")}
              fullWidth
            />
            <TextField
              label={t("fields.color")}
              margin="dense"
              fullWidth
              slotProps={{
                htmlInput: {
                  type: "color",
                  name: "color",
                  defaultValue: "#1976d2",
                },
                inputLabel: {
                  shrink: true,
                },
              }}
            />
            <TextField
              margin="dense"
              variant="outlined"
              id="iconPath"
              name="iconPath"
              label={t("fields.iconPath")}
              fullWidth
            />
          </form>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateAppCategoryDialogOpen(false)}>
            {t("actions.cancel", { ns: "shared" })}
          </Button>
          <Button type="submit" form="create-form">
            {t("actions.create", { ns: "shared" })}
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
