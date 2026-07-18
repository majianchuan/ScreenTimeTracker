import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";

export const AppLocalizationProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  return (
    <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale="zh-cn">
      {children}
    </LocalizationProvider>
  );
};
