import Link from "next/link";
import { Button } from "@/components/common/Button";

export default function NotFound() {
  return (
    <div className="flex min-h-svh flex-col items-center justify-center gap-4 p-6 text-center">
      <h1 className="text-5xl font-bold tracking-tight">404</h1>
      <p className="text-muted-foreground">Página no encontrada</p>
      <Link href="/dashboard">
        <Button variant="primary">Volver al inicio</Button>
      </Link>
    </div>
  );
}
