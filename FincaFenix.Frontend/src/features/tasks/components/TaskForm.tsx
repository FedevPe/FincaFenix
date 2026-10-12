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
import type { TaskDTO } from "@/types/api";
import { RENDIMIENTO_MODES, rendimientoModeLabel } from "../constants";
import { createTask, updateTask } from "../services/task.service";

interface TaskFormProps {
  task?: TaskDTO | null;
  onClose: () => void;
  onSaved: () => void;
}

export function TaskForm({ task, onClose, onSaved }: TaskFormProps) {
  const [description, setDescription] = useState("");
  const [mode, setMode] = useState<string>("ManHours");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setDescription(task?.description ?? "");
    setMode(task?.rendimientoMode ?? "ManHours");
    setErrors({});
    setSubmitError(null);
  }, [task]);

  const validate = (): boolean => {
    const next: Record<string, string> = {};
    if (!description.trim()) {
      next.description = "La descripción de la tarea es obligatoria.";
    } else if (description.trim().length > 100) {
      next.description = "La descripción no puede superar los 100 caracteres.";
    }
    if (!mode) next.mode = "El modo de rendimiento es obligatorio.";
    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!validate()) return;

    setSubmitting(true);
    try {
      const dto = {
        description: description.trim(),
        rendimientoMode: mode,
      };
      if (task?.id) {
        await updateTask({ id: task.id, ...dto });
      } else {
        await createTask(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar la tarea.");
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

      <Field
        label="Descripción"
        htmlFor="description"
        required
        error={errors.description}
      >
        <TextInput
          id="description"
          value={description}
          invalid={!!errors.description}
          placeholder="Ej: Aplicación de herbicida mochila"
          onChange={(event) => {
            setDescription(event.target.value);
            setErrors((prev) => ({ ...prev, description: "" }));
          }}
        />
      </Field>

      <Field label="Modo de rendimiento" htmlFor="mode" required error={errors.mode}>
        <SelectInput
          id="mode"
          value={mode}
          invalid={!!errors.mode}
          onChange={(event) => {
            setMode(event.target.value);
            setErrors((prev) => ({ ...prev, mode: "" }));
          }}
        >
          {RENDIMIENTO_MODES.map((item) => (
            <option key={item} value={item}>
              {rendimientoModeLabel(item)}
            </option>
          ))}
        </SelectInput>
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          {task?.id ? "Guardar cambios" : "Crear tarea"}
        </Button>
      </div>
    </form>
  );
}