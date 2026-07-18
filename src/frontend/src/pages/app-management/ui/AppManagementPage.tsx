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
import { useState } from "react";
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
import { AppCategorySelecter } from "@/entities/app-category";
import { LazyTextField } from "@/shared/ui/LazyTextField";
import dayjs from "@/shared/lib/dayjs";
export const AppManagementPage = () => {
  const { enqueueSnackbar } = useSnackbar();
  const { data: appsData, isLoading } = useQuery(appQueries.apps({})) as {
    isLoading: boolean;
    data?: App[];
  };
  const { mutateAsync: patchAppAsync } = usePatchApp();
  const { mutateAsync: deleteAppAsync } = useDeleteApp();

  const [deleteAppComfirmDialogOpen, setDeleteAppComfirmDialogOpen] =
    useState(false);
  const [appToDelete, setAppToDelete] = useState<App | null>(null);

  const stopGridKeyboardEvent = (e: React.KeyboardEvent) => {
    e.stopPropagation();
  };

  const columns: GridColDef[] = [
    {
      field: "name",
      headerName: "名称",
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
                enqueueSnackbar("更新“颜色”失败", { variant: "error" });
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "appCategory",
      headerName: "应用类别",
      width: 210,
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
      headerName: "图标路径",
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
                enqueueSnackbar("更新“图标路径”失败", { variant: "error" });
              }
            }}
          />
        </Box>
      ),
    },
    {
      field: "isAutoUpdateEnabled",
      headerName: "自动更新",
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
              enqueueSnackbar("更新“自动更新”失败", { variant: "error" });
            }
          }}
        />
      ),
    },
    {
      field: "lastAutoUpdated",
      headerName: "上次自动更新时间",
      width: 140,
      valueFormatter: (value) => dayjs(value).format("L LT"),
    },
    {
      field: "processName",
      headerName: "进程名",
      width: 150,
    },
    {
      field: "executablePath",
      headerName: "可执行文件路径",
      width: 200,
    },
    {
      field: "action",
      headerName: "操作",
      width: 60,
      sortable: false,
      filterable: false,
      disableColumnMenu: true,
      renderCell: (params: GridRenderCellParams<App>) => (
        <>
          <IconButton
            color="error"
            onClick={() => {
              setAppToDelete(params.row);
              setDeleteAppComfirmDialogOpen(true);
            }}
          >
            <DeleteIcon />
          </IconButton>
        </>
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
          loading={isLoading}
          rows={rows}
          columns={columns}
        />
      </Box>

      <Dialog
        open={deleteAppComfirmDialogOpen}
        onClose={() => setDeleteAppComfirmDialogOpen(false)}
        role="alertdialog"
      >
        <DialogTitle>确定删除应用“{appToDelete?.name}”吗？</DialogTitle>
        <DialogContent>
          <DialogContentText id="alert-dialog-description">
            同时将删除所有该应用的已有数据。
          </DialogContentText>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setDeleteAppComfirmDialogOpen(false)}
            autoFocus
          >
            取消
          </Button>
          <Button
            onClick={async () => {
              if (appToDelete == null) {
                enqueueSnackbar("未选择要删除的应用", { variant: "error" });
                return;
              }
              try {
                await deleteAppAsync(appToDelete.id);
              } catch {
                enqueueSnackbar("删除应用失败", { variant: "error" });
              } finally {
                setDeleteAppComfirmDialogOpen(false);
                setAppToDelete(null);
              }
            }}
            color="error"
          >
            确定
          </Button>
        </DialogActions>
      </Dialog>
    </>
  );
};
