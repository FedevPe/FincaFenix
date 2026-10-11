import type { ReactNode } from "react";
import type { LucideIcon } from "lucide-react";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { cn } from "@/lib/utils";

export interface DropdownItem {
  label: string;
  icon?: LucideIcon;
  onClick: () => void;
  variant?: "default" | "danger";
  disabled?: boolean;
}

interface DropdownProps {
  trigger: ReactNode;
  triggerLabel: string;
  items: DropdownItem[];
  align?: "right" | "left";
  className?: string;
}

export function Dropdown({
  trigger,
  triggerLabel,
  items,
  align = "right",
  className,
}: DropdownProps) {
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button
          type="button"
          aria-label={triggerLabel}
          onClick={(event) => event.stopPropagation()}
          onPointerDown={(event) => event.stopPropagation()}
          className={cn(
            "inline-flex items-center justify-center rounded-md text-muted-foreground transition-colors hover:bg-accent hover:text-accent-foreground focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none [&_svg]:size-4",
            className,
          )}
        >
          {trigger}
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align={align === "right" ? "end" : "start"}>
        {items.map((item, index) => {
          const Icon = item.icon;
          return (
            <DropdownMenuItem
              key={`${item.label}-${index}`}
              disabled={item.disabled}
              variant={item.variant === "danger" ? "destructive" : "default"}
              onSelect={() => item.onClick()}
            >
              {Icon ? <Icon aria-hidden /> : null}
              <span>{item.label}</span>
            </DropdownMenuItem>
          );
        })}
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
