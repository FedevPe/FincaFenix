"use client";

import { usePathname } from "next/navigation";
import { Menu, Sun, Moon, LogOut, ChevronDown } from "lucide-react";
import { useTheme } from "@/app/providers/ThemeProvider";
import { useAuth } from "@/app/providers/AuthProvider";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

function getTitle(pathname: string): string {
  if (pathname.startsWith("/work-orders")) return "Órdenes de trabajo";
  if (pathname.startsWith("/dashboard")) return "Dashboard";
  return "FincaFenix";
}

export function AppBar({ onToggleSidebar }: { onToggleSidebar: () => void }) {
  const pathname = usePathname();
  const { theme, toggleTheme } = useTheme();
  const { user, logout } = useAuth();

  const initials = (user?.userName ?? "U").slice(0, 2).toUpperCase();

  return (
    <header className="sticky top-0 z-30 flex h-[var(--header-height)] items-center gap-2 border-b border-chrome-border bg-chrome-surface px-3 text-chrome-foreground md:px-4">
      <button
        type="button"
        onClick={onToggleSidebar}
        className="inline-flex size-9 items-center justify-center rounded-md text-chrome-muted hover:bg-white/10 hover:text-white"
        aria-label="Alternar menú"
      >
        <Menu className="size-5" />
      </button>

      <h1 className="truncate text-base font-semibold text-white">{getTitle(pathname)}</h1>

      <div className="ml-auto flex items-center gap-2">
        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <button
              type="button"
              className="inline-flex items-center gap-2 rounded-md px-2 py-1.5 text-sm text-chrome-foreground transition-colors hover:bg-white/10"
            >
              <Avatar className="size-7">
                <AvatarFallback className="bg-chrome-primary text-xs text-white">
                  {initials}
                </AvatarFallback>
              </Avatar>
              <span className="hidden max-w-40 truncate sm:inline">
                {user?.userName || "Usuario"}
              </span>
              <ChevronDown className="size-4 text-chrome-muted" />
            </button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-52">
            <DropdownMenuLabel className="truncate">
              {user?.userName || "Usuario"}
            </DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem onSelect={() => toggleTheme()}>
              {theme === "dark" ? (
                <Sun className="size-4" />
              ) : (
                <Moon className="size-4" />
              )}
              {theme === "dark" ? "Modo claro" : "Modo oscuro"}
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem variant="destructive" onSelect={() => void logout()}>
              <LogOut className="size-4" />
              Cerrar sesión
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  );
}
