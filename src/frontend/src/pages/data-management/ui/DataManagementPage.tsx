import Paper from "@mui/material/Paper";
import {
  exportDataQueryOptions,
  useDeleteData,
  useImportData,
} from "../api/queries";
import { dateToDateOnly } from "@/shared/lib/date-only";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { useState } from "react";
import Typography from "@mui/material/Typography";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import Dialog from "@mui/material/Dialog";
import DialogActions from "@mui/material/DialogActions";
import DialogTitle from "@mui/material/DialogTitle";
import { useSnackbar } from "notistack";
import dayjs from "@/shared/lib/dayjs";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import IconButton from "@mui/material/IconButton";
import ContentPasteIcon from "@mui/icons-material/ContentPaste";
import Stack from "@mui/material/Stack";

export const DataManagementPage = () => {
  const { enqueueSnackbar } = useSnackbar();
  const { mutateAsync: deleteUsageDataAsync } = useDeleteData();
  const { refetch } = useQuery({
    ...exportDataQueryOptions(),
    enabled: false,
  });
  const { mutateAsync: importDataAsync } = useImportData();

  const [deleteUsageDataStartDate, setDeleteUsageDataStartDate] =
    useState<Date | null>(new Date(new Date().getFullYear(), 0, 1));
  const [deleteUsageDataEndDate, setDeleteUsageDataEndDate] =
    useState<Date | null>(new Date());
  const [
    deleteUsageDataComfirmDialogOpen,
    setDeleteUsageDataComfirmDialogOpen,
  ] = useState(false);

  const handleImportError = (err: unknown) => {
    if (axios.isAxiosError(err) && err.response?.status === 422) {
      enqueueSnackbar("配置版本不受支持，导入失败", { variant: "error" });
    } else {
      enqueueSnackbar("导入数据失败", { variant: "success" });
    }
  };

  const fetchExportContent = async () => {
    const { data } = await refetch();
    return JSON.stringify(data, null, 4);
  };

  return (
    <Paper
      variant="outlined"
      sx={{
        p: 2,
      }}
    >
      <Stack spacing={2}>
        <Stack
          direction="row"
          spacing={1}
          sx={{
            alignItems: "center",
          }}
        >
          <Typography>删除</Typography>
          <DatePicker
            sx={{ width: "10rem" }}
            value={dayjs(deleteUsageDataStartDate)}
            onChange={(newdate) =>
              setDeleteUsageDataStartDate(newdate?.toDate() || null)
            }
            slotProps={{
              textField: {
                size: "small",
              },
            }}
          />
          <Typography>到</Typography>
          <DatePicker
            sx={{ width: "10rem" }}
            value={dayjs(deleteUsageDataEndDate)}
            onChange={(newdate) =>
              setDeleteUsageDataEndDate(newdate?.toDate() || null)
            }
            slotProps={{
              textField: {
                size: "small",
              },
            }}
          />
          <Typography>的所有使用时间数据</Typography>
          <Button
            variant="contained"
            color="error"
            onClick={() => {
              if (
                deleteUsageDataStartDate === null ||
                deleteUsageDataEndDate === null
              ) {
                enqueueSnackbar("请选择日期范围", { variant: "error" });
                return;
              }
              setDeleteUsageDataComfirmDialogOpen(true);
            }}
          >
            删除
          </Button>
        </Stack>

        <Box>
          <Stack direction="row" spacing={2}>
            <Stack direction="row" spacing={1}>
              <Button
                variant="contained"
                onClick={async () => {
                  try {
                    const content = await fetchExportContent();
                    const blob = new Blob([content], {
                      type: "application/json",
                    });
                    const url = URL.createObjectURL(blob);
                    const a = document.createElement("a");
                    a.href = url;
                    a.download = `screen-time-tracker-data-${dayjs(new Date()).format("YYYY-MM-DD")}.json`;
                    a.click();
                    URL.revokeObjectURL(url);

                    enqueueSnackbar("导出数据到文件成功", {
                      variant: "success",
                    });
                  } catch {
                    enqueueSnackbar("导出数据到文件失败", { variant: "error" });
                  }
                }}
              >
                导出数据
              </Button>
              <IconButton
                onClick={async () => {
                  try {
                    const content = await fetchExportContent();
                    await navigator.clipboard.writeText(content);
                    enqueueSnackbar("导出数据到剪贴板成功", {
                      variant: "success",
                    });
                  } catch {
                    enqueueSnackbar("导出数据到剪贴板失败", {
                      variant: "error",
                    });
                  }
                }}
              >
                <ContentCopyIcon />
              </IconButton>
            </Stack>
            <Stack direction="row" spacing={1}>
              <Button component="label" variant="contained">
                导入数据
                <input
                  hidden
                  type="file"
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (!file) return;
                    try {
                      const result = await importDataAsync(await file.text());
                      enqueueSnackbar(
                        `从文件导入数据成功。新增应用${result.newApps}个，类别${result.newAppCategories}个；导入会话${result.importedSessions}条，跳过${result.skippedSessions}条`,
                        { variant: "success" },
                      );
                    } catch (err: unknown) {
                      handleImportError(err);
                    } finally {
                      e.target.value = "";
                    }
                  }}
                />
              </Button>
              <IconButton
                sx={{ ml: 1 }}
                onClick={async () => {
                  try {
                    const result = await importDataAsync(
                      await navigator.clipboard.readText(),
                    );
                    enqueueSnackbar(
                      `从剪贴板导入数据成功。新增应用${result.newApps}个，类别${result.newAppCategories}个；导入会话${result.importedSessions}条，跳过${result.skippedSessions}条`,
                      { variant: "success" },
                    );
                  } catch (err: unknown) {
                    handleImportError(err);
                  }
                }}
              >
                <ContentPasteIcon />
              </IconButton>
            </Stack>
          </Stack>
        </Box>
      </Stack>

      <Dialog
        open={deleteUsageDataComfirmDialogOpen}
        onClose={() => setDeleteUsageDataComfirmDialogOpen(false)}
        role="alertdialog"
      >
        <DialogTitle>
          确定删除 {deleteUsageDataStartDate?.toLocaleDateString()} 至{" "}
          {deleteUsageDataEndDate?.toLocaleDateString()} 的所有使用时间数据吗？
        </DialogTitle>
        <DialogActions>
          <Button
            onClick={() => setDeleteUsageDataComfirmDialogOpen(false)}
            autoFocus
          >
            取消
          </Button>
          <Button
            onClick={async () => {
              if (
                deleteUsageDataStartDate === null ||
                deleteUsageDataEndDate === null
              )
                return;
              try {
                await deleteUsageDataAsync({
                  startDate: dateToDateOnly(deleteUsageDataStartDate),
                  endDate: dateToDateOnly(deleteUsageDataEndDate),
                });
                enqueueSnackbar("删除使用时间数据成功", { variant: "success" });
              } catch {
                enqueueSnackbar("删除使用时间数据失败", { variant: "error" });
              } finally {
                setDeleteUsageDataComfirmDialogOpen(false);
              }
            }}
            color="error"
          >
            确定
          </Button>
        </DialogActions>
      </Dialog>
    </Paper>
  );
};
