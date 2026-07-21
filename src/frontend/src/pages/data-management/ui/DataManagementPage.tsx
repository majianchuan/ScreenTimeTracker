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
import { useTranslation } from "react-i18next";

export const DataManagementPage = () => {
  const { t } = useTranslation(["page_dataManagement", "shared"]);
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
      enqueueSnackbar(t("messages.unsupportedVersion"), { variant: "error" });
    } else {
      enqueueSnackbar(t("messages.importFailed"), { variant: "error" });
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
          <Typography>{t("labels.deletePrefix")}</Typography>
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
          <Typography>{t("common.to", { ns: "shared" })}</Typography>
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
          <Typography>{t("labels.deleteSuffix")}</Typography>
          <Button
            variant="contained"
            color="error"
            onClick={() => {
              if (
                deleteUsageDataStartDate === null ||
                deleteUsageDataEndDate === null
              ) {
                enqueueSnackbar(
                  t("validation.selectDateRange", { ns: "shared" }),
                  { variant: "error" },
                );
                return;
              }
              setDeleteUsageDataComfirmDialogOpen(true);
            }}
          >
            {t("actions.delete", { ns: "shared" })}
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
                    a.download = t("export.filenamePattern", {
                      date: dayjs(new Date()).format("YYYY-MM-DD"),
                    });
                    a.click();
                    URL.revokeObjectURL(url);

                    enqueueSnackbar(t("messages.exportFileSuccess"), {
                      variant: "success",
                    });
                  } catch {
                    enqueueSnackbar(t("messages.exportFileFailed"), {
                      variant: "error",
                    });
                  }
                }}
              >
                {t("labels.exportDataBtn")}
              </Button>
              <IconButton
                onClick={async () => {
                  try {
                    const content = await fetchExportContent();
                    await navigator.clipboard.writeText(content);
                    enqueueSnackbar(t("messages.exportClipboardSuccess"), {
                      variant: "success",
                    });
                  } catch {
                    enqueueSnackbar(t("messages.exportClipboardFailed"), {
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
                {t("labels.importDataBtn")}
                <input
                  hidden
                  type="file"
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (!file) return;
                    try {
                      const result = await importDataAsync(await file.text());
                      enqueueSnackbar(
                        t("messages.importFileSuccess", {
                          newApps: result.newApps,
                          newAppCategories: result.newAppCategories,
                          importedSessions: result.importedSessions,
                          skippedSessions: result.skippedSessions,
                        }),
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
                      t("messages.importClipboardSuccess", {
                        newApps: result.newApps,
                        newAppCategories: result.newAppCategories,
                        importedSessions: result.importedSessions,
                        skippedSessions: result.skippedSessions,
                      }),
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
          {t("dialogs.deleteConfirm.title", {
            startDate: deleteUsageDataStartDate?.toLocaleDateString(),
            endDate: deleteUsageDataEndDate?.toLocaleDateString(),
          })}
        </DialogTitle>
        <DialogActions>
          <Button
            onClick={() => setDeleteUsageDataComfirmDialogOpen(false)}
            autoFocus
          >
            {t("actions.cancel", { ns: "shared" })}
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
                enqueueSnackbar(t("messages.deleteSuccess"), {
                  variant: "success",
                });
              } catch {
                enqueueSnackbar(t("messages.deleteFailed"), {
                  variant: "error",
                });
              } finally {
                setDeleteUsageDataComfirmDialogOpen(false);
              }
            }}
            color="error"
          >
            {t("actions.confirm", { ns: "shared" })}
          </Button>
        </DialogActions>
      </Dialog>
    </Paper>
  );
};
