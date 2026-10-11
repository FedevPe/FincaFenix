"use client";

import type { CurrentUserDTO } from "@/types/api";
import { AuthProvider } from "./providers/AuthProvider";
import { ThemeProvider } from "./providers/ThemeProvider";
import { TooltipProvider } from "@/components/ui/tooltip";
import { Toaster } from "@/components/ui/sonner";

export function Providers({
  children,
  initialUser,
}: {
  children: React.ReactNode;
  initialUser?: CurrentUserDTO | null;
}) {
  return (
    <ThemeProvider>
      <AuthProvider initialUser={initialUser}>
        <TooltipProvider delayDuration={200}>
          {children}
          <Toaster richColors position="top-right" />
        </TooltipProvider>
      </AuthProvider>
    </ThemeProvider>
  );
}
