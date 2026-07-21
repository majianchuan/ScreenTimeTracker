import {
  appSettingsQueries,
  usePatchAppSettings,
} from "@/entities/app-settings";
import {
  usePatchUserSettings,
  userSettingsQueries,
} from "@/entities/user-settings";
import Box from "@mui/material/Box";
import CircularProgress from "@mui/material/CircularProgress";
import IconButton from "@mui/material/IconButton";
import MenuItem from "@mui/material/MenuItem";
import Paper from "@mui/material/Paper";
import Select, { type SelectChangeEvent } from "@mui/material/Select";
import Stack from "@mui/material/Stack";
import Tooltip from "@mui/material/Tooltip";
import Typography from "@mui/material/Typography";
import Switch from "@mui/material/Switch";
import { useQuery } from "@tanstack/react-query";
import { useState } from "react";
import HelpIcon from "@mui/icons-material/Help";
import Dialog from "@mui/material/Dialog";
import DialogTitle from "@mui/material/DialogTitle";
import DialogActions from "@mui/material/DialogActions";
import Button from "@mui/material/Button";
import { LazyTextField } from "@/shared/ui/LazyTextField";
import { LazyNumberField } from "@/shared/ui/LazyNumberField";
import { SUPPORTED_LANGUAGES, type LanguageCode } from "@/shared/i18n";
import { useTranslation } from "react-i18next";

