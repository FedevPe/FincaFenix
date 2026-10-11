import { redirect } from "next/navigation";
import { getCurrentUserServer } from "@/lib/server/get-current-user";
import { Providers } from "@/app/providers";
import { AppShell } from "@/components/layout/AppShell";

export default async function AppLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const user = await getCurrentUserServer();

  if (!user) {
    redirect("/login");
  }

  return (
    <Providers initialUser={user}>
      <AppShell>{children}</AppShell>
    </Providers>
  );
}
