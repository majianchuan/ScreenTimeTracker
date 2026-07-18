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

export const SettingsManagementPage = () => {
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
        <Typography>无法获取数据，请检查网络</Typography>
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
          <Typography sx={{ fontWeight: "bold" }}>应用设置</Typography>
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>默认界面打开模式</Typography>
              <Tooltip title="点击托盘图标时界面的打开模式">
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
              <MenuItem value="Window">窗口</MenuItem>
              <MenuItem value="Browser">浏览器</MenuItem>
            </Select>
          </Stack>
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>开机自启动</Typography>
              <Tooltip
                title={
                  <>
                    如果程序是以管理员身份运行的，此时打开“开机自启动”将在下次开机时同样以管理员身份启动。否则将以普通用户身份启动。
                    <br />
                    修改可执行文件名或更改路径后开机自启动将失效，需要重新关闭再启用
                  </>
                }
              >
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>静默启动</Typography>
              <Tooltip title="运行程序后不会自动打开界面。">
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

        <Dialog
          open={autoStartAlertDialogOpen}
          onClose={() => setAutoStartAlertDialogOpen(false)}
          role="alertdialog"
        >
          <DialogTitle>
            开启后将写入修改系统配置，删除程序前请务必关闭，否则将导致残留！
          </DialogTitle>
          <DialogActions>
            <Button
              onClick={() => {
                setAutoStartAlertDialogOpen(false);
              }}
            >
              确定
            </Button>
          </DialogActions>
        </Dialog>
      </Paper>
      <Paper
        variant="outlined"
        sx={{
          p: 2,
        }}
      >
        <Stack spacing={1} direction="column">
          <Typography sx={{ fontWeight: "bold" }}>屏幕使用时间设置</Typography>
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>应用图标文件夹</Typography>
              <Tooltip title="程序自动获取的应用图标将会保存在这里设定的文件夹路径下，修改后并不会改变已有图标的路径，只会影响新获取到的应用图标包括更新信息时获取的图标（如果图标有变化）。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>应用信息过期时间 (分钟)</Typography>
              <Tooltip
                title={
                  <>
                    程序会在第一次遇到某应用时获取对应的可执行文件路径、图标、描述，下次遇到这个应用时，判断上次获取信息的时间距离现在是否超过了这里设定的过期时间，如果超过了则重新获取，否则不获取，保留旧信息。
                    <br />
                    注意：如果需要自定义应用图标，手动修改图标路径后一般关闭该应用的自动更新，否则更新时会覆盖自定义的图标路径。
                  </>
                }
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>活跃会话自动保存时间 (秒)</Typography>
              <Tooltip title="自动保存当前活动应用使用会话的时间间隔，防止意外崩溃时数据丢失，确保屏幕时间统计准确。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>空闲检测</Typography>
              <Tooltip title="程序会在用户没有操作键盘或鼠标超过设定的时间后认为已经空闲，并将从未操作键鼠开始直到再次操作键鼠之间的所有使用时长归为“Idle”应用的使用时长。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>空闲阈值 (秒)</Typography>
              <Tooltip title="程序会在用户没有操作键盘或鼠标超过设定的时间后认为已经空闲，并将从未操作键鼠开始直到再次操作键鼠之间的所有使用时长归为“Idle”应用的使用时长。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>空闲检测轮询间隔 (秒)</Typography>
              <Tooltip title="每隔这里设定的时间后，检测一次用户是否空闲。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>最小有效会话时长 (秒)</Typography>
              <Tooltip title="每次触发优化时，会自动过滤删除掉持续时间短于此值的会话记录。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>会话合并容差时间 (秒)</Typography>
              <Tooltip
                title={
                  <>
                    每次触发优化时，会合并两段间隔小于这里值且为相同App的记录为一段长记录。
                    <br />
                    注意：合并时无论中间是否有其他使用记录，都会被删除。
                  </>
                }
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>会话优化间隔 (分钟)</Typography>
              <Tooltip title="每个这里设定的时间后，会触发一次会话优化。">
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
          <Stack
            direction="row"
            sx={{
              alignItems: "center",
              justifyContent: "space-between",
            }}
          >
            <Stack direction="row" sx={{ alignItems: "center" }}>
              <Typography>日期切换小时</Typography>
              <Tooltip title="定义了这一天从几点开始，到第二天几点结束。">
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
