import { Link, useLocation } from "react-router";
import { useAuthStore } from "@/features/auth/stores/authStore";
import { cn } from "@/shared/lib/utils";

const navLinks = [
  { to: "/", label: "Home", requiresAuth: false },
  { to: "/posts", label: "Posts", requiresAuth: false },
  { to: "/categorias", label: "Categorias", requiresAuth: true, requiresAdmin: true },
  { to: "/sobre", label: "Sobre", requiresAuth: false },
];

export function DesktopNav() {
  const location = useLocation();
  const { user } = useAuthStore();

  const visibleLinks = navLinks.filter((link) => {
    if (link.requiresAdmin) {
      return user?.role === "Admin";
    }
    if (link.requiresAuth) {
      return !!user;
    }
    return true;
  });

  return (
    <nav className="hidden lg:flex items-center gap-6" aria-label="Main navigation">
      {visibleLinks.map((link) => {
        const isActive = location.pathname === link.to;
        return (
          <Link
            key={link.to}
            to={link.to}
            className={cn(
              "text-sm font-medium transition-colors hover:text-primary relative py-1",
              isActive
                ? "text-foreground"
                : "text-muted-foreground"
            )}
            aria-current={isActive ? "page" : undefined}
          >
            {link.label}
            {isActive && (
              <span className="absolute bottom-0 left-0 right-0 h-0.5 bg-primary" />
            )}
          </Link>
        );
      })}
    </nav>
  );
}
