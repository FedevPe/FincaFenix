"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { Field, TextInput, SelectInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import { formatNumber, parseDecimal } from "@/lib/utils";
import type {
  AddDetailWorkOrderDTO,
  EmployeeDTO,
  ShowWorkOrderDTO,
} from "@/types/api";
import { addActivity } from "../../services/detailWorkOrder.service";
import { getEmployees } from "../../services/employee.service";
import { rendimientoModeLabel } from "../../constants";

interface AddActivityFormProps {
  workOrder: ShowWorkOrderDTO;
  onClose: () => void;
  onSaved: () => void;
}

export function AddActivityForm({
  workOrder,
  onClose,
  onSaved,
}: AddActivityFormProps) {
  const farmId = workOrder.farm?.id;

  const [employees, setEmployees] = useState<EmployeeDTO[]>([]);
  const [employeesError, setEmployeesError] = useState<string | null>(null);

  const [sectorId, setSectorId] = useState("");
  const [employeeId, setEmployeeId] = useState("");
  const [workedHours, setWorkedHours] = useState("");
  const [machinePasses, setMachinePasses] = useState("");
  const [producedAmount, setProducedAmount] = useState("");
  const [description, setDescription] = useState("");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const mode = workOrder.rendimientoMode;
  const isMaterialEfficiency = mode === "MaterialEfficiency";
  const isOutputPerManHour = mode === "OutputPerManHour";

  const sectors = workOrder.sectorList ?? [];
  const selectedSector = sectors.find((sector) => sector.id === Number(sectorId));

  useEffect(() => {
    if (!farmId) return;
    let active = true;
    void (async () => {
      try {
        const employeeList = await getEmployees(farmId);
        if (!active) return;
        setEmployees(employeeList);
      } catch (err) {
        if (!active) return;
        const pd = getProblemDetails(err);
        setEmployeesError(
          pd?.detail || "No se pudieron cargar los operarios de la finca.",
        );
      }
    })();
    return () => {
      active = false;
    };
  }, [farmId]);

  const validate = (): boolean => {
    const next: Record<string, string> = {};

    if (!sectorId) next.sector = "El sector trabajado es obligatorio.";
    if (!employeeId) next.employee = "El operario es obligatorio.";
    if (parseDecimal(workedHours) <= 0) {
      next.workedHours = "Las horas trabajadas deben ser mayores a cero.";
    }
    if (isMaterialEfficiency && parseDecimal(machinePasses) <= 0) {
      next.machinePasses = "La cantidad de maquinadas debe ser mayor a cero.";
    }
    if (isOutputPerManHour && parseDecimal(producedAmount) <= 0) {
      next.producedAmount = "La cantidad producida (kg) debe ser mayor a cero.";
    }
    if (!description.trim()) {
      next.description = "La descripción de la actividad es obligatoria.";
    }

    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!validate()) return;

    const dto: AddDetailWorkOrderDTO = {
      orderId: workOrder.id,
      employeeId: Number(employeeId),
      activityDate: new Date().toISOString(),
      info: {
        sectorWorkedId: Number(sectorId),
        machinePasses: parseDecimal(machinePasses),
        workedHours: parseDecimal(workedHours),
        producedAmount: producedAmount ? parseDecimal(producedAmount) : null,
        description: description.trim(),
      },
    };

    setSubmitting(true);
    try {
      await addActivity(dto);
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo registrar la actividad.");
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

      <Field label="Sector trabajado" htmlFor="sectorId" required error={errors.sector}>
        <SelectInput
          id="sectorId"
          value={sectorId}
          invalid={!!errors.sector}
          onChange={(event) => {
            setSectorId(event.target.value);
            setErrors((prev) => ({ ...prev, sector: "" }));
          }}
        >
          <option value="">Seleccione el sector</option>
          {sectors.map((sector) => (
            <option key={sector.id} value={sector.id}>
              {sector.sectorName}
              {sector.fruitName ? ` · ${sector.fruitName}` : ""}
              {sector.varietyName ? ` · ${sector.varietyName}` : ""}
              {sector.area != null
                ? ` · ${formatNumber(sector.area, { maximumFractionDigits: 2 })} ha`
                : ""}
            </option>
          ))}
        </SelectInput>
      </Field>

      {selectedSector ? (
        <div className="grid grid-cols-2 gap-3 rounded-md border bg-muted/40 px-3 py-2 sm:grid-cols-4">
          <div className="space-y-0.5">
            <span className="text-xs text-muted-foreground">Fruta</span>
            <span className="block text-sm">{selectedSector.fruitName ?? "—"}</span>
          </div>
          <div className="space-y-0.5">
            <span className="text-xs text-muted-foreground">Variedad</span>
            <span className="block text-sm">{selectedSector.varietyName ?? "—"}</span>
          </div>
          <div className="space-y-0.5">
            <span className="text-xs text-muted-foreground">Plantas</span>
            <span className="block text-sm tabular-nums">
              {selectedSector.numberPlants != null
                ? formatNumber(selectedSector.numberPlants)
                : "—"}
            </span>
          </div>
          <div className="space-y-0.5">
            <span className="text-xs text-muted-foreground">Superficie</span>
            <span className="block text-sm tabular-nums">
              {selectedSector.area != null
                ? `${formatNumber(selectedSector.area, { maximumFractionDigits: 2 })} ha`
                : "—"}
            </span>
          </div>
        </div>
      ) : null}

      <Field label="Operario" htmlFor="employeeId" required error={errors.employee}>
        {employeesError ? (
          <span className="text-xs text-destructive">{employeesError}</span>
        ) : employees.length === 0 ? (
          <span className="text-xs text-destructive">
            La finca no tiene operarios cargados.
          </span>
        ) : (
          <SelectInput
            id="employeeId"
            value={employeeId}
            invalid={!!errors.employee}
            onChange={(event) => {
              setEmployeeId(event.target.value);
              setErrors((prev) => ({ ...prev, employee: "" }));
            }}
          >
            <option value="">Seleccione el operario</option>
            {employees.map((employee) => (
              <option key={employee.id} value={employee.id}>
                {employee.name} {employee.lastName ?? ""}
              </option>
            ))}
          </SelectInput>
        )}
      </Field>

      <p className="text-xs text-muted-foreground">
        La fecha y hora se registran automáticamente al momento de guardar la
        actividad.
      </p>

      <Field
        label="Horas trabajadas"
        htmlFor="workedHours"
        required
        error={errors.workedHours}
      >
        <TextInput
          id="workedHours"
          inputMode="decimal"
          value={workedHours}
          invalid={!!errors.workedHours}
          placeholder="Ej: 4"
          onChange={(event) => {
            setWorkedHours(event.target.value);
            setErrors((prev) => ({ ...prev, workedHours: "" }));
          }}
        />
      </Field>

      {isMaterialEfficiency ? (
        <Field
          label="Maquinadas (cargas de tanque)"
          htmlFor="machinePasses"
          required
          error={errors.machinePasses}
        >
          <TextInput
            id="machinePasses"
            inputMode="decimal"
            value={machinePasses}
            invalid={!!errors.machinePasses}
            placeholder="Ej: 1"
            onChange={(event) => {
              setMachinePasses(event.target.value);
              setErrors((prev) => ({ ...prev, machinePasses: "" }));
            }}
          />
        </Field>
      ) : null}

      {isOutputPerManHour ? (
        <Field
          label="Cantidad producida (kg)"
          htmlFor="producedAmount"
          required
          error={errors.producedAmount}
        >
          <TextInput
            id="producedAmount"
            inputMode="decimal"
            value={producedAmount}
            invalid={!!errors.producedAmount}
            placeholder="Ej: 500"
            onChange={(event) => {
              setProducedAmount(event.target.value);
              setErrors((prev) => ({ ...prev, producedAmount: "" }));
            }}
          />
        </Field>
      ) : null}

      <Field
        label="Descripción"
        htmlFor="description"
        required
        error={errors.description}
        hint={mode ? `Modo de la tarea: ${rendimientoModeLabel(mode)}` : undefined}
      >
        <TextInput
          id="description"
          value={description}
          invalid={!!errors.description}
          placeholder="Detalle de la actividad realizada"
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
          Registrar actividad
        </Button>
      </div>
    </form>
  );
}
