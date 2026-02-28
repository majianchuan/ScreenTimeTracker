import { useQuery } from "@tanstack/react-query";
import {
  screenTimeUserSettingsQueries,
  shellUserSettingsQueries,
  usePatchScreenTimeUserSettings,
  usePatchShellUserSettings,
} from "../api/queries";
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
import { BlurInput } from "@/shared/ui/BlurInput";
import { CircleQuestionMark } from "lucide-react";
import { useState } from "react";
import { useUsageConfig } from "@/entities/usage";

export const SettingsManagementPage = () => {
  const { data: screenTimeUserSettingsDtoData } = useQuery(
    screenTimeUserSettingsQueries.screenTimeUserSettings(),
  );
  const { data: shellUserSettingsDtoData } = useQuery(
    shellUserSettingsQueries.shellUserSettings(),
  );
  const { mutateAsync: patchScreenTimeUserSettingsAsync } =
    usePatchScreenTimeUserSettings();
  const { mutateAsync: patchShellUserSettingsAsync } =
    usePatchShellUserSettings();
  const [autoStartAlertDialogOpen, setAutoStartAlertDialogOpen] =
    useState(false);
  const { refetchIntervalSeconds, setRefetchIntervalSeconds } =
    useUsageConfig();

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
                界面打开模式
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
                value={shellUserSettingsDtoData?.uiOpenMode ?? "Window"}
                onValueChange={async (value) => {
                  if (!value) return;
                  await patchShellUserSettingsAsync({
                    uiOpenMode: value as "Window" | "Browser",
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
                checked={
                  shellUserSettingsDtoData?.windowDestroyOnClose ?? false
                }
                onCheckedChange={async (value) => {
                  if (value === shellUserSettingsDtoData?.windowDestroyOnClose)
                    return;
                  await patchShellUserSettingsAsync({
                    windowDestroyOnClose: value,
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
                      如果程序是以管理员身份运行的，此时打开“开机自自启动”将在下次开机时同样以管理员身份运行。否则将以普通用户身份运行。
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
                checked={shellUserSettingsDtoData?.autoStart ?? false}
                onCheckedChange={async (value) => {
                  if (value === shellUserSettingsDtoData?.autoStart) return;
                  if (value === true) {
                    setAutoStartAlertDialogOpen(true);
                  }
                  await patchShellUserSettingsAsync({
                    autoStart: value,
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
                checked={shellUserSettingsDtoData?.silentStart ?? false}
                onCheckedChange={async (value) => {
                  if (value === shellUserSettingsDtoData?.silentStart) return;
                  await patchShellUserSettingsAsync({
                    silentStart: value,
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
                采样间隔(毫秒)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      每隔这里设定的时间后获取一次顶层窗口对应的应用，并认为这段时间内一直使用的就是这个应用。
                    </p>
                    <p>
                      间隔越短统计结果越精确但对电脑资源占用越大，反正越不精确占用越小
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <BlurInput
                className="w-30"
                type="number"
                value={
                  screenTimeUserSettingsDtoData?.samplingIntervalMilliseconds ??
                  1000
                }
                onBlurUpdate={async (value: number) => {
                  if (!value) return;
                  await patchScreenTimeUserSettingsAsync({
                    samplingIntervalMilliseconds: value,
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
                checked={screenTimeUserSettingsDtoData?.idleDetection ?? false}
                onCheckedChange={async (value) => {
                  if (value === screenTimeUserSettingsDtoData?.idleDetection)
                    return;
                  await patchScreenTimeUserSettingsAsync({
                    idleDetection: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                空闲超时(秒)
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
              <BlurInput
                className="w-30"
                type="number"
                value={screenTimeUserSettingsDtoData?.idleTimeoutSeconds ?? 600}
                onBlurUpdate={async (value: number) => {
                  if (!value) return;
                  await patchScreenTimeUserSettingsAsync({
                    idleTimeoutSeconds: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                应用信息过期时间(分钟)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      程序会在第一次遇到某应用时获取对应的可执行文件路径、图标、描述，下次遇到这个应用时，判断上次获取信息的时间距离现在是否超过了这里设定的过期时间，如果超过了则重新获取，否则不获取，保留旧信息。
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <BlurInput
                className="w-30"
                type="number"
                value={
                  screenTimeUserSettingsDtoData?.appInfoStaleThresholdMinutes ??
                  1440
                }
                onBlurUpdate={async (value: number) => {
                  if (!value) return;
                  await patchScreenTimeUserSettingsAsync({
                    appInfoStaleThresholdMinutes: value,
                  });
                }}
              />
            </Field>
            <Field orientation="horizontal">
              <FieldLabel>
                数据聚合间隔(分钟)
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      每隔这里设定的时间后，把采样的数据聚合为更方便统计的数据，不建议修改。
                    </p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <BlurInput
                className="w-30"
                type="number"
                value={
                  screenTimeUserSettingsDtoData?.aggregationIntervalMinutes ??
                  60
                }
                onBlurUpdate={async (value: number) => {
                  if (!value) return;
                  await patchScreenTimeUserSettingsAsync({
                    aggregationIntervalMinutes: value,
                  });
                }}
              />
            </Field>
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
              <BlurInput
                className="w-50"
                value={screenTimeUserSettingsDtoData?.appIconDirectory ?? ""}
                onBlurUpdate={async (value: string) => {
                  if (!value) return;
                  await patchScreenTimeUserSettingsAsync({
                    appIconDirectory: value,
                  });
                }}
              />
            </Field>
          </FieldGroup>
        </FieldSet>
        <FieldSeparator />
        <FieldSet>
          <FieldLegend>界面</FieldLegend>
          <FieldGroup>
            <Field orientation="horizontal">
              <FieldLabel>
                使用数据重新获取间隔（秒）
                <Tooltip>
                  <TooltipTrigger>
                    <CircleQuestionMark className="size-4" />
                  </TooltipTrigger>
                  <TooltipContent>
                    <p>
                      每隔这里设定的时间后，界面会重新获取一次使用数据并展示。
                    </p>
                    <p>此配置保存不会在浏览器和窗口或不同浏览器之间同步。</p>
                  </TooltipContent>
                </Tooltip>
              </FieldLabel>
              <BlurInput
                className="w-50"
                type="number"
                value={refetchIntervalSeconds ?? 5}
                onBlurUpdate={async (value: number) => {
                  if (!value) return;
                  setRefetchIntervalSeconds(value);
                }}
              />
            </Field>
          </FieldGroup>
        </FieldSet>
      </FieldGroup>
    </>
  );
};
