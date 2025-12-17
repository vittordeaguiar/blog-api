import { useState } from "react";
import { Link, useLocation } from "react-router";
import { Menu, User, LogOut } from "lucide-react";
import { Button } from "@/shared/ui/button";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@/shared/ui/sheet";
import { ScrollArea } from "@/shared/ui/scroll-area";
import { Separator } from "@/shared/ui/separator";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { cn } from "@/shared/lib/utils";

const navLinks = [
  { to: "/", label: "Home", requiresAuth: false },
  { to: "/posts", label: "Posts", requiresAuth: false },
  { to: "/categorias", label: "Categorias", requiresAuth: true, requiresAdmin: true },
  { to: "/sobre", label: "Sobre", requiresAuth: false },
];

export function MobileMenu() {
  const [open, setOpen] = useState(false);
  const location = useLocation();
  const { user, logout } = useAuthStore();

  const visibleLinks = navLinks.filter((link) => {
    if (link.requiresAdmin) {
      return user?.role === "Admin";
    }
    if (link.requiresAuth) {
      return !!user;
    }
    return true;
  });

  const handleLogout = () => {
    logout();
    setOpen(false);
  };

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="lg:hidden">
          <Menu className="h-5 w-5" />
          <span className="sr-only">Toggle menu</span>
        </Button>
      </SheetTrigger>
      <SheetContent side="right" className="w-[300px] sm:w-[400px]">
        <SheetHeader>
          <SheetTitle>Menu</SheetTitle>
        </SheetHeader>
        <ScrollArea className="h-full py-6">
          {user && (
            <>
              <div className="flex items-center gap-3 px-2 py-4">
                <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary/10">
                  <User className="h-5 w-5 text-primary" />
                </div>
                <div className="flex flex-col">
                  <span className="text-sm font-medium">{user.name}</span>
                  <span className="text-xs text-muted-foreground">{user.email}</span>
                </div>
              </div>
              <Separator className="my-4" />
            </>
          )}

          <nav className="flex flex-col space-y-1">
            {visibleLinks.map((link) => {
              const isActive = location.pathname === link.to;
              return (
                <Link
                  key={link.to}
                  to={link.to}
                  onClick={() => setOpen(false)}
                  className={cn(
                    "flex items-center rounded-lg px-3 py-2 text-sm font-medium transition-colors",
                    isActive
                      ? "bg-primary/10 text-primary"
                      : "text-muted-foreground hover:bg-muted hover:text-foreground"
                  )}
                  aria-current={isActive ? "page" : undefined}
                >
                  {link.label}
                </Link>
              );
            })}
          </nav>

          {user && (
            <>
              <Separator className="my-4" />
              <nav className="flex flex-col space-y-1">
                <Link
                  to="/perfil"
                  onClick={() => setOpen(false)}
                  className="flex items-center rounded-lg px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-muted hover:text-foreground transition-colors"
                >
                  <User className="mr-2 h-4 w-4" />
                  Perfil
                </Link>
                <button
                  onClick={handleLogout}
                  className="flex items-center rounded-lg px-3 py-2 text-sm font-medium text-destructive hover:bg-destructive/10 transition-colors text-left w-full"
                >
                  <LogOut className="mr-2 h-4 w-4" />
                  Sair
                </button>
              </nav>
            </>
          )}

          {!user && (
            <>
              <Separator className="my-4" />
              <Button asChild className="w-full" onClick={() => setOpen(false)}>
                <Link to="/login">Login</Link>
              </Button>
            </>
          )}
        </ScrollArea>
      </SheetContent>
    </Sheet>
  );
}
