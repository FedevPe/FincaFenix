import type { ButtonHTMLAttributes, ReactNode, ComponentPropsWithoutRef } from "react";
import type { LucideIcon } from "lucide-react";
import { Loader2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { Button as ShadcnButton } from "@/components/ui/button";

type Variant = "primary" | "secondary" | "ghost" | "danger" | "outline";
type Size = "sm" | "md";

const VARIANT_MAP: Record<Variant, ComponentPropsWithoutRef<typeof ShadcnButton>["variant"]> = {
  primary: "default",
  secondary: "secondary",
  ghost: "ghost",
  danger: "destructive",
  outline: "outline"
};

const SIZE_MAP: Record<Size, ComponentPropsWithoutRef<typeof ShadcnButton>["size"]> = {
  sm: "sm",
  md: "default",
};

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: Variant;
  size?: Size;
  icon?: LucideIcon;
  loading?: boolean;
  children?: ReactNode;
  asChild?: boolean;
}

export function Button({
  variant = "secondary",
  size = "md",
  icon: Icon,
  loading = false,
  children,
  className,
  disabled,
  asChild = false,
  type = "button",
  ...rest
}: ButtonProps) {
  const resolvedVariant = VARIANT_MAP[variant];
  const resolvedSize = SIZE_MAP[size];
  const isDisabled = disabled || loading;

  const iconElement = loading ? (
    <Loader2 data-icon="inline-start" className="animate-spin" aria-hidden />
  ) : Icon ? (
    <Icon data-icon="inline-start" aria-hidden />
  ) : null;

  return (
    <ShadcnButton
      variant={resolvedVariant}
      size={resolvedSize}
      asChild={asChild}
      className={cn(className)}
      disabled={isDisabled}
      type={type}
      {...rest}
    >
      {iconElement}
      {children}
    </ShadcnButton>
  );
}