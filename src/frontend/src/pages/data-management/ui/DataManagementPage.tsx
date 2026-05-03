import {
  exportDataQueryOptions,
  useDeleteData,
  useImportData,
} from "../api/queries";
import { dateToDateOnly } from "@/shared/lib/date-only";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
  Button,
  Calendar,
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/shared/lib/shadcn";
import { useQuery } from "@tanstack/react-query";
import axios from "axios";
import { CalendarIcon, CopyIcon, ClipboardIcon } from "lucide-react";
import { useState } from "react";
import type { DateRange } from "react-day-picker";
import { toast } from "sonner";
import { format } from "date-fns";

export const DataManagementPage = () => {
  const { mutateAsync: deleteUsageDataAsync } = useDeleteData();
  const [date, setDate] = useState<DateRange | undefined>({
    from: new Date(new Date().getFullYear(), 0, 1),
    to: new Date(),
  });
  const { refetch } = useQuery({
    ...exportDataQueryOptions(),
    enabled: false,
  });
  const { mutateAsync: importDataAsync } = useImportData();

  const handleImportError = (err: unknown) => {
    if (axios.isAxiosError(err) && err.response?.status === 422) {
      toast.error("配置版本不受支持，导入失败");
    } else {
      toast.error("导入数据失败");
    }
  };

  const fetchExportContent = async () => {
    const { data } = await refetch();
    return JSON.stringify(data, null, 4);
  };

  return (
    <>
      <div className="flex items-center gap-1">
        删除
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              id="date-picker-range"
              className="justify-start px-2.5 font-normal"
            >
              <CalendarIcon />
              {date?.from ? (
                date.to ? (
                  <>
                    {date.from.toLocaleDateString()} -{" "}
                    {date.to.toLocaleDateString()}
                  </>
                ) : (
                  date.from.toLocaleDateString()
                )
              ) : (
                <span>Pick a date</span>
              )}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start">
            <Calendar
              mode="range"
              defaultMonth={date?.from}
              selected={date}
              onSelect={setDate}
              numberOfMonths={2}
            />
          </PopoverContent>
        </Popover>
        的所有使用时间数据
        <AlertDialog>
          <AlertDialogTrigger asChild>
            <Button variant="destructive">删除</Button>
          </AlertDialogTrigger>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>
                你确定要删除{date?.from?.toLocaleDateString()}至
                {date?.to?.toLocaleDateString()}的所有使用时间数据吗？
              </AlertDialogTitle>
              <AlertDialogDescription>
                不包括应用数据、类别数据和用户配置。
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>取消</AlertDialogCancel>
              <AlertDialogAction
                onClick={async () => {
                  if (!date?.from || !date?.to) {
                    toast.error("无效的日期范围");
                    return;
                  }
                  try {
                    await deleteUsageDataAsync({
                      startDate: dateToDateOnly(date.from),
                      endDate: dateToDateOnly(date.to),
                    });
                    toast.success("删除使用时间数据成功");
                  } catch {
                    toast.error("删除使用时间数据失败");
                  }
                }}
              >
                删除
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>
      <div className="mt-2 flex gap-3">
        <div className="flex gap-1">
          <Button
            variant="outline"
            onClick={async () => {
              try {
                const content = await fetchExportContent();
                const blob = new Blob([content], { type: "application/json" });
                const url = URL.createObjectURL(blob);
                const a = document.createElement("a");
                a.href = url;
                a.download = `screen-time-tracker-data-${format(new Date(), "yyyy-MM-dd")}.json`;
                a.click();
                URL.revokeObjectURL(url);

                toast.success("导出数据到文件成功");
              } catch {
                toast.error("导出数据到文件失败");
              }
            }}
          >
            导出数据
          </Button>
          <Button
            variant="outline"
            size="icon"
            onClick={async () => {
              try {
                const content = await fetchExportContent();
                await navigator.clipboard.writeText(content);
                toast.success("导出数据到剪贴板成功");
              } catch {
                toast.error("导出数据到剪贴板失败");
              }
            }}
          >
            <CopyIcon />
          </Button>
        </div>
        <div className="flex gap-1">
          <Button asChild variant="outline">
            <label>
              导入数据
              <input
                type="file"
                accept=".json"
                className="hidden"
                onChange={async (e) => {
                  const file = e.target.files?.[0];
                  if (!file) return;
                  try {
                    const result = await importDataAsync(await file.text());
                    toast.success(
                      `从文件导入数据成功，导入${result.importedCount}条，跳过${result.skippedCount}条`,
                    );
                  } catch (err: unknown) {
                    handleImportError(err);
                  } finally {
                    e.target.value = "";
                  }
                }}
              />
            </label>
          </Button>
          <Button
            variant="outline"
            onClick={async () => {
              try {
                const result = await importDataAsync(
                  await navigator.clipboard.readText(),
                );
                toast.success(
                  `从剪贴板导入数据成功，导入${result.importedCount}条，跳过${result.skippedCount}条`,
                );
              } catch (err: unknown) {
                handleImportError(err);
              }
            }}
          >
            <ClipboardIcon />
          </Button>
        </div>
      </div>
    </>
  );
};
