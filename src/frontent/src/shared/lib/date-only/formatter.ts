import type { DateOnly } from "./schema";

export const dateToDateOnly = (date: Date): DateOnly => {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
};

export const dateOnlyToDate = (dateOnly: DateOnly): Date => {
  return new Date(dateOnly);
};
