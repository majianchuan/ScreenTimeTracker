import { ThemeProvider, createTheme } from "@mui/material/styles";

export const AppThemeProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const theme = createTheme({
    colorSchemes: {
      dark: true,
    },
  });

  return <ThemeProvider theme={theme}>{children}</ThemeProvider>;
};