export const SettingsManagementPage = () => {
  const { t } = useTranslation(["page_settingsManagement", "shared"]);
  const { data: userSettingsDtoData, isLoading: isuserSettingsDataLoading } =
    useQuery(userSettingsQueries.userSettings());
  const { data: appSettingsDtoData, isLoading: isAppSettingsDataLoading } =
    useQuery(appSettingsQueries.appSettings());
  const { mutateAsync: patchUserSettingsAsync } = usePatchUserSettings();
  const { mutateAsync: patchAppSettingsAsync } = usePatchAppSettings();
  const [autoStartAlertDialogOpen, setAutoStartAlertDialogOpen] =
    useState(false);

  if (isuserSettingsDataLoading || isAppSettingsDataLoading)
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
        }}
      >
        <CircularProgress />
      </Box>
    );

  if (!userSettingsDtoData || !appSettingsDtoData)
    return (
      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
        }}
      >
        <Typography>{t("errors.fetchFailed", { ns: "shared" })}</Typography>
      </Box>
    );

  return (
    <Stack spacing={2} direction="column">
      <Paper
        variant="outlined"
        sx={{
          p: 2,
        }}
      >
        <Stack spacing={1} direction="column">
          <Typography sx={{ fontWeight: "bold" }}>
            {t("appSettings.title")}
          </Typography>
          {/* 语言设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Typography>{t("appSettings.language.label")}</Typography>
            <Select
              size="small"
              value={appSettingsDtoData.language}
              onChange={async (event: SelectChangeEvent<string>) => {
                await patchAppSettingsAsync({
                  language: event.target.value as LanguageCode,
                });
              }}
            >
              {SUPPORTED_LANGUAGES.map((language) => (
                <MenuItem value={language.code}>{language.label}</MenuItem>
              ))}
            </Select>
          </Stack>
          {/* 打开模式设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("appSettings.defaultUIOpenMode.label")}
              </Typography>
              <Tooltip title={t("appSettings.defaultUIOpenMode.tooltip")}>
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <Select
              size="small"
              value={appSettingsDtoData.defaultUIOpenMode}
              onChange={async (event: SelectChangeEvent<string>) => {
                await patchAppSettingsAsync({
                  defaultUIOpenMode: event.target.value as "Window" | "Browser",
                });
              }}
            >
              <MenuItem value="Window">
                {" "}
                {t("appSettings.defaultUIOpenMode.options.window")}
              </MenuItem>
              <MenuItem value="Browser">
                {t("appSettings.defaultUIOpenMode.options.browser")}
              </MenuItem>
            </Select>
          </Stack>
          {/* 开机启动设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("appSettings.isAutoStartEnabled.label")}
              </Typography>
              <Tooltip title={t("appSettings.isAutoStartEnabled.tooltip")}>
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <Switch
              checked={appSettingsDtoData.isAutoStartEnabled}
              onChange={async (event: React.ChangeEvent<HTMLInputElement>) => {
                const {
                  target: { checked },
                } = event;
                if (checked === true) setAutoStartAlertDialogOpen(true);

                await patchAppSettingsAsync({
                  isAutoStartEnabled: checked,
                });
              }}
            />
          </Stack>
          {/* 开机启动警告 Dialog */}
          <Dialog
            open={autoStartAlertDialogOpen}
            onClose={() => setAutoStartAlertDialogOpen(false)}
            role="alertdialog"
          >
            <DialogTitle>{t("appSettings.autoStartAlert.title")}</DialogTitle>
            <DialogActions>
              <Button
                onClick={() => {
                  setAutoStartAlertDialogOpen(false);
                }}
              >
                {t("actions.confirm", { ns: "shared" })}
              </Button>
            </DialogActions>
          </Dialog>
          {/* 静默启动设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("appSettings.isSilentStartEnabled.label")}
              </Typography>
              <Tooltip title={t("appSettings.isSilentStartEnabled.tooltip")}>
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <Switch
              checked={appSettingsDtoData.isSilentStartEnabled}
              onChange={async (event: React.ChangeEvent<HTMLInputElement>) => {
                await patchAppSettingsAsync({
                  isSilentStartEnabled: event.target.checked,
                });
              }}
            />
          </Stack>
        </Stack>
      </Paper>
      <Paper
        variant="outlined"
        sx={{
          p: 2,
        }}
      >
        <Stack spacing={1} direction="column">
          <Typography sx={{ fontWeight: "bold" }}>
            {t("screenTimeSettings.title")}
          </Typography>
          {/* 应用图标目录设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.appIconDirectory.label")}
              </Typography>
              <Tooltip title={t("screenTimeSettings.appIconDirectory.tooltip")}>
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyTextField
              size="small"
              value={userSettingsDtoData.appIconDirectory}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  appIconDirectory: value,
                });
              }}
            />
          </Stack>
          {/* 应用信息过期阈值设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.appInfoStaleThresholdMinutes.label")}
              </Typography>
              <Tooltip
                title={t(
                  "screenTimeSettings.appInfoStaleThresholdMinutes.tooltip",
                )}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.appInfoStaleThresholdMinutes}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  appInfoStaleThresholdMinutes: value,
                });
              }}
              min={0}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 活动会话自动保存设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.activeSessionAutoSaveSeconds.label")}
              </Typography>
              <Tooltip
                title={t(
                  "screenTimeSettings.activeSessionAutoSaveSeconds.tooltip",
                )}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.activeSessionAutoSaveSeconds}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  activeSessionAutoSaveSeconds: value,
                });
              }}
              min={1}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 空闲检测设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.isIdleDetectionEnabled.label")}
              </Typography>
              <Tooltip
                title={t("screenTimeSettings.isIdleDetectionEnabled.tooltip")}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <Switch
              checked={userSettingsDtoData.isIdleDetectionEnabled}
              onChange={async (event: React.ChangeEvent<HTMLInputElement>) => {
                await patchUserSettingsAsync({
                  isIdleDetectionEnabled: event.target.checked,
                });
              }}
            />
          </Stack>
          {/* 空闲阈值设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.idleThresholdSeconds.label")}
              </Typography>
              <Tooltip
                title={t("screenTimeSettings.idleThresholdSeconds.tooltip")}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.idleThresholdSeconds}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  idleThresholdSeconds: value,
                });
              }}
              min={1}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 空闲检测轮询间隔设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t(
                  "screenTimeSettings.idleDetectionPollingIntervalSeconds.label",
                )}
              </Typography>
              <Tooltip
                title={t(
                  "screenTimeSettings.idleDetectionPollingIntervalSeconds.tooltip",
                )}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.idleDetectionPollingIntervalSeconds}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  idleDetectionPollingIntervalSeconds: value,
                });
              }}
              min={1}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 最小有效会话时长设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.minValidSessionDurationSeconds.label")}
              </Typography>
              <Tooltip
                title={t(
                  "screenTimeSettings.minValidSessionDurationSeconds.tooltip",
                )}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.minValidSessionDurationSeconds}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  minValidSessionDurationSeconds: value,
                });
              }}
              min={0}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 会话合并容差时间设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.sessionMergeToleranceSeconds.label")}
              </Typography>
              <Tooltip
                title={t(
                  "screenTimeSettings.sessionMergeToleranceSeconds.tooltip",
                )}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.sessionMergeToleranceSeconds}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  sessionMergeToleranceSeconds: value,
                });
              }}
              min={0}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 会话优化间隔设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t(
                  "screenTimeSettings.sessionOptimizationIntervalMinutes.label",
                )}
              </Typography>
              <Tooltip
                title={t(
                  "screenTimeSettings.sessionOptimizationIntervalMinutes.tooltip",
                )}
              >
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.sessionOptimizationIntervalMinutes}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  sessionOptimizationIntervalMinutes: value,
                });
              }}
              min={1}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
          {/* 日期切换小时设置 */}
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>
                {t("screenTimeSettings.dayCutoffHour.label")}
              </Typography>
              <Tooltip title={t("screenTimeSettings.dayCutoffHour.tooltip")}>
                <IconButton size="small">
                  <HelpIcon fontSize="inherit" />
                </IconButton>
              </Tooltip>
            </Stack>
            <LazyNumberField
              size="small"
              value={userSettingsDtoData.dayCutoffHour}
              onValueChange={async (value) => {
                await patchUserSettingsAsync({
                  dayCutoffHour: value,
                });
              }}
              min={0}
              allowDecimal={false}
              allowEmpty={false}
            />
          </Stack>
        </Stack>
      </Paper>
    </Stack>
  );
};
