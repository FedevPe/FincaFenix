"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import {
  Field,
  SelectInput,
  TextInput,
} from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { UserDTO } from "@/types/api";
import { createUser, updateUser } from "../services/user.service";

interface UserFormProps {
  user?: UserDTO | null;
  roles: string[];
  onClose: () => void;
  onSaved: () => void;
}

export function UserForm({ user, roles, onClose, onSaved }: UserFormProps) {
  const isEdit = Boolean(user?.id);
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [name, setName] = useState("");
  const [lastName, setLastName] = useState("");
  const [password, setPassword] = useState("");
  const [roleName, setRoleName] = useState("");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setUserName(user?.userName ?? "");
    setEmail(user?.email ?? "");
    setName(user?.name ?? "");
    setLastName(user?.lastName ?? "");
    setPassword("");
    setRoleName(user?.roles?.[0] ?? roles[0] ?? "");
    setErrors({});
    setSubmitError(null);
  }, [user, roles]);

  const validatePassword = (value: string): string | null => {
    if (value.length < 6) return "Debe tener al menos 6 caracteres.";
    if (!/[0-9]/.test(value)) return "Debe incluir al menos un dígito.";
    if (!/[a-z]/.test(value)) return "Debe incluir al menos una minúscula.";
    if (!/[A-Z]/.test(value)) return "Debe incluir al menos una mayúscula.";
    return null;
  };

  const handleSubmit = async () => {
    setSubmitError(null);
    const next: Record<string, string> = {};
    if (!isEdit && !userName.trim()) next.userName = "El usuario es obligatorio.";
    if (!roleName) next.roleName = "Seleccioná un rol.";
    if (!isEdit) {
      const passwordError = validatePassword(password);
      if (passwordError) next.password = passwordError;
    }
    if (Object.keys(next).length > 0) {
      setErrors(next);
      return;
    }

    setSubmitting(true);
    try {
      if (isEdit && user) {
        await updateUser({
          id: user.id,
          email: email.trim() || null,
          name: name.trim() || null,
          lastName: lastName.trim() || null,
          roleName,
        });
      } else {
        await createUser({
          userName: userName.trim(),
          email: email.trim() || null,
          name: name.trim() || null,
          lastName: lastName.trim() || null,
          password,
          roleName,
        });
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar el usuario.");
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form
      className="space-y-4"
      onSubmit={(event) => {
        event.preventDefault();
        void handleSubmit();
      }}
      noValidate
    >
      {submitError ? (
        <div
          className="flex items-center gap-2 rounded-md border border-destructive/40 bg-destructive/10 px-3 py-2 text-sm text-destructive"
          role="alert"
        >
          <AlertCircle className="size-4 shrink-0" aria-hidden />
          <span>{submitError}</span>
        </div>
      ) : null}

      <div className="grid gap-4 sm:grid-cols-2">
        <Field
          label="Usuario"
          htmlFor="userName"
          required={!isEdit}
          error={errors.userName}
          hint={isEdit ? "El nombre de usuario no se puede modificar." : undefined}
        >
          <TextInput
            id="userName"
            value={userName}
            disabled={isEdit}
            invalid={!!errors.userName}
            onChange={(event) => {
              setUserName(event.target.value);
              setErrors((prev) => ({ ...prev, userName: "" }));
            }}
          />
        </Field>
        <Field label="Email" htmlFor="userEmail">
          <TextInput
            id="userEmail"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
          />
        </Field>
        <Field label="Nombre" htmlFor="userFirstName">
          <TextInput
            id="userFirstName"
            value={name}
            onChange={(event) => setName(event.target.value)}
          />
        </Field>
        <Field label="Apellido" htmlFor="userLastName">
          <TextInput
            id="userLastName"
            value={lastName}
            onChange={(event) => setLastName(event.target.value)}
          />
        </Field>
        <Field label="Rol" htmlFor="userRole" required error={errors.roleName}>
          <SelectInput
            id="userRole"
            value={roleName}
            invalid={!!errors.roleName}
            onChange={(event) => {
              setRoleName(event.target.value);
              setErrors((prev) => ({ ...prev, roleName: "" }));
            }}
          >
            <option value="">Seleccionar…</option>
            {roles.map((role) => (
              <option key={role} value={role}>
                {role}
              </option>
            ))}
          </SelectInput>
        </Field>
        {!isEdit ? (
          <Field
            label="Contraseña"
            htmlFor="userPassword"
            required
            error={errors.password}
            hint="Mínimo 6 caracteres, con mayúscula, minúscula y dígito."
          >
            <TextInput
              id="userPassword"
              type="password"
              value={password}
              invalid={!!errors.password}
              onChange={(event) => {
                setPassword(event.target.value);
                setErrors((prev) => ({ ...prev, password: "" }));
              }}
            />
          </Field>
        ) : null}
      </div>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          {isEdit ? "Guardar cambios" : "Crear usuario"}
        </Button>
      </div>
    </form>
  );
}
