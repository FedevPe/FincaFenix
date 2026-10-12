"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { Field, TextInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { MaterialCategoryDTO } from "@/types/api";
import {
  createMaterialCategory,
  updateMaterialCategory,
} from "../../materials/services/material.service";

interface CategoryFormProps {
  category?: MaterialCategoryDTO | null;
  onClose: () => void;
  onSaved: () => void;
}

export function CategoryForm({ category, onClose, onSaved }: CategoryFormProps) {
  const [description, setDescription] = useState("");
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setDescription(category?.description ?? "");
    setErrors({});
    setSubmitError(null);
  }, [category]);

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!description.trim()) {
      setErrors({ description: "La descripción es obligatoria." });
      return;
    }

    setSubmitting(true);
    try {
      const dto = { description: description.trim() };
      if (category?.id) {
        await updateMaterialCategory({ id: category.id, ...dto });
      } else {
        await createMaterialCategory(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar la categoría.");
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

      <Field label="Descripción" htmlFor="categoryDescription" required error={errors.description}>
        <TextInput
          id="categoryDescription"
          value={description}
          invalid={!!errors.description}
          placeholder="Ej: Herbicidas"
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
          {category?.id ? "Guardar cambios" : "Crear categoría"}
        </Button>
      </div>
    </form>
  );
}
