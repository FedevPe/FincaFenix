"use client";

import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { AlertCircle, RotateCcw, X } from "lucide-react";
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
import { useAutoSave } from "@/hooks/useAutoSave";

interface AddActivityFormProps {
  workOrder: ShowWorkOrderDTO;
  onClose: () => void;
  onSaved: () => void;
}

const addActivitySchema = z.object({
  sectorId: z.string().min(1, "El sector trabajado es obligatorio."),
  employeeId: z.string().min(1, "El operario es obligatorio."),
  workedHours: z.string().refine((val) => parseDecimal(val) > 0, "Las horas trabajadas deben ser mayores a cero."),
  machinePasses: z.string().optional(),
  producedAmount: z.string().optional(),
  percentageAreaWorked: z.string().refine(
    (val) => {
      const pct = parseDecimal(val);
      return pct > 0 && pct <= 100;
    },
    "El porcentaje debe estar entre 1 y 100.",
  ),
  description: z.string().min(1, "La descripción de la actividad es obligatoria."),
});

type AddActivityFormData = z.infer<typeof addActivitySchema>;

export function AddActivityForm({
  workOrder,
  onClose,
  onSaved,
}: AddActivityFormProps) {
  const farmId = workOrder.farm?.id;

  const [employees, setEmployees] = useState<EmployeeDTO[]>([]);
  const [employeesError, setEmployeesError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<AddActivityFormData>({
    resolver: zodResolver(addActivitySchema),
    mode: "onBlur",
    reValidateMode: "onBlur",
    defaultValues: {
      sectorId: "",
      employeeId: "",
      workedHours: "",
      machinePasses: "",
      producedAmount: "",
      percentageAreaWorked: "",
      description: "",
    },
  });

  const formData = watch();
  const autoSaveKey = `add-activity-draft-${workOrder.id}`;
  const { restore, clear } = useAutoSave<AddActivityFormData>({
    key: autoSaveKey,
    data: formData,
    enabled: true,
    debounceMs: 3000,
  });

  const [showRestorePrompt, setShowRestorePrompt] = useState(false);
  const [draftData, setDraftData] = useState<AddActivityFormData | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const mode = workOrder.rendimientoMode;
  const isMaterialEfficiency = mode === "MaterialEfficiency";
  const isOutputPerManHour = mode === "OutputPerManHour";

  const sectors = workOrder.sectorList ?? [];
  const selectedSector = sectors.find((sector) => sector.id === Number(formData.sectorId));

  const accumulatedPercentage = (workOrder.detailsWorkOrder ?? [])
    .filter((activity) => activity.sector?.id === Number(formData.sectorId))
    .reduce((sum, activity) => sum + (activity.percentageAreaWorked ?? 0), 0);
  const remainingPercentage = Math.max(0, 100 - accumulatedPercentage);
  const percentageValue = parseDecimal(formData.percentageAreaWorked);
  const workedArea =
    selectedSector?.area != null && percentageValue > 0
      ? (selectedSector.area * percentageValue) / 100
      : null;

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

  useEffect(() => {
    const draft = restore();
    if (draft) {
      setDraftData(draft);
      setShowRestorePrompt(true);
    }
  }, [restore]);

  const handleRestoreDraft = () => {
    if (draftData) {
      reset(draftData);
      clear();
    }
    setShowRestorePrompt(false);
    setDraftData(null);
  };

  const handleDiscardDraft = () => {
    clear();
    setShowRestorePrompt(false);
    setDraftData(null);
  };

  const onSubmit = async (data: AddActivityFormData) => {
    setSubmitError(null);

    const pct = parseDecimal(data.percentageAreaWorked);
    if (pct > remainingPercentage) {
      setSubmitError(
        `El sector ya tiene ${formatNumber(accumulatedPercentage, { maximumFractionDigits: 2 })}% trabajado; el máximo es ${formatNumber(remainingPercentage, { maximumFractionDigits: 2 })}%.`,
      );
      return;
    }
    if (isMaterialEfficiency && parseDecimal(data.machinePasses || "0") <= 0) {
      setSubmitError("La cantidad de maquinadas debe ser mayor a cero.");
      return;
    }
    if (isOutputPerManHour && parseDecimal(data.producedAmount || "0") <= 0) {
      setSubmitError("La cantidad producida (kg) debe ser mayor a cero.");
      return;
    }

    const dto: AddDetailWorkOrderDTO = {
      orderId: workOrder.id,
      employeeId: Number(data.employeeId),
      activityDate: new Date().toISOString(),
      info: {
        sectorWorkedId: Number(data.sectorId),
        machinePasses: parseDecimal(data.machinePasses),
        workedHours: parseDecimal(data.workedHours),
        percentageAreaWorked: pct,
        producedAmount: data.producedAmount ? parseDecimal(data.producedAmount) : null,
        description: data.description.trim(),
      },
    };

    setSubmitting(true);
    try {
      await addActivity(dto);
      clear();
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
      onSubmit={handleSubmit(onSubmit)}
      noValidate
    >
      {showRestorePrompt && draftData && (
        <div className="flex items-start gap-3 rounded-md border border-yellow-500/40 bg-yellow-50/50 px-3 py-3" role="alert">
          <RotateCcw className="size-5 shrink-0 text-yellow-600 mt-0.5" aria-hidden />
          <div className="flex-1 min-w-0">
            <p className="text-sm font-medium text-yellow-800">Se encontró un borrador guardado automáticamente</p>
            <p className="text-xs text-yellow-700 mt-0.5">¿Desea restaurar los datos del borrador para esta orden de trabajo?</p>
          </div>
          <div className="flex gap-2 shrink-0">
            <Button variant="ghost" size="sm" onClick={handleDiscardDraft}>
              <X className="size-3.5" aria-hidden />
              <span className="sr-only">Descartar</span>
            </Button>
            <Button variant="secondary" size="sm" onClick={handleRestoreDraft}>
              Restaurar
            </Button>
          </div>
        </div>
      )}

      {submitError ? (
        <div
          className="flex items-center gap-2 rounded-md border border-destructive/40 bg-destructive/10 px-3 py-2 text-sm text-destructive"
          role="alert"
        >
          <AlertCircle className="size-4 shrink-0" aria-hidden />
          <span>{submitError}</span>
        </div>
      ) : null}

      <Field label="Sector trabajado" htmlFor="sectorId" required error={errors.sectorId?.message}>
        <SelectInput
          id="sectorId"
          {...register("sectorId")}
          invalid={!!errors.sectorId}
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

      <Field
        label="Porcentaje de área trabajada"
        htmlFor="percentageAreaWorked"
        required
        error={errors.percentageAreaWorked?.message}
        hint={
          selectedSector
            ? `Ya trabajado: ${formatNumber(accumulatedPercentage, { maximumFractionDigits: 2 })}% · Máx. esta actividad: ${formatNumber(remainingPercentage, { maximumFractionDigits: 2 })}%`
            : "Seleccione primero el sector"
        }
      >
        <TextInput
          id="percentageAreaWorked"
          inputMode="decimal"
          {...register("percentageAreaWorked")}
          invalid={!!errors.percentageAreaWorked}
          placeholder="Ej: 25"
        />
        {workedArea != null && selectedSector?.area != null ? (
          <span className="text-xs text-muted-foreground">
            {formatNumber(percentageValue, { maximumFractionDigits: 2 })}% de{" "}
            {formatNumber(selectedSector.area, { maximumFractionDigits: 2 })} ha ={" "}
            {formatNumber(workedArea, { maximumFractionDigits: 2 })} ha
          </span>
        ) : null}
      </Field>

      <Field label="Operario" htmlFor="employeeId" required error={errors.employeeId?.message}>
        {employeesError ? (
          <span className="text-xs text-destructive">{employeesError}</span>
        ) : employees.length === 0 ? (
          <span className="text-xs text-destructive">
            La finca no tiene operarios cargados.
          </span>
        ) : (
          <SelectInput
            id="employeeId"
            {...register("employeeId")}
            invalid={!!errors.employeeId}
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
        error={errors.workedHours?.message}
      >
        <TextInput
          id="workedHours"
          inputMode="decimal"
          {...register("workedHours")}
          invalid={!!errors.workedHours}
          placeholder="Ej: 4"
        />
      </Field>

      {isMaterialEfficiency ? (
        <Field
          label="Maquinadas (cargas de tanque)"
          htmlFor="machinePasses"
          required
          error={errors.machinePasses?.message}
        >
          <TextInput
            id="machinePasses"
            inputMode="decimal"
            {...register("machinePasses")}
            invalid={!!errors.machinePasses}
            placeholder="Ej: 1"
          />
        </Field>
      ) : null}

      {isOutputPerManHour ? (
        <Field
          label="Cantidad producida (kg)"
          htmlFor="producedAmount"
          required
          error={errors.producedAmount?.message}
        >
          <TextInput
            id="producedAmount"
            inputMode="decimal"
            {...register("producedAmount")}
            invalid={!!errors.producedAmount}
            placeholder="Ej: 500"
          />
        </Field>
      ) : null}

      <Field
        label="Descripción"
        htmlFor="description"
        required
        error={errors.description?.message}
        hint={mode ? `Modo de la tarea: ${rendimientoModeLabel(mode)}` : undefined}
      >
        <TextInput
          id="description"
          {...register("description")}
          invalid={!!errors.description}
          placeholder="Detalle de la actividad realizada"
        />
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose} disabled={submitting || isSubmitting}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting || isSubmitting}>
          Registrar actividad
        </Button>
      </div>
    </form>
  );
}