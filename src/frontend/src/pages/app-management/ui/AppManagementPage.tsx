import {
  AppIcon,
  appQueries,
  useDeleteApp,
  usePatchApp,
  type App,
} from "@/entities/app";
import {
  AppCategoryIcon,
  AppCategoryPicker,
  appCategoryQueries,
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
  cn,
  DropdownMenu,
  DropdownMenuCheckboxItem,
  DropdownMenuContent,
  DropdownMenuTrigger,
  Input,
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
  PaginationLink,
  Select,
  SelectContent,
  SelectGroup,
  SelectItem,
  SelectTrigger,
  SelectValue,
  Switch,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/shared/lib/shadcn";
import { BlurInput } from "@/shared/ui/BlurInput";
import { useQuery } from "@tanstack/react-query";
import {
  type Column,
  type ColumnDef,
  type ColumnFiltersState,
  flexRender,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
  type VisibilityState,
} from "@tanstack/react-table";
import {
  ArrowDown,
  ArrowUp,
  ChevronLeft,
  ChevronRight,
  Trash,
} from "lucide-react";
import { useState } from "react";
import { toast } from "sonner";

export const AppManagementPage = () => {
  const { data: appsData } = useQuery(appQueries.apps({})) as {
    data?: App[];
  };
  const { data: appCategoriesData } = useQuery(
    appCategoryQueries.appCategories({ fields: "id,name,iconPath" }),
  ) as {
    data?: AppCategory[];
  };
  const { mutateAsync: patchAppAsync } = usePatchApp();
  const { mutateAsync: deleteAppAsync } = useDeleteApp();

  const applyNextSort = (column: Column<App>) => {
    switch (column.getIsSorted()) {
      case false:
        column.toggleSorting(false);
        break;
      case "asc":
        column.toggleSorting(true);
        break;
      case "desc":
        column.clearSorting();
        break;
    }
  };

  const columnTitlesMap = {
    name: "名称",
    appCategoryId: "应用类别",
    icon: "图标",
    iconPath: "图标路径",
    isAutoUpdateEnabled: "自动更新",
    lastAutoUpdated: "上次自动更新时间",
    processName: "进程名",
    description: "描述",
    executablePath: "可执行文件路径",
    action: "操作",
  };

  const columns: ColumnDef<App>[] = [
    {
      accessorKey: "name",
      header: ({ column }) => {
        return (
          <Button
            className="w-full justify-start"
            variant="ghost"
            onClick={() => applyNextSort(column)}
          >
            {columnTitlesMap[column.id as keyof typeof columnTitlesMap]}
            <ArrowUp
              className={cn(
                column.getIsSorted() !== "asc" ? "hidden" : "",
                "ml-1 size-4",
              )}
            />
            <ArrowDown
              className={cn(
                column.getIsSorted() !== "desc" ? "hidden" : "",
                "ml-1 size-4",
              )}
            />
          </Button>
        );
      },
      cell: ({ row }) => {
        const app = row.original;
        return (
          <BlurInput
            className="min-w-20"
            value={app.name}
            onBlurUpdate={async (value: string) => {
              try {
                await patchAppAsync({
                  id: app.id,
                  body: { name: value },
                });
                toast.success("更新名称成功");
              } catch {
                toast.error("更新名称失败");
              }
            }}
          />
        );
      },
    },
    {
      accessorKey: "appCategoryId",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
      cell: ({ row }) => {
        const app = row.original;
        return (
          <Select
            value={app.appCategoryId}
            onValueChange={async (value) => {
              try {
                await patchAppAsync({
                  id: app.id,
                  body: { appCategoryId: value },
                });
                toast.success("更新应用类别成功");
              } catch {
                toast.error("更新应用类别失败");
              }
            }}
          >
            <SelectTrigger>
              <SelectValue />
            </SelectTrigger>
            <SelectContent position="popper">
              <SelectGroup>
                {appCategoriesData?.map((item) => {
                  return (
                    <SelectItem key={item.id} value={item.id}>
                      <AppCategoryIcon
                        className="size-5"
                        id={item.id}
                        iconPath={item.iconPath}
                      />
                      {item.name}
                    </SelectItem>
                  );
                })}
              </SelectGroup>
            </SelectContent>
          </Select>
        );
      },
    },
    {
      id: "icon",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
      cell: ({ row }) => {
        const app = row.original;
        return (
          <AppIcon className="size-7" id={app.id} iconPath={app.iconPath} />
        );
      },
    },
    {
      accessorKey: "iconPath",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
      cell: ({ row }) => {
        const app = row.original;
        return (
          <BlurInput
            className="min-w-20"
            value={app.iconPath !== null ? app.iconPath : ""}
            onBlurUpdate={async (value: string) => {
              try {
                await patchAppAsync({
                  id: app.id,
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
      accessorKey: "isAutoUpdateEnabled",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
      cell: ({ row }) => {
        return (
          <Switch
            checked={row.getValue("isAutoUpdateEnabled")}
            onCheckedChange={async (checked) => {
              const app = row.original;
              try {
                await patchAppAsync({
                  id: app.id,
                  body: { isAutoUpdateEnabled: checked },
                });
                toast.success("更新允许自动更新成功");
              } catch {
                toast.error("更新允许自动更新失败");
              }
            }}
          />
        );
      },
    },
    {
      accessorKey: "lastAutoUpdated",
      header: ({ column }) => {
        return (
          <Button
            className="w-full justify-start"
            variant="ghost"
            onClick={() => applyNextSort(column)}
          >
            {columnTitlesMap[column.id as keyof typeof columnTitlesMap]}
            <ArrowUp
              className={cn(
                column.getIsSorted() !== "asc" ? "hidden" : "",
                "ml-1 size-4",
              )}
            />
            <ArrowDown
              className={cn(
                column.getIsSorted() !== "desc" ? "hidden" : "",
                "ml-1 size-4",
              )}
            />
          </Button>
        );
      },
      cell: ({ row }) => {
        return row.getValue("lastAutoUpdated")?.toLocaleString();
      },
    },
    {
      accessorKey: "processName",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
    },
    {
      accessorKey: "description",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
    },
    {
      accessorKey: "executablePath",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
    },
    {
      id: "action",
      header: ({ column }) =>
        columnTitlesMap[column.id as keyof typeof columnTitlesMap],
      cell: ({ row }) => {
        const app = row.original;
        return (
          <AlertDialog>
            <AlertDialogTrigger asChild>
              <Button variant="destructive">
                <Trash />
              </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
              <AlertDialogHeader>
                <AlertDialogTitle>
                  你确定要删除应用“{app.name}”吗？
                </AlertDialogTitle>
                <AlertDialogDescription>
                  同时将删除所有该应用的已有数据。
                </AlertDialogDescription>
              </AlertDialogHeader>
              <AlertDialogFooter>
                <AlertDialogCancel>取消</AlertDialogCancel>
                <AlertDialogAction
                  onClick={async () => {
                    try {
                      await deleteAppAsync(app.id);
                      toast.success("删除应用成功");
                    } catch {
                      toast.error("删除应用失败");
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

  const [sorting, setSorting] = useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
  const [columnVisibility, setColumnVisibility] = useState<VisibilityState>({
    lastAutoUpdated: false,
    processName: false,
    description: false,
    executablePath: false,
  });
  const table = useReactTable({
    data: appsData || [],
    columns,
    getCoreRowModel: getCoreRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    onSortingChange: setSorting,
    getSortedRowModel: getSortedRowModel(),
    onColumnFiltersChange: setColumnFilters,
    getFilteredRowModel: getFilteredRowModel(),
    onColumnVisibilityChange: setColumnVisibility,
    state: {
      sorting,
      columnFilters,
      columnVisibility,
    },
  });

  // 正数表示页码，-1表示左省略号，-2表示右省略号
  const getPaginationRange = (
    currentPage: number,
    totalPages: number,
  ): number[] => {
    if (totalPages <= 5) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    const result: number[] = [];
    result.push(1);
    if (currentPage <= 3) {
      for (let i = 2; i <= 4; i++) {
        if (!result.includes(i)) result.push(i);
      }
      result.push(-2);
    } else if (currentPage >= totalPages - 2) {
      result.push(-1);
      for (let i = totalPages - 3; i < totalPages; i++) {
        if (!result.includes(i)) result.push(i);
      }
    } else {
      result.push(-1);
      result.push(currentPage - 1);
      result.push(currentPage);
      result.push(currentPage + 1);
      result.push(-2);
    }
    result.push(totalPages);

    return result;
  };

  return (
    <>
      <div className="flex gap-2">
        <Input
          placeholder="过滤名称"
          value={(table.getColumn("name")?.getFilterValue() as string) ?? ""}
          onChange={(event) =>
            table.getColumn("name")?.setFilterValue(event.target.value)
          }
          className="max-w-50"
        />
        <AppCategoryPicker
          mode="single"
          value={
            (table.getColumn("appCategoryId")?.getFilterValue() as string[]) ??
            []
          }
          onChange={(value) =>
            table.getColumn("appCategoryId")?.setFilterValue(value)
          }
          placeholder="选择类别"
        />
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="outline" className="ml-auto">
              列
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-auto">
            {table
              .getAllColumns()
              .filter((column) => column.getCanHide())
              .map((column) => {
                return (
                  <DropdownMenuCheckboxItem
                    key={column.id}
                    checked={column.getIsVisible()}
                    onCheckedChange={(value) =>
                      column.toggleVisibility(!!value)
                    }
                  >
                    {columnTitlesMap[column.id as keyof typeof columnTitlesMap]}
                  </DropdownMenuCheckboxItem>
                );
              })}
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
      <div className="border-border mt-2 rounded-lg border">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  return (
                    <TableHead key={header.id}>
                      {header.isPlaceholder
                        ? null
                        : flexRender(
                            header.column.columnDef.header,
                            header.getContext(),
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
                    <TableCell key={cell.id}>
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

      <div className="mt-2 flex items-center justify-center gap-2">
        <Pagination className="mx-0 w-auto">
          <PaginationContent>
            <PaginationItem>
              <PaginationLink onClick={() => table.previousPage()}>
                <ChevronLeft />
              </PaginationLink>
            </PaginationItem>
            {getPaginationRange(
              table.getState().pagination.pageIndex + 1,
              table.getPageCount(),
            ).map((page) =>
              page > 0 ? (
                <PaginationItem key={page}>
                  <PaginationLink
                    onClick={() => table.setPageIndex(page - 1)}
                    isActive={
                      page === table.getState().pagination.pageIndex + 1
                    }
                  >
                    {page}
                  </PaginationLink>
                </PaginationItem>
              ) : (
                <PaginationItem key={page}>
                  <PaginationEllipsis />
                </PaginationItem>
              ),
            )}
            <PaginationItem>
              <PaginationLink onClick={() => table.nextPage()}>
                <ChevronRight />
              </PaginationLink>
            </PaginationItem>
          </PaginationContent>
        </Pagination>
        <Select
          value={`${table.getState().pagination.pageSize}`}
          onValueChange={(value) => {
            table.setPageSize(Number(value));
          }}
        >
          <SelectTrigger className="h-8 w-20">
            <SelectValue placeholder={table.getState().pagination.pageSize} />
          </SelectTrigger>
          <SelectContent position="popper">
            {[5, 10, 20, 40, 80].map((pageSize) => (
              <SelectItem key={pageSize} value={`${pageSize}`}>
                {pageSize}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
    </>
  );
};
