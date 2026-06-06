import {
  AppCategoryIcon,
  appCategoryQueries,
  useCreateAppCategory,
  useDeleteAppCategory,
  usePatchAppCategory,
  type AppCategory,
} from "@/entities/app-category";
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
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  Field,
  FieldGroup,
  FieldLabel,
  Input,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/lib/shadcn";
import { useQuery } from "@tanstack/react-query";
import {
  type ColumnDef,
  flexRender,
  getCoreRowModel,
  useReactTable,
} from "@tanstack/react-table";
import { Check, Trash, X } from "lucide-react";
import { toast } from "sonner";
import { useForm } from "react-hook-form";
import { useState } from "react";
import { LazyInputText } from "@/shared/ui/LazyInputText";
import { LazyInputColor } from "@/shared/ui/LazyInputColor";

export const AppCategoryManagementPage = () => {
  const { data: appCategoriesData } = useQuery(
    appCategoryQueries.appCategories({}),
  ) as {
    data?: AppCategory[];
  };
  const { mutateAsync: createAppCategoryAsync } = useCreateAppCategory();
  const { mutateAsync: patchAppCategoryAsync } = usePatchAppCategory();
  const { mutateAsync: deleteAppCategoryAsync } = useDeleteAppCategory();

  const columns: ColumnDef<AppCategory>[] = [
    {
      accessorKey: "name",
      header: "名称",
      size: 200,
      cell: ({ row }) => {
        const appCategory = row.original;
        return (
          <LazyInputText
            value={appCategory.name}
            onValueChange={async (value) => {
              try {
                await patchAppCategoryAsync({
                  id: appCategory.id,
                  body: { name: value },
                });
                toast.success("类别名称更新成功");
              } catch {
                toast.error("更新类别名称失败");
              }
            }}
          />
        );
      },
    },
    {
      id: "color",
      header: "颜色",
      size: 45,
      cell: ({ row }) => {
        const app = row.original;
        return (
          <LazyInputColor
            value={app.color}
            style={{ width: "25px" }}
            onValueChange={async (color) => {
              try {
                await patchAppCategoryAsync({
                  id: app.id,
                  body: { color: color },
                });
                toast.success("更新颜色成功");
              } catch {
                toast.error("更新颜色失败");
              }
            }}
          />
        );
      },
    },
    {
      id: "icon",
      header: "图标",
      size: 45,
      cell: ({ row }) => {
        const appCategory = row.original;
        return (
          <AppCategoryIcon
            className="size-7"
            id={appCategory.id}
            iconPath={appCategory.iconPath}
          />
        );
      },
    },
    {
      accessorKey: "iconPath",
      header: "图标路径",
      size: 400,
      cell: ({ row }) => {
        const appCategory = row.original;
        return (
          <LazyInputText
            value={appCategory.iconPath !== null ? appCategory.iconPath : ""}
            onValueChange={async (value) => {
              try {
                await patchAppCategoryAsync({
                  id: appCategory.id,
                  body: { iconPath: value !== "" ? value : null },
                });
                toast.success("图标路径更新成功");
              } catch {
                toast.error("更新图标路径失败");
              }
            }}
          />
        );
      },
    },
    {
      accessorKey: "isSystem",
      header: "系统类别",
      size: 75,
      cell: ({ row }) => {
        return row.getValue("isSystem") ? (
          <Check className="size-5" />
        ) : (
          <X className="size-5" />
        );
      },
    },
    {
      id: "actions",
      header: "操作",
      size: 60,
      cell: ({ row }) => {
        const appCategory = row.original;
        return (
          <AlertDialog>
            <AlertDialogTrigger asChild>
              <Button variant="destructive" disabled={appCategory.isSystem}>
                <Trash />
              </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>
                  你确定要删除类别“{appCategory.name}”吗？
                </AlertDialogTitle>
                <AlertDialogDescription>
                  属于该类别的应用将变为默认类别。
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>取消</AlertDialogCancel>
                <AlertDialogAction
                  onClick={async () => {
                    try {
                      await deleteAppCategoryAsync(appCategory.id);
                      toast.success("类别删除成功");
                    } catch {
                      toast.error("删除类别失败");
                    }
                  }}
                >
                  删除
                </AlertDialogAction>
              </AlertDialogFooter>
            </AlertDialogContent>
          </AlertDialog>
        );
      },
    },
  ];

  const table = useReactTable({
    data: appCategoriesData ?? [],
    columns,
    columnResizeMode: "onChange",
    autoResetPageIndex: false,
    getCoreRowModel: getCoreRowModel(),
  });

  const [isCreateAppCategoryDialogOpen, setIsCreateAppCategoryDialogOpen] =
    useState(false);
  type CreateAppCategoryInputs = {
    name: string;
    color: string;
    iconPath: string;
  };
  const { register, handleSubmit, reset, setValue } =
    useForm<CreateAppCategoryInputs>();

  const handleCreateDialogOpen = (open: boolean) => {
    setIsCreateAppCategoryDialogOpen(open);

    if (open) {
      setValue("color", generateColor());
    }
  };

  function hslToHex(h: number, s: number, l: number): string {
    s /= 100;
    l /= 100;

    const c = (1 - Math.abs(2 * l - 1)) * s;
    const x = c * (1 - Math.abs(((h / 60) % 2) - 1));
    const m = l - c / 2;

    let r = 0;
    let g = 0;
    let b = 0;

    if (h < 60) {
      r = c;
      g = x;
    } else if (h < 120) {
      r = x;
      g = c;
    } else if (h < 180) {
      g = c;
      b = x;
    } else if (h < 240) {
      g = x;
      b = c;
    } else if (h < 300) {
      r = x;
      b = c;
    } else {
      r = c;
      b = x;
    }

    const toHex = (n: number) =>
      Math.round((n + m) * 255)
        .toString(16)
        .padStart(2, "0");

    return `#${toHex(r)}${toHex(g)}${toHex(b)}`;
  }

  function generateColor(): string {
    const h = Math.floor(Math.random() * 360);
    const s = 60 + Math.floor(Math.random() * 40); // 60~99
    const l = 50 + Math.floor(Math.random() * 30); // 50~79

    return hslToHex(h, s, l);
  }

  return (
    <>
      <div className="flex flex-row justify-end">
        <Dialog
          open={isCreateAppCategoryDialogOpen}
          onOpenChange={handleCreateDialogOpen}
        >
          <DialogTrigger asChild>
            <Button variant="outline">创建类别</Button>
          </DialogTrigger>
          <DialogContent className="sm:max-w-sm">
            <DialogHeader>
              <DialogTitle>创建类别</DialogTitle>
              <DialogDescription />
            </DialogHeader>
            <form
              id="create-app-category-form"
              onSubmit={handleSubmit(async (data) => {
                await createAppCategoryAsync({
                  name: data.name,
                  color: data.color,
                  iconPath: data.iconPath !== "" ? data.iconPath : null,
                });
                toast.success("类别创建成功");
                setIsCreateAppCategoryDialogOpen(false);
                reset();
              })}
            >
              <FieldGroup>
                <Field>
                  <FieldLabel>名称</FieldLabel>
                  <Input {...register("name")} />
                </Field>
                <Field>
                  <FieldLabel>颜色</FieldLabel>
                  <input type="color" {...register("color")} />
                </Field>
                <Field>
                  <FieldLabel>图标路径</FieldLabel>
                  <Input {...register("iconPath")} />
                </Field>
              </FieldGroup>
            </form>
            <DialogFooter>
              <DialogClose asChild>
                <Button variant="outline">取消</Button>
              </DialogClose>
              <Button type="submit" form="create-app-category-form">
                创建
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      </div>
      <div className="border-border mt-2 rounded-lg border">
        <Table
          style={{
            tableLayout: "fixed",
            width: table.getTotalSize(),
          }}
        >
          <TableHeader className="select-none">
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead
                      key={header.id}
                      style={{ width: header.getSize() }}
                      className="relative truncate"
                    >
                      {header.isPlaceholder
                        ? null
                        : flexRender(
                            header.column.columnDef.header,
                            header.getContext(),
                          )}
                      {header.column.getCanResize() && (
                        <div
                          onMouseDown={header.getResizeHandler()}
                          onTouchStart={header.getResizeHandler()}
                          className={
                            "bg-border absolute top-0 right-0 h-full w-0.5 cursor-col-resize"
                          }
                        />
                      )}
                    </TableHead>
                  );
                })}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            {table.getRowModel().rows?.length ? (
              table.getRowModel().rows.map((row) => (
                <TableRow
                  key={row.id}
                  data-state={row.getIsSelected() && "selected"}
                >
                  {row.getVisibleCells().map((cell) => (
                    <TableCell
                      key={cell.id}
                      style={{ width: cell.column.getSize() }}
                      className="truncate"
                    >
                      {flexRender(
                        cell.column.columnDef.cell,
                        cell.getContext(),
                      )}
                    </TableCell>
                  ))}
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell
                  colSpan={columns.length}
                  className="h-24 text-center"
                >
                  暂无
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </>
  );
};
