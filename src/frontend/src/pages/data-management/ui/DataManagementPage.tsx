import { useDeleteData } from "@/entities/usage";
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
import { CalendarIcon } from "lucide-react";
import { useState } from "react";
import type { DateRange } from "react-day-picker";
import { toast } from "sonner";

export const DataManagementPage = () => {
  const { mutateAsync: deleteUsageDataAsync } = useDeleteData();
  const [date, setDate] = useState<DateRange | undefined>({
    from: new Date(new Date().getFullYear(), 0, 1),
    to: new Date(),
  });
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
    </>
  );
};
