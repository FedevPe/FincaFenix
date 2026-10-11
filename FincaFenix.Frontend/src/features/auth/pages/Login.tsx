"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { useAuth } from "@/app/providers/AuthProvider";
import { getProblemDetails } from "@/lib/api.client";
import { login } from "../services/auth.service";

const schema = z.object({
  userName: z.string().min(1, "El usuario es obligatorio"),
  password: z.string().min(1, "La contraseña es obligatoria"),
});

type FormValues = z.infer<typeof schema>;

export default function Login() {
  const { login: setAuth } = useAuth();
  const router = useRouter();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: { userName: "", password: "" },
  });

  const onSubmit = async (values: FormValues) => {
    setServerError(null);
    try {
      const user = await login(values);
      setAuth(user);
      router.replace("/dashboard");
      router.refresh();
    } catch (error) {
      const pd = getProblemDetails(error);
      setServerError(pd?.detail || "Usuario o contraseña incorrectos");
    }
  };

  return (
    <div className="w-full max-w-sm rounded-xl border bg-card p-6 text-card-foreground shadow-sm">
      <header className="mb-6 space-y-1 text-center">
        <h1 className="text-2xl font-bold tracking-tight">FincaFenix</h1>
        <p className="text-sm text-muted-foreground">
          Inicie sesión para continuar
        </p>
      </header>

      <form className="space-y-4" onSubmit={handleSubmit(onSubmit)}>
        <div className="space-y-1.5">
          <label className="text-sm font-medium" htmlFor="userName">
            Usuario
          </label>
          <input
            id="userName"
            autoComplete="username"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none"
            {...register("userName")}
          />
          {errors.userName ? (
            <span className="text-xs text-destructive">{errors.userName.message}</span>
          ) : null}
        </div>

        <div className="space-y-1.5">
          <label className="text-sm font-medium" htmlFor="password">
            Contraseña
          </label>
          <input
            id="password"
            type="password"
            autoComplete="current-password"
            className="h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none"
            {...register("password")}
          />
          {errors.password ? (
            <span className="text-xs text-destructive">{errors.password.message}</span>
          ) : null}
        </div>

        {serverError ? (
          <div
            className="flex items-center gap-2 rounded-md border border-destructive/40 bg-destructive/10 px-3 py-2 text-sm text-destructive"
            role="alert"
          >
            <AlertCircle className="size-4 shrink-0" aria-hidden />
            <span>{serverError}</span>
          </div>
        ) : null}

        <Button
          type="submit"
          variant="primary"
          className="w-full"
          loading={isSubmitting}
        >
          {isSubmitting ? "Ingresando..." : "Ingresar"}
        </Button>
      </form>
    </div>
  );
}
