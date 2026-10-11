"use client";

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import type { CurrentUserDTO } from "../../types/api";
import * as authService from "../../features/auth/services/auth.service";

export interface AuthUser {
  id?: string;
  userName?: string;
  email?: string;
  roles?: string[];
  policies?: string[];
}

interface AuthContextValue {
  isAuthenticated: boolean;
  isChecking: boolean;
  user: AuthUser | null;
  login: (user: CurrentUserDTO) => void;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

export function AuthProvider({
  children,
  initialUser,
}: {
  children: React.ReactNode;
  initialUser?: CurrentUserDTO | null;
}) {
  const [user, setUser] = useState<AuthUser | null>(initialUser ?? null);
  const [isChecking, setIsChecking] = useState(!initialUser);
  const router = useRouter();

  useEffect(() => {
    if (initialUser) return;

    let active = true;

    authService
      .me()
      .then((currentUser) => {
        if (active) setUser((prev) => prev ?? currentUser);
      })
      .catch(() => {
        // Sin sesión activa (401) — se mantiene el estado actual.
      })
      .finally(() => {
        if (active) setIsChecking(false);
      });

    return () => {
      active = false;
    };
  }, [initialUser]);

  const login = useCallback((currentUser: CurrentUserDTO) => {
    setUser(currentUser);
  }, []);

  const logout = useCallback(async () => {
    try {
      await authService.logout();
    } catch {
      // Ignorar errores de red al cerrar sesión.
    }
    setUser(null);
    router.replace("/login");
    router.refresh();
  }, [router]);

  const value = useMemo(
    () => ({
      isAuthenticated: !!user,
      isChecking,
      user,
      login,
      logout,
    }),
    [user, isChecking, login, logout]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}
