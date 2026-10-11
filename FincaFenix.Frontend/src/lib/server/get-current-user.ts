import { cookies } from "next/headers";
import type { CurrentUserDTO } from "@/types/api";

export async function getCurrentUserServer(): Promise<CurrentUserDTO | null> {
  const cookieStore = await cookies();
  const cookieHeader = cookieStore.toString();
  if (!cookieHeader) return null;

  const base = process.env.API_PROXY_TARGET ?? "http://localhost:5000";

  try {
    const res = await fetch(`${base}/api/auth/me`, {
      headers: { cookie: cookieHeader },
      cache: "no-store",
    });
    if (!res.ok) return null;
    return (await res.json()) as CurrentUserDTO;
  } catch {
    return null;
  }
}
