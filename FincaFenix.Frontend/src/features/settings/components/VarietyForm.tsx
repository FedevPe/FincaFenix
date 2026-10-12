"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { CheckboxRow, Field, TextInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { FruitVarietyDTO } from "@/types/api";
import {
  createFruitVariety,
  updateFruitVariety,
} from "../services/fruit.service";

interface VarietyFormProps {
  variety: FruitVarietyDTO | null;
  onSaved: () => void;
  onCancel: () => void;
}

export function VarietyForm({ variety, onSaved, onCancel }: VarietyFormProps) {
  const [description, setDescription] = useState("");
  const [isOrganic, setIsOrganic] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    setDescription(variety?.description ?? "");
    setIsOrganic(variety?.isOrganic ?? false);
    setError(null);
  }, [variety]);

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!description.trim()) {
      setError("La descripción es obligatoria.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      const dto = {
        id: variety?.id ?? null,
        description: description.trim(),
        isOrganic,
      };
      if (variety) {
        await updateFruitVariety(dto);
      } else {
        await createFruitVariety(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudo guardar la variedad.");
    } finally {
      setSaving(false);
    }
  };

  return (
    <form className="space-y-4" onSubmit={handleSubmit}>
      {error ? (
        <div
          className="flex items-start gap-2 rounded-md bg-destructive/10 p-3 text-sm text-destructive"
          role="alert"
        >
          <AlertCircle className="mt-0.5 size-4 shrink-0" />
          <span>{error}</span>
        </div>
      ) : null}

      <Field label="Descripción" htmlFor="variety-description" required>
        <TextInput
          id="variety-description"
          value={description}
          maxLength={100}
          onChange={(event) => setDescription(event.target.value)}
          placeholder="Ej: Valencia"
        />
      </Field>

      <CheckboxRow
        label="Orgánica"
        description="Marcá si la variedad es orgánica."
        checked={isOrganic}
        onChange={setIsOrganic}
      />

      <div className="flex justify-end gap-2">
        <Button variant="ghost" type="button" onClick={onCancel}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={saving}>
          {variety ? "Guardar cambios" : "Crear variedad"}
        </Button>
      </div>
    </form>
  );
}
