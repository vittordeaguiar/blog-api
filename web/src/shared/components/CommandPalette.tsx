import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import {
  Search,
  FileText,
  Home,
  LogIn,
  Moon,
  Sun,
  Settings,
} from "lucide-react";
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/shared/ui/command";
import { useTheme } from "@/shared/providers/ThemeProvider";
import { usePosts } from "@/features/posts/hooks/usePost";

interface CommandPaletteProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CommandPalette({ open, onOpenChange }: CommandPaletteProps) {
  const navigate = useNavigate();
  const { theme, setTheme } = useTheme();
  const [searchQuery, setSearchQuery] = useState("");

  const { data: postsData } = usePosts({
    page: 1,
    pageSize: 10,
  });

  const filteredPosts = postsData?.items.filter((post) => {
    const query = searchQuery.toLowerCase();
    return (
      post.title.toLowerCase().includes(query) ||
      post.content.toLowerCase().includes(query)
    );
  });

  useEffect(() => {
    const down = (e: KeyboardEvent) => {
      if (e.key === "k" && (e.metaKey || e.ctrlKey)) {
        e.preventDefault();
        onOpenChange(true);
      }
    };

    document.addEventListener("keydown", down);
    return () => document.removeEventListener("keydown", down);
  }, [onOpenChange]);

  const handleSelect = (callback: () => void) => {
    onOpenChange(false);
    callback();
  };

  return (
    <CommandDialog open={open} onOpenChange={onOpenChange}>
      <CommandInput
        placeholder="Buscar posts, páginas, preferências..."
        value={searchQuery}
        onValueChange={setSearchQuery}
      />
      <CommandList>
        <CommandEmpty>Nenhum resultado encontrado.</CommandEmpty>

        <CommandGroup heading="Navegação">
          <CommandItem onSelect={() => handleSelect(() => navigate("/"))}>
            <Home className="mr-2 h-4 w-4" />
            <span>Página Inicial</span>
          </CommandItem>
          <CommandItem onSelect={() => handleSelect(() => navigate("/login"))}>
            <LogIn className="mr-2 h-4 w-4" />
            <span>Login</span>
          </CommandItem>
        </CommandGroup>

        <CommandSeparator />

        {filteredPosts && filteredPosts.length > 0 && (
          <>
            <CommandGroup heading="Posts">
              {filteredPosts.map((post) => (
                <CommandItem
                  key={post.id}
                  onSelect={() =>
                    handleSelect(() => navigate(`/posts/${post.slug}`))
                  }
                >
                  <FileText className="mr-2 h-4 w-4" />
                  <span>{post.title}</span>
                </CommandItem>
              ))}
            </CommandGroup>
            <CommandSeparator />
          </>
        )}

        <CommandGroup heading="Preferências">
          <CommandItem
            onSelect={() =>
              handleSelect(() => setTheme(theme === "dark" ? "light" : "dark"))
            }
          >
            {theme === "dark" ? (
              <Sun className="mr-2 h-4 w-4" />
            ) : (
              <Moon className="mr-2 h-4 w-4" />
            )}
            <span>Alternar Tema</span>
          </CommandItem>
        </CommandGroup>
      </CommandList>
    </CommandDialog>
  );
}
