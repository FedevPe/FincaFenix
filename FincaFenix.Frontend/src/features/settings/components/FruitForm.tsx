"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { CheckboxRow, Field, TextInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { FruitDTO, FruitVarietyDTO } from "@/types/api";
import {
  createFruit,
  getFruitVarieties,
  updateFruit,
} from "../services/fruit.service";

interface FruitFormProps {
  fruit: FruitDTO | null;
  onSaved: () => void;
  onCancel: () => void;
}

export function FruitForm({ fruit, onSaved, onCancel }: FruitFormProps) {
  const [description, setDescription] = useState("");
  const [varieties, setVarieties] = useState<FruitVarietyDTO[]>([]);
  const [varietyIds, setVarietyIds] = useState<number[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    void (async () => {
      try {
        setVarieties(await getFruitVarieties());
      } catch (err) {
        const pd = getProblemDetails(err);
        setError(pd?.detail || "No se pudieron cargar las variedades.");
      }
    })();
  }, []);

  useEffect(() => {
    setDescription(fruit?.description ?? "");
    setVarietyIds(fruit?.varietyIds ?? []);
    setError(null);
  }, [fruit]);

  const toggleVariety = (id: number, checked: boolean) => {
    setVarietyIds((current) =>
      checked ? [...current, id] : current.filter((value) => value !== id),
    );
  };

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
        varietyIds,
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

      <Field
        label="Variedades"
        htmlFor="fruit-varieties"
        hint="Seleccioná las variedades que pertenecen a esta fruta."
      >
        {varieties.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            No hay variedades cargadas. Creá variedades desde la pestaña
            &quot;Variedades&quot;.
          </p>
        ) : (
          <div className="grid max-h-56 gap-2 overflow-y-auto rounded-md border p-3 sm:grid-cols-2">
            {varieties.map((variety) => {
              const assignedElsewhere =
                variety.fruitId != null && variety.fruitId !== fruit?.id;
              return (
                <CheckboxRow
                  key={variety.id}
                  label={
                    variety.fruitName && fruit?.id !== variety.fruitId
                      ? `${variety.description} (en ${variety.fruitName})`
                      : variety.description
                  }
                  checked={varietyIds.includes(variety.id)}
                  onChange={(checked) => toggleVariety(variety.id, checked)}
                />
              );
            })}
          </div>
        )}
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
