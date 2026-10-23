"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { Field, TextInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { FruitDTO } from "@/types/api";
import { createFruit, updateFruit } from "../services/fruit.service";

interface FruitFormProps {
  fruit: FruitDTO | null;
  onSaved: () => void;
  onCancel: () => void;
}

export function FruitForm({ fruit, onSaved, onCancel }: FruitFormProps) {
  const [description, setDescription] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    setDescription(fruit?.description ?? "");
    setError(null);
  }, [fruit]);

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
        id: fruit?.id ?? null,
        description: description.trim(),
      };
      if (fruit) {
        await updateFruit(dto);
      } else {
        await createFruit(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudo guardar la fruta.");
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

      <Field label="Descripción" htmlFor="fruit-description" required>
        <TextInput
          id="fruit-description"
          value={description}
          invalid={Boolean(error)}
          maxLength={100}
          onChange={(event) => setDescription(event.target.value)}
          placeholder="Ej: Naranja"
        />
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" type="button" onClick={onCancel}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={saving}>
          {fruit ? "Guardar cambios" : "Crear fruta"}
        </Button>
      </div>
    </form>
  );
}
