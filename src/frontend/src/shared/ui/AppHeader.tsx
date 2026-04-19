import { Button } from "@/shared/lib/shadcn/components/button";
import { Separator } from "@/shared/lib/shadcn/components/separator";
import { Moon, Sun } from "lucide-react";
import { SiDiscord, SiGithub, SiQq } from "@icons-pack/react-simple-icons";
import { useTheme } from "next-themes";
import { LogoIcon } from "./icons";

export const AppHeader = () => {
  const { theme, setTheme, systemTheme } = useTheme();
  const isDark = (theme === "system" ? systemTheme : theme) === "dark";

  const toggleTheme = () => {
    const nextTheme = isDark ? "light" : "dark";
    if (nextTheme === systemTheme) {
      setTheme("system");
    } else {
      setTheme(nextTheme);
    }
  };
  return (
    <header className="bg-background flex h-full w-full items-center justify-between">
      <a href="/" className="flex cursor-pointer items-center">
        <LogoIcon className="ml-3 h-8" />
        <span className="ml-1 text-xl font-bold">Screen Time Tracker</span>
      </a>
      <div className="mr-3 flex items-center gap-2">
        <Button asChild variant="ghost" size="icon">
          <a
            href="https://qm.qq.com/q/uiwJZiQRAm"
            target="_blank"
            rel="noopener noreferrer"
          >
            <SiQq />
          </a>
        </Button>
        <Button variant="ghost" size="icon">
          <a
            href="https://github.com/majianchuan/ScreenTimeTracker"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="GitHub"
          >
            <SiGithub />
          </a>
        </Button>
        <Button variant="ghost" size="icon">
          <a
            href="https://discord.gg/PxqGwcsVuh"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="Discord"
          >
            <SiDiscord />
          </a>
        </Button>
        <Separator orientation="vertical" className="h-5 self-center!" />
        <Button variant="ghost" size="icon" onClick={toggleTheme}>
          <Sun className={isDark ? "hidden" : ""} />
          <Moon className={isDark ? "" : "hidden"} />
        </Button>
      </div>
    </header>
  );
};
