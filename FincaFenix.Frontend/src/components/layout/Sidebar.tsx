"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Boxes, Gauge, ListChecks, X } from "lucide-react";
import { cn, hasPolicy } from "@/lib/utils";
import { useAuth } from "@/app/providers/AuthProvider";

interface NavItem {
  href: string;
  label: string;
  icon: typeof Gauge;
  exact?: boolean;
  policy?: string;
}

const navItems: NavItem[] = [
  { href: "/dashboard", label: "Dashboard", icon: Gauge, exact: true },
  { href: "/work-orders", label: "Órdenes de trabajo", icon: ListChecks },
  { href: "/inventory", label: "Inventario", icon: Boxes, policy: "STOCK_READ" },
];

interface SidebarProps {
  collapsed: boolean;
  mobileOpen: boolean;
  onNavigate: () => void;
}

export function Sidebar({ collapsed, mobileOpen, onNavigate }: SidebarProps) {
  const pathname = usePathname();
  const { user } = useAuth();

  const visibleItems = navItems.filter(
    (item) => !item.policy || hasPolicy(user?.policies, item.policy),
  );

  return (
    <aside
      className={cn(
        "fixed inset-y-0 left-0 z-50 flex flex-col border-r border-chrome-border bg-chrome text-chrome-foreground transition-[width,transform] duration-200 ease-out",
        "w-[var(--sidebar-width)] md:translate-x-0",
        collapsed ? "md:w-[var(--sidebar-width-collapsed)]" : "md:w-[var(--sidebar-width)]",
        mobileOpen ? "translate-x-0" : "-translate-x-full"
      )}
    >
      <div
        className={cn(
          "flex h-[var(--header-height)] shrink-0 items-center border-b border-chrome-border px-4",
          collapsed ? "md:justify-center md:px-0" : "justify-between"
        )}
      >
        <span
          className={cn(
            "text-lg font-bold tracking-tight text-white",
            collapsed && "md:hidden"
          )}
        >
          FincaFenix
        </span>
        <button
          type="button"
          onClick={onNavigate}
          className="inline-flex size-8 items-center justify-center rounded-md text-chrome-muted hover:bg-white/10 hover:text-white md:hidden"
          aria-label="Cerrar menú"
        >
          <X className="size-5" />
        </button>
      </div>

      <nav className="flex flex-1 flex-col gap-1 overflow-y-auto p-3">
        {visibleItems.map((item) => {
          const active = item.exact
            ? pathname === item.href
            : pathname.startsWith(item.href);
          const Icon = item.icon;

          return (
            <Link
              key={item.href}
              href={item.href}
              onClick={onNavigate}
              aria-current={active ? "page" : undefined}
              className={cn(
                "flex items-center gap-3 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                collapsed && "md:justify-center md:px-0",
                active
                  ? "bg-chrome-primary/20 text-white"
                  : "text-chrome-muted hover:bg-white/10 hover:text-white"
              )}
              title={collapsed ? item.label : undefined}
            >
              <Icon className="size-5 shrink-0" />
              <span className={cn("truncate", collapsed && "md:hidden")}>{item.label}</span>
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
