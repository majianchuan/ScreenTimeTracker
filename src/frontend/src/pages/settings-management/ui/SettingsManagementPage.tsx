import { useQuery } from "@tanstack/react-query";
import {
  FieldLegend,
  FieldSet,
  FieldSeparator,
  FieldGroup,
  FieldLabel,
  Field,
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
  Switch,
  Tooltip,
  TooltipContent,
  TooltipTrigger,
  AlertDialog,
  AlertDialogAction,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/shared/lib/shadcn";
import { CircleQuestionMark } from "lucide-react";
import { useState } from "react";
import {
  userSettingsQueries,
  usePatchuserSettings,
} from "@/entities/user-settings";
import {
  appSettingsQueries,
  usePatchAppSettings,
} from "@/entities/app-settings";
import { LazyInputText } from "@/shared/ui/LazyInputText";
import { LazyInputNumber } from "@/shared/ui/LazyInputNumber";

export const SettingsManagementPage = () => {
  const { data: userSettingsDtoData, isLoading: isuserSettingsDataLoading } =
    useQuery(userSettingsQueries.userSettings());
  const { data: appSettingsDtoData, isLoading: isAppSettingsDataLoading } =
    useQuery(appSettingsQueries.appSettings());
  const { mutateAsync: patchuserSettingsAsync } = usePatchuserSettings();
  const { mutateAsync: patchAppSettingsAsync } = usePatchAppSettings();
  const [autoStartAlertDialogOpen, setAutoStartAlertDialogOpen] =
    useState(false);

  if (isuserSettingsDataLoading || isAppSettingsDataLoading)
    return (
      <div className="flex h-full items-center justify-center">
        <span>正在获取数据，加载中。。。</span>
      </div>
    );

  if (!userSettingsDtoData || !appSettingsDtoData)
    return (
      <div className="flex h-full items-center justify-center">
        <span>无法获取数据，请检查网络</span>
      </div>
    );

  return (
    <>
      <FieldGroup>
        <FieldSet>
          <FieldLegend>系统</FieldLegend>
          <FieldGroup>
            {/* <Field orientation="horizontal">
              <FieldLabel>语言</FieldLabel>
            </Field> */}
            <Field orientation="horizontal">
              <FieldLabel>
                默认界面打开模式
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>点击托盘图标时界面的打开模式</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <Select
                value={appSettingsDtoData.defaultUIOpenMode}
                onValueChange={async (value) => {
                  await patchAppSettingsAsync({
                    defaultUIOpenMode: value as "Window" | "Browser",
                  });
                }}
              >
                <SelectTrigger>
                  <SelectValue />
                </SelectTrigger>
                <SelectContent position="popper">
                  <SelectGroup>
                    <SelectItem value="Window">窗口</SelectItem>
                    <SelectItem value="Browser">浏览器</SelectItem>
                  </SelectGroup>
                </SelectContent>
              </Select>
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                窗口关闭时销毁
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>界面以窗口模式打开时，点击关闭按钮时是否销毁窗口。</p>
                    <p>
                      如果销毁，下次打开时将稍慢但在窗口关闭时占用更少的内存和CPU。
                    </p>
                    <p>
                      如果不销毁，下次打开时将很快，但窗口关闭时占用更多的内存和CPU。
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <Switch
                checked={appSettingsDtoData.shouldDestroyWindowOnClose}
                onCheckedChange={async (value) => {
                  await patchAppSettingsAsync({
                    shouldDestroyWindowOnClose: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                开机自启动
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      如果程序是以管理员身份运行的，此时打开“开机自启动”将在下次开机时同样以管理员身份运行。否则将以普通用户身份运行。
                    </p>
                    <p>
                      修改可执行文件名或更改路径后开机自启动将失效，需要重新启用
                    </p>
                  </TooltipContent>
                </Tooltip>
                <AlertDialog
                  open={autoStartAlertDialogOpen}
                  onOpenChange={setAutoStartAlertDialogOpen}
                >
                  <AlertDialogContent>
                    <AlertDialogHeader>
                      <AlertDialogTitle>注意！</AlertDialogTitle>
                      <AlertDialogDescription>
                        开启后将写入修改系统配置，删除程序前请务必关闭，否则将导致残留！
                      </AlertDialogDescription>
                    </AlertDialogHeader>
                    <AlertDialogFooter>
                      <AlertDialogAction>好的</AlertDialogAction>
                    </AlertDialogFooter>
                  </AlertDialogContent>
                </AlertDialog>
              </FieldLabel>
              <Switch
                checked={appSettingsDtoData.isAutoStartEnabled}
                onCheckedChange={async (value) => {
                  if (value === true) setAutoStartAlertDialogOpen(true);

                  await patchAppSettingsAsync({
                    isAutoStartEnabled: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                静默启动
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>运行程序后不会自动打开界面。</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <Switch
                checked={appSettingsDtoData.isSilentStartEnabled}
                onCheckedChange={async (value) => {
                  await patchAppSettingsAsync({
                    isSilentStartEnabled: value,
                  });
                }}
              />
            </Field>
          </FieldGroup>
        </FieldSet>
        <FieldSeparator />
        <FieldSet>
          <FieldLegend>记录器</FieldLegend>
          <FieldGroup>
            <Field orientation="horizontal">
              <FieldLabel>
                应用图标文件夹
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      程序自动获取的应用图标将会保存在这里设定的文件夹路径下，修改后并不会改变已有图标的路径，只会影响新获取到的应用图标包括更新信息时获取的图标（如果图标有变化）。
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputText
                className="w-50"
                value={userSettingsDtoData.appIconDirectory}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    appIconDirectory: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                应用信息过期时间 (分钟)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      程序会在第一次遇到某应用时获取对应的可执行文件路径、图标、描述，下次遇到这个应用时，判断上次获取信息的时间距离现在是否超过了这里设定的过期时间，如果超过了则重新获取，否则不获取，保留旧信息。
                    </p>
                    <p>
                      注意：如果需要自定义应用图标，手动修改图标路径后一般关闭该应用的自动更新，否则更新时会覆盖自定义的图标路径。
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={0}
                step={1}
                value={userSettingsDtoData.appInfoStaleThresholdMinutes}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    appInfoStaleThresholdMinutes: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                活跃会话自动保存时间 (秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      自动保存当前活动应用使用会话的时间间隔，防止意外崩溃时数据丢失，确保屏幕时间统计准确。
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={1}
                step={1}
                value={userSettingsDtoData.activeSessionAutoSaveSeconds}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    activeSessionAutoSaveSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                空闲检测
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      程序会在用户没有操作键盘或鼠标超过设定的时间后认为已经空闲，并将从未操作键鼠开始直到再次操作键鼠之间的所有使用时长归为“Idle”应用的使用时长
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <Switch
                checked={userSettingsDtoData?.isIdleDetectionEnabled ?? false}
                onCheckedChange={async (value) => {
                  if (value === userSettingsDtoData?.isIdleDetectionEnabled)
                    return;
                  await patchuserSettingsAsync({
                    isIdleDetectionEnabled: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                空闲阈值 (秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      程序会在用户没有操作键盘或鼠标超过设定的时间后认为已经空闲，并将从未操作键鼠开始直到再次操作键鼠之间的所有使用时长归为“Idle”应用的使用时长
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={0}
                step={1}
                value={userSettingsDtoData.idleThresholdSeconds}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    idleThresholdSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                空闲检测轮询间隔 (秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>每隔这里设定的时间后，检测一次用户是否空闲。</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={1}
                step={1}
                value={userSettingsDtoData.idleDetectionPollingIntervalSeconds}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    idleDetectionPollingIntervalSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                最小有效会话时长 (秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      每次触发优化时，会自动过滤删除掉持续时间短于此值的会话记录
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={0}
                step={1}
                value={
                  userSettingsDtoData?.minValidSessionDurationSeconds ?? 60
                }
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    minValidSessionDurationSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                会话合并容差时间 (秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      每次触发优化时，会合并两段间隔小于这里值且为相同App的记录为一段长记录。
                    </p>
                    <p>注意：合并时无论中间是否有其他使用记录，都会被删除。</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={0}
                step={1}
                value={userSettingsDtoData?.sessionMergeToleranceSeconds ?? 60}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    sessionMergeToleranceSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                会话优化间隔 (秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>每个这里设定的时间后，会触发一次会话优化。</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={1}
                step={1}
                value={
                  userSettingsDtoData?.sessionOptimizationIntervalSeconds ?? 60
                }
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    sessionOptimizationIntervalSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                日期切换小时
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>定义了这一天从几点开始，到第二天几点结束</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <LazyInputNumber
                className="w-30"
                min={0}
                step={1}
                value={userSettingsDtoData?.dayCutoffHour ?? 60}
                onValueChange={async (value) => {
                  await patchuserSettingsAsync({
                    dayCutoffHour: value,
                  });
                }}
              />
            </Field>
          </FieldGroup>
        </FieldSet>
      </FieldGroup>
    </>
  );
};
