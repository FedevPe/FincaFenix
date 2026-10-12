"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import {
  CheckboxRow,
  Field,
  TextInput,
} from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import type { EmployeeDTO, FarmDTO } from "@/types/api";
import { createEmployee, updateEmployee } from "../services/employee.service";

interface EmployeeFormProps {
  employee?: EmployeeDTO | null;
  farms: FarmDTO[];
  onClose: () => void;
  onSaved: () => void;
}

export function EmployeeForm({ employee, farms, onClose, onSaved }: EmployeeFormProps) {
  const [name, setName] = useState("");
  const [lastName, setLastName] = useState("");
  const [farmIds, setFarmIds] = useState<number[]>([]);
  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setName(employee?.name ?? "");
    setLastName(employee?.lastName ?? "");
    setFarmIds(employee?.farmIds ?? []);
    setErrors({});
    setSubmitError(null);
  }, [employee]);

  const toggleFarm = (farmId: number, checked: boolean) => {
    setFarmIds((prev) =>
      checked ? [...prev, farmId] : prev.filter((id) => id !== farmId),
    );
    setErrors((prev) => ({ ...prev, farmIds: "" }));
  };

  const handleSubmit = async () => {
    setSubmitError(null);
    const next: Record<string, string> = {};
    if (!name.trim()) next.name = "El nombre es obligatorio.";
    else if (name.trim().length > 50) next.name = "El nombre no puede superar 50 caracteres.";
    if (!lastName.trim()) next.lastName = "El apellido es obligatorio.";
    else if (lastName.trim().length > 50)
      next.lastName = "El apellido no puede superar 50 caracteres.";
    if (farmIds.length === 0) next.farmIds = "Debe asignar al menos una finca.";
    if (Object.keys(next).length > 0) {
      setErrors(next);
      return;
    }

    setSubmitting(true);
    try {
      const dto = { name: name.trim(), lastName: lastName.trim(), farmIds };
      if (employee?.id) {
        await updateEmployee({ id: employee.id, ...dto });
      } else {
        await createEmployee(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar el empleado.");
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
        <Field label="Nombre" htmlFor="employeeName" required error={errors.name}>
          <TextInput
            id="employeeName"
            value={name}
            invalid={!!errors.name}
            onChange={(event) => {
              setName(event.target.value);
              setErrors((prev) => ({ ...prev, name: "" }));
            }}
          />
        </Field>
        <Field label="Apellido" htmlFor="employeeLastName" required error={errors.lastName}>
          <TextInput
            id="employeeLastName"
            value={lastName}
            invalid={!!errors.lastName}
            onChange={(event) => {
              setLastName(event.target.value);
              setErrors((prev) => ({ ...prev, lastName: "" }));
            }}
          />
        </Field>
      </div>

      <Field label="Fincas asignadas" htmlFor="employeeFarms" required error={errors.farmIds}>
        {farms.length === 0 ? (
          <p className="text-sm text-muted-foreground">
            No hay fincas disponibles.
          </p>
        ) : (
          <div className="grid max-h-56 gap-2 overflow-y-auto rounded-md border p-3 sm:grid-cols-2">
            {farms.map((farm) => (
              <CheckboxRow
                key={farm.id}
                label={farm.name}
                checked={farmIds.includes(farm.id)}
                onChange={(checked) => toggleFarm(farm.id, checked)}
              />
            ))}
          </div>
        )}
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          {employee?.id ? "Guardar cambios" : "Crear empleado"}
        </Button>
      </div>
    </form>
  );
}
