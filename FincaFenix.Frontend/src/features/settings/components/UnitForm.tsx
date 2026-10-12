"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { Field, TextInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { UnitOfMeasureDTO } from "@/types/api";
import {
  createUnitOfMeasure,
  updateUnitOfMeasure,
} from "../../materials/services/material.service";

interface UnitFormProps {
  unit?: UnitOfMeasureDTO | null;
  onClose: () => void;
  onSaved: () => void;
}

export function UnitForm({ unit, onClose, onSaved }: UnitFormProps) {
  const [description, setDescription] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setDescription(unit?.description ?? "");
    setErrors({});
    setSubmitError(null);
  }, [unit]);

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!description.trim()) {
      setErrors({ description: "La descripción es obligatoria." });
      return;
    }

    setSubmitting(true);
    try {
      const dto = { description: description.trim() };
      if (unit?.id) {
        await updateUnitOfMeasure({ id: unit.id, ...dto });
      } else {
        await createUnitOfMeasure(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar la unidad de medida.");
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

      <Field label="Descripción" htmlFor="unitDescription" required error={errors.description}>
        <TextInput
          id="unitDescription"
          value={description}
          invalid={!!errors.description}
          placeholder="Ej: Litro"
          onChange={(event) => {
            setDescription(event.target.value);
            setErrors((prev) => ({ ...prev, description: "" }));
          }}
        />
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          {unit?.id ? "Guardar cambios" : "Crear unidad"}
        </Button>
      </div>
    </form>
  );
}
