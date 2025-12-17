import { Logo } from "@/shared/components/Logo";
import { ThemeToggle } from "@/shared/components/ThemeToggle";
import { DesktopNav } from "./DesktopNav";
import { UserDropdown } from "./UserDropdown";
import { MobileMenu } from "./MobileMenu";

export function Header() {
  return (
    <header className="sticky top-0 z-50 w-full border-b bg-background/80 backdrop-blur-xl supports-backdrop-filter:bg-background/60">
      <div className="container flex h-16 items-center justify-between px-4">
        <div className="flex items-center gap-6">
          <Logo />
          <DesktopNav />
        </div>

        <div className="flex items-center gap-3">
          <ThemeToggle />
          <UserDropdown />
          <MobileMenu />
        </div>
      </div>
    </header>
  );
}
