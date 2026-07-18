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

export const AppCategoryManagementPage = () => {
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
      headerName: "名称",
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
                enqueueSnackbar("更新“名称”失败", { variant: "error" });
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "color",
      headerName: "颜色",
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
                enqueueSnackbar("更新“颜色”失败", { variant: "error" });
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "icon",
      headerName: "图标",
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
      headerName: "图标路径",
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
                enqueueSnackbar("更新“图标路径”失败", { variant: "error" });
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "action",
      headerName: "操作",
      width: 60,
      sortable: false,
      filterable: false,
      disableColumnMenu: true,
      renderCell: (params: GridRenderCellParams<AppCategory>) => (
        <>
          <IconButton
            color="error"
            onClick={() => {
              setAppCategoryToDelete(params.row);
              setDeleteAppCategoryComfirmDialogOpen(true);
            }}
          >
            <DeleteIcon />
          </IconButton>
        </>
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
          创建类别
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
        <DialogTitle>确定删除类别{appCategoryToDelete?.name}吗？</DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            属于该类别的应用将变为默认类别。
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setDeleteAppCategoryComfirmDialogOpen(false)}
            autoFocus
          >
            取消
          </Button>
          <Button
            onClick={async () => {
              if (appCategoryToDelete == null) {
                enqueueSnackbar("未选择要删除的类别", { variant: "error" });
                return;
              }
              try {
                await deleteAppCategoryAsync(appCategoryToDelete.id);
              } catch {
                enqueueSnackbar("删除类别失败", { variant: "error" });
              } finally {
                setDeleteAppCategoryComfirmDialogOpen(false);
                setAppCategoryToDelete(null);
              }
            }}
            color="error"
          >
            确定
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog
        open={createAppCategoryDialogOpen}
        onClose={() => setCreateAppCategoryDialogOpen(false)}
      >
        <DialogTitle>创建类别</DialogTitle>
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
                enqueueSnackbar("创建类别失败", { variant: "error" });
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
              label="名称"
              fullWidth
            />
            <TextField
              label="颜色"
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
              label="图标路径"
              fullWidth
            />
          </form>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateAppCategoryDialogOpen(false)}>
            取消
          </Button>
          <Button type="submit" form="create-form">
            创建
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
