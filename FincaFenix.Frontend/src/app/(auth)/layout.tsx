import { Providers } from "@/app/providers";

export default function AuthLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <Providers>
      <div className="flex min-h-svh items-center justify-center bg-background p-4">
        {children}
      </div>
    </Providers>
  );
}
