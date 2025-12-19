import { useState } from "react";
import { Search } from "lucide-react";
import { Logo } from "@/shared/components/Logo";
import { ThemeToggle } from "@/shared/components/ThemeToggle";
import { CommandPalette } from "@/shared/components/CommandPalette";
import { Button } from "@/shared/ui/button";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/shared/ui/tooltip";
import { DesktopNav } from "./DesktopNav";
import { UserDropdown } from "./UserDropdown";
import { MobileMenu } from "./MobileMenu";

export function Header() {
  const [commandOpen, setCommandOpen] = useState(false);

  return (
    <TooltipProvider>
      <header className="sticky top-0 z-50 w-full border-b bg-background/80 backdrop-blur-xl supports-backdrop-filter:bg-background/60">
        <div className="container flex h-16 items-center justify-between px-4">
          <div className="flex items-center gap-6">
            <Logo />
            <DesktopNav />
          </div>

          <div className="flex items-center gap-3">
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  variant="outline"
                  size="sm"
                  className="hidden md:flex items-center gap-2 text-muted-foreground"
                  onClick={() => setCommandOpen(true)}
                >
                  <Search className="h-4 w-4" />
                  <span className="text-xs">Buscar</span>
                  <kbd className="pointer-events-none inline-flex h-5 select-none items-center gap-1 rounded border bg-muted px-1.5 font-mono text-[10px] font-medium text-muted-foreground opacity-100">
                    <span className="text-xs">⌘</span>K
                  </kbd>
                </Button>
              </TooltipTrigger>
              <TooltipContent>
                <p>Buscar (⌘K)</p>
              </TooltipContent>
            </Tooltip>

            <ThemeToggle />
            <UserDropdown />
            <MobileMenu />
          </div>
        </div>

        <CommandPalette open={commandOpen} onOpenChange={setCommandOpen} />
      </header>
    </TooltipProvider>
  );
}
