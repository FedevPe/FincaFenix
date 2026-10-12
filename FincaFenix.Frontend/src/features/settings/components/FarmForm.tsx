"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { Field, TextInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { FarmDTO } from "@/types/api";
import { createFarm, updateFarm } from "../services/farm.service";

interface FarmFormProps {
  farm?: FarmDTO | null;
  onClose: () => void;
  onSaved: () => void;
}

export function FarmForm({ farm, onClose, onSaved }: FarmFormProps) {
  const [name, setName] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setName(farm?.name ?? "");
    setErrors({});
    setSubmitError(null);
  }, [farm]);

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!name.trim()) {
      setErrors({ name: "El nombre de la finca es obligatorio." });
      return;
    }
    if (name.trim().length > 100) {
      setErrors({ name: "El nombre no puede superar los 100 caracteres." });
      return;
    }

    setSubmitting(true);
    try {
      const dto = { name: name.trim() };
      if (farm?.id) {
        await updateFarm({ id: farm.id, ...dto });
      } else {
        await createFarm(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar la finca.");
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

      <Field label="Nombre" htmlFor="farmName" required error={errors.name}>
        <TextInput
          id="farmName"
          value={name}
          invalid={!!errors.name}
          placeholder="Ej: Finca Norte"
          onChange={(event) => {
            setName(event.target.value);
            setErrors((prev) => ({ ...prev, name: "" }));
          }}
        />
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          {farm?.id ? "Guardar cambios" : "Crear finca"}
        </Button>
      </div>
    </form>
  );
}
