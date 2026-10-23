"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { useForm, useFieldArray, type FieldValues } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { AlertCircle, ArrowLeft, Calendar as CalendarIcon, FlaskConical, Plus, Trash2 } from "lucide-react";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/common/Button";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { Field, TextInput, SelectInput } from "@/components/common/FormControls";
import { DataTable, type Column } from "@/components/common/DataTable";
import { Modal } from "@/components/common/Modal";
import { useAuth } from "@/app/providers/AuthProvider";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy, formatNumber, parseDecimal, cn } from "@/lib/utils";
import { Popover, PopoverTrigger, PopoverContent } from "@/components/ui/popover";
import { Calendar } from "@/components/ui/calendar";
import { format, parseISO } from "date-fns";
import type {
  DetailSectorFarmDTO,
  FarmDTO,
  MachineRecipeDTO,
  MaterialCategoryDTO,
  MaterialRecipeDTO,
  TaskDTO,
  WorkOrderDTO,
} from "@/types/api";
import {
  createWorkOrder,
  getFarms,
  getMachines,
  getSectorsByFarm,
  getTasks,
} from "../services/workOrder.service";
import {
  getMaterialCategories,
  getMaterials,
} from "../../materials/services/material.service";
import { getStockByFarm } from "../../inventory/services/inventory.service";
import type { StockDTO } from "../../../types/api/inventory";
import {
  convertAmount,
  materialLabel,
  unitCodeFor,
  unitFamilyOf,
} from "../../materials/material.utils";
import type { UnitFamily } from "../../materials/material.utils";
import { useAutoSave } from "@/hooks/useAutoSave";

interface RecipeRow {
  key: number;
  categoryId: string;
  materialId: string;
  amountRequired: string;
  amountRequiredUnit: string;
  brand: string;
  pestDisease: string;
}

const RECIPE_UNITS: Array<{ value: string; label: string }> = [
  { value: "lts", label: "Litros (lts)" },
  { value: "kg", label: "Kilogramos (kg)" },
  { value: "gr", label: "Gramos (gr)" },
  { value: "cc", label: "Centímetros cúbicos (cc)" },
];

const DOSE_UNITS_BY_FAMILY: Record<
  UnitFamily,
  Array<{ value: string; label: string }>
> = {
  volume: [
    { value: "lts", label: "Litros (lts)" },
    { value: "cc", label: "Centímetros cúbicos (cc)" },
  ],
  mass: [
    { value: "kg", label: "Kilogramos (kg)" },
    { value: "gr", label: "Gramos (gr)" },
  ],
  count: [{ value: "unidad", label: "Unidades" }],
  length: [{ value: "metro", label: "Metros (m)" }],
  package: [
    { value: "bolsa", label: "Bolsas" },
    { value: "caja", label: "Cajas" },
  ],
  unknown: RECIPE_UNITS,
};

function doseUnitsFor(
  material: MaterialRecipeDTO | undefined,
): Array<{ value: string; label: string }> {
  return DOSE_UNITS_BY_FAMILY[unitFamilyOf(material?.unitOfMeasure)];
}

function defaultDoseUnitFor(material: MaterialRecipeDTO | undefined): string {
  const allowed = doseUnitsFor(material);
  const base = unitCodeFor(material?.unitOfMeasure);
  if (allowed.some((unit) => unit.value === base)) return base;
  return allowed[0]?.value ?? "lts";
}

function theoreticalPasses(
  areaTotal: number,
  trv: number,
  volumeMachine: number,
): number | null {
  if (areaTotal <= 0 || trv <= 0 || volumeMachine <= 0) return null;
  return (areaTotal * trv) / volumeMachine;
}

interface ServerFieldError {
  propertyName: string;
  errorMessage: string;
}

function computeEstimatedAmount(
  row: RecipeRow,
  passes: number | null,
): { value: number; unit: string } {
  const unit = row.amountRequiredUnit || "lts";
  if (passes == null || !row.materialId) return { value: 0, unit };
  const dose = parseDecimal(row.amountRequired);
  return { value: dose * passes, unit };
}

const recipeRowSchema = z.object({
  key: z.number(),
  categoryId: z.string(),
  materialId: z.string(),
  amountRequired: z.string(),
  amountRequiredUnit: z.string(),
  brand: z.string(),
  pestDisease: z.string(),
});

function buildCreateWorkOrderSchema(
  allowsRecipeForTask: (taskId: string) => boolean,
) {
  return z
    .object({
      taskId: z.string().min(1, "La tarea es obligatoria."),
      farmId: z.string().min(1, "La finca es obligatoria."),
      startDate: z.string().min(1, "La fecha de inicio es obligatoria."),
      description: z.string().optional(),
      selectedSectors: z
        .array(z.number())
        .min(1, "Debe seleccionar al menos un sector."),
      recipeEnabled: z.boolean(),
      machineId: z.string().optional(),
      volumeMachine: z.string().optional(),
      trv: z.string().optional(),
      rows: z.array(recipeRowSchema),
    })
    .refine(
      (data) => {
        if (!data.recipeEnabled) return true;
        if (!data.machineId) return false;
        if (!data.volumeMachine || parseDecimal(data.volumeMachine) <= 0) return false;
        const validRows = data.rows.filter(
          (row) => row.materialId && parseDecimal(row.amountRequired) > 0,
        );
        return validRows.length > 0;
      },
      {
        message: "La receta debe tener al menos un material con cantidades completas.",
        path: ["recipe"],
      },
    )
    .refine(
      (data) => !data.recipeEnabled || allowsRecipeForTask(data.taskId),
      {
        message:
          "La tarea seleccionada no admite receta. Solo las tareas con rendimiento por eficiencia de material permiten receta.",
        path: ["recipeEnabled"],
      },
    );
}

type CreateWorkOrderFormData = z.infer<
  ReturnType<typeof buildCreateWorkOrderSchema>
>;

interface ServerProblemDetails {
  detail?: string;
  status?: number;
  errors?: ServerFieldError[] | Record<string, unknown>;
}

function collectServerErrors(pd: ServerProblemDetails | null): string | null {
  if (!pd?.errors) return null;
  if (Array.isArray(pd.errors)) {
    const messages = (pd.errors as ServerFieldError[])
      .map((error) => error.errorMessage)
      .filter(Boolean);
    return messages.length ? messages.join(" — ") : null;
  }
  if (typeof pd.errors === "object") {
    const messages = Object.values(pd.errors as Record<string, unknown>)
      .flat()
      .filter((value): value is string => typeof value === "string");
    return messages.length ? messages.join(" — ") : null;
  }
  return null;
}

const AUTO_SAVE_KEY = "create-workorder-draft";

export default function CreateWorkOrder() {
  const router = useRouter();
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "WORKORDER_CREATE");
  const canReadMaterials = hasPolicy(user?.policies, "MATERIAL_READ");
  const canReadMachines = hasPolicy(user?.policies, "MACHINE_READ");

  const [tasks, setTasks] = useState<TaskDTO[]>([]);
  const [farms, setFarms] = useState<FarmDTO[]>([]);
  const [machines, setMachines] = useState<MachineRecipeDTO[]>([]);
  const [materials, setMaterials] = useState<MaterialRecipeDTO[]>([]);
  const [categories, setCategories] = useState<MaterialCategoryDTO[]>([]);
  const [sectors, setSectors] = useState<DetailSectorFarmDTO[]>([]);
  const [stock, setStock] = useState<StockDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [sectorsError, setSectorsError] = useState<string | null>(null);
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [showRestoreModal, setShowRestoreModal] = useState(false);
  const [draftData, setDraftData] = useState<CreateWorkOrderFormData | null>(null);

  const schema = useMemo(
    () =>
      buildCreateWorkOrderSchema(
        (taskId) =>
          tasks.find((task) => task.id === Number(taskId))?.rendimientoMode ===
          "MaterialEfficiency",
      ),
    [tasks],
  );

  const {
    register,
    handleSubmit,
    watch,
    setValue,
    reset,
    control,
    formState: { errors, isSubmitting: isFormSubmitting },
  } = useForm<CreateWorkOrderFormData>({
    resolver: zodResolver(schema),
    mode: "onBlur",
    reValidateMode: "onBlur",
    defaultValues: {
      taskId: "",
      farmId: "",
      startDate: (() => {
        const today = new Date();
        return `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, "0")}-${String(
          today.getDate(),
        ).padStart(2, "0")}`;
      })(),
      description: "",
      selectedSectors: [],
      recipeEnabled: false,
      machineId: "",
      volumeMachine: "",
      trv: "",
      rows: [],
    },
  });

  const { fields: recipeRows, append, remove, update } = useFieldArray({
    control,
    name: "rows",
  });

  const formData = watch();
  const { restore, clear } = useAutoSave<CreateWorkOrderFormData>({
    key: AUTO_SAVE_KEY,
    data: formData,
    enabled: true,
    debounceMs: 3000,
  });

  const selectedTask = tasks.find((task) => task.id === Number(formData.taskId));
  const rendimientoMode = selectedTask?.rendimientoMode;
  const requiresRecipe = rendimientoMode === "MaterialEfficiency";
  const allowsRecipe = rendimientoMode === "MaterialEfficiency";

  useEffect(() => {
    let active = true;

    const load = async () => {
      setLoading(true);
      setLoadError(null);
      try {
        const [taskList, farmList] = await Promise.all([getTasks(), getFarms()]);
        if (!active) return;
        setTasks(taskList);
        setFarms(farmList);
      } catch (err) {
        if (!active) return;
        const pd = getProblemDetails(err);
        setLoadError(
          pd?.detail || "No se pudieron cargar los datos para crear la orden.",
        );
      } finally {
        if (active) setLoading(false);
      }
    };

    void load();
    return () => {
      active = false;
    };
  }, []);

  useEffect(() => {
    if (!canReadMaterials || !canReadMachines) return;
    let active = true;
    void (async () => {
      try {
        const [machineList, materialList, categoryList] = await Promise.all([
          getMachines(),
          getMaterials(),
          getMaterialCategories(),
        ]);
        if (!active) return;
        setMachines(machineList);
        setMaterials(materialList);
        setCategories(categoryList);
      } catch {
        // La receta se considera opcional; el error se reflejará al intentar usarla.
      }
    })();
    return () => {
      active = false;
    };
  }, [canReadMaterials, canReadMachines]);

  useEffect(() => {
    if (!formData.farmId) {
      setSectors([]);
      setStock([]);
      setValue("selectedSectors", [], { shouldValidate: true });
      return;
    }
    setSectorsError(null);
    let active = true;
    void (async () => {
      try {
        const [sectorList, stockList] = await Promise.all([
          getSectorsByFarm(Number(formData.farmId)),
          getStockByFarm(Number(formData.farmId)),
        ]);
        if (!active) return;
        setSectors(sectorList);
        setStock(stockList);
        setValue("selectedSectors", [], { shouldValidate: true });
      } catch (err) {
        if (!active) return;
        const pd = getProblemDetails(err);
        setSectorsError(
          pd?.detail || "No se pudieron cargar los sectores de la finca.",
        );
      }
    })();
    return () => {
      active = false;
    };
  }, [formData.farmId, setValue]);

  useEffect(() => {
    setValue("recipeEnabled", allowsRecipe, { shouldValidate: true });
  }, [allowsRecipe, setValue]);

  useEffect(() => {
    const draft = restore();
    if (draft) {
      setDraftData(draft);
      setShowRestoreModal(true);
    }
  }, [restore]);

  const sortedCategories = useMemo(
    () => [...categories].sort((a, b) => a.description.localeCompare(b.description)),
    [categories],
  );

  const sectorGroups = useMemo(() => {
    const map = new Map<string, DetailSectorFarmDTO[]>();
    for (const sector of sectors) {
      const key = sector.fruitName ?? "Sin fruta";
      const list = map.get(key);
      if (list) list.push(sector);
      else map.set(key, [sector]);
    }
    return [...map.entries()];
  }, [sectors]);

  const addRow = () => {
    append({
      key: Date.now() + Math.random(),
      categoryId: "",
      materialId: "",
      amountRequired: "",
      amountRequiredUnit: "lts",
      brand: "",
      pestDisease: "",
    });
  };

  const onSelectMaterial = (index: number, materialId: string) => {
    const material = materials.find((m) => m.id === Number(materialId));
    const currentRow = recipeRows[index];
    if (currentRow) {
      update(index, {
        ...currentRow,
        materialId,
        amountRequiredUnit: materialId ? defaultDoseUnitFor(material) : "lts",
        brand: material?.brand ?? "",
      });
    }
  };

  const addSector = (id: number) => {
    const current = watch("selectedSectors");
    if (!current.includes(id)) {
      setValue("selectedSectors", [...current, id], { shouldValidate: true });
    }
  };

  const removeSector = (id: number) => {
    const current = watch("selectedSectors");
    setValue("selectedSectors", current.filter((value: number) => value !== id), { shouldValidate: true });
  };

  const handleRestoreDraft = () => {
    if (draftData) {
      reset(draftData);
      clear();
    }
    setShowRestoreModal(false);
    setDraftData(null);
  };

  const handleDiscardDraft = () => {
    clear();
    setShowRestoreModal(false);
    setDraftData(null);
  };

  const onSubmit = async (data: CreateWorkOrderFormData) => {
    setSubmitError(null);

    const now = new Date();
    const start = new Date(`${data.startDate}T00:00:00`);

    const areaTotal = sectors
      .filter((sector) => data.selectedSectors.includes(sector.id))
      .reduce((sum, sector) => sum + Number(sector.area || 0), 0);
    const passes = theoreticalPasses(
      areaTotal,
      parseDecimal(data.trv),
      parseDecimal(data.volumeMachine),
    );

    const recipeDetails =
      data.recipeEnabled && data.rows.length > 0
        ? data.rows
            .filter((row) => row.materialId)
            .map((row) => {
              const material = materials.find(
                (m) => m.id === Number(row.materialId),
              );
              const estimated = computeEstimatedAmount(row, passes);
              return {
                categoryId:
                  Number(row.categoryId) ||
                  material?.category?.id ||
                  material?.categoryId ||
                  0,
                materialId: Number(row.materialId),
                amountRequired: parseDecimal(row.amountRequired),
                amountRequiredUnit:
                  row.amountRequiredUnit || defaultDoseUnitFor(material),
                estimatedAmount: estimated.value,
                estimatedAmountUnit: estimated.unit,
                brand: row.brand || material?.brand || "",
                pestDisease: row.pestDisease,
              };
            })
        : [];

    const payload: WorkOrderDTO = {
      taskId: Number(data.taskId),
      farmId: Number(data.farmId),
      description: data.description.trim() || undefined,
      sectorList: sectors
        .filter((sector) => data.selectedSectors.includes(sector.id))
        .map((sector) => ({ ...sector, selected: true })),
      createdDate: now.toISOString(),
      startDate: start.toISOString(),
      endDate: now.toISOString(),
      status: "Pendiente",
      totalArea: areaTotal,
      isDeleted: false,
      recipe:
        data.recipeEnabled && recipeDetails.length > 0
          ? {
              machineId: Number(data.machineId),
              volumeMachine: parseDecimal(data.volumeMachine),
              volumeMachineUnit: "lts",
              trv: parseDecimal(data.trv),
              status: "Activo",
              details: recipeDetails,
            }
          : null,
    };

    setSubmitting(true);
    try {
      const result = await createWorkOrder(payload);
      if (result.success) {
        clear();
        router.push("/work-orders?created=1");
        return;
      }
      setSubmitError(
        result.errors.map((error) => error.errorMessage).join(" — ") ||
          "No se pudo crear la orden de trabajo.",
      );
    } catch (err) {
      const pd = getProblemDetails(err) as ServerProblemDetails | null;
      const messages = collectServerErrors(pd);
      setSubmitError(
        messages || pd?.detail || "No se pudo crear la orden de trabajo.",
      );
    } finally {
      setSubmitting(false);
    }
  };

  if (!canCreate) {
    return (
      <EmptyState
        title="Sin permiso"
        description="Su usuario no tiene permisos para crear órdenes de trabajo."
      />
    );
  }

  if (loading) {
    return <LoadingState label="Cargando datos de configuración..." />;
  }

  if (loadError) {
    return <ErrorState message={loadError} onRetry={() => router.refresh()} />;
  }

  const totalSelectedArea = sectors
    .filter((sector) => formData.selectedSectors.includes(sector.id))
    .reduce((sum, sector) => sum + Number(sector.area || 0), 0);

  const passes = theoreticalPasses(
    totalSelectedArea,
    parseDecimal(formData.trv),
    parseDecimal(formData.volumeMachine),
  );

  const selectedSectorRows = sectors.filter((sector) =>
    formData.selectedSectors.includes(sector.id),
  );

  const sectorColumns: Column<DetailSectorFarmDTO>[] = [
    { key: "sector", header: "Sector", render: (sector) => sector.sectorName },
    { key: "fruit", header: "Fruta", render: (sector) => sector.fruitName ?? "—" },
    {
      key: "variety",
      header: "Variedad",
      render: (sector) => sector.varietyName ?? "—",
    },
    {
      key: "plants",
      header: "Plantas",
      align: "right",
      render: (sector) =>
        sector.numberPlants != null ? formatNumber(sector.numberPlants) : "—",
    },
    {
      key: "area",
      header: "Superficie (ha)",
      align: "right",
      render: (sector) =>
        formatNumber(Number(sector.area ?? 0), { maximumFractionDigits: 2 }),
    },
    {
      key: "actions",
      header: "",
      align: "right",
      render: (sector) => (
        <button
          type="button"
          aria-label={`Quitar ${sector.sectorName}`}
          className="rounded-md p-1 text-muted-foreground hover:text-destructive"
          onClick={() => removeSector(sector.id)}
        >
          <Trash2 className="size-3.5" />
        </button>
      ),
    },
  ];

  return (
    <div className="space-y-6">
      <Button variant="outline" icon={ArrowLeft} onClick={() => router.push("/work-orders")}>
        Volver al listado
      </Button>

      <PageHeader
        title="Nueva orden de trabajo"
        subtitle="Complete los datos de la orden. Si la fecha de inicio es futura, la orden se creará como Pendiente."
      />

      {submitError ? (
        <div
          className="flex items-center gap-2 rounded-md border border-destructive/40 bg-destructive/10 px-3 py-2 text-sm text-destructive"
          role="alert"
        >
          <AlertCircle className="size-4 shrink-0" aria-hidden />
          <span>{submitError}</span>
        </div>
      ) : null}

      <form
        className="space-y-6"
        onSubmit={handleSubmit(onSubmit)}
        noValidate
      >
        <section className="space-y-4 rounded-lg border bg-card p-4">
          <h2 className="text-base font-semibold">Datos generales</h2>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field label="Tarea" htmlFor="taskId" required error={errors.taskId?.message}>
              <SelectInput
                id="taskId"
                {...register("taskId")}
                invalid={!!errors.taskId}
              >
                <option value="">Seleccione la tarea</option>
                {tasks.map((task) => (
                  <option key={task.id} value={task.id}>
                    {task.description}
                  </option>
                ))}
              </SelectInput>
            </Field>

            <Field label="Finca" htmlFor="farmId" required error={errors.farmId?.message}>
              <SelectInput
                id="farmId"
                {...register("farmId")}
                invalid={!!errors.farmId}
              >
                <option value="">Seleccione la finca</option>
                {farms.map((farm) => (
                  <option key={farm.id} value={farm.id}>
                    {farm.name}
                  </option>
                ))}
              </SelectInput>
            </Field>

            <Field
              label="Fecha de inicio"
              htmlFor="startDate"
              required
              error={errors.startDate?.message}
              hint="Puede dejar la fecha actual o elegir una fecha futura."
            >
              <Popover>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      errors.startDate && "border-destructive focus:border-destructive focus:ring-destructive/20"
                    )}
                    type="button"
                    id="startDate"
                    aria-invalid={!!errors.startDate}
                    aria-describedby={errors.startDate ? "startDate-error" : undefined}
                  >
                    <CalendarIcon className="mr-2 h-4 w-4" aria-hidden />
                    {formData.startDate
                      ? format(parseISO(formData.startDate), "dd/MM/yyyy")
                      : "Seleccionar fecha"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={formData.startDate ? parseISO(formData.startDate) : undefined}
                    onSelect={(date) =>
                      date && setValue("startDate", format(date, "yyyy-MM-dd"), { shouldValidate: true })
                    }
                  />
                </PopoverContent>
              </Popover>
              {errors.startDate && (
                <p id="startDate-error" className="mt-1 text-sm text-destructive" role="alert">
                  {errors.startDate.message}
                </p>
              )}
            </Field>

            <Field label="Descripción" htmlFor="description">
              <TextInput
                id="description"
                {...register("description")}
                placeholder="Detalle opcional de la orden"
              />
            </Field>
          </div>
        </section>

        <section className="space-y-4 rounded-lg border bg-card p-4">
          <h2 className="text-base font-semibold">Sectores de la finca</h2>

          {!formData.farmId ? (
            <p className="text-sm text-muted-foreground">
              Seleccione una finca para cargar sus sectores.
            </p>
          ) : sectorsError ? (
            <ErrorState message={sectorsError} />
          ) : sectors.length === 0 ? (
            <p className="text-sm text-muted-foreground">
              La finca seleccionada no tiene sectores cargados.
            </p>
          ) : (
            <div className="space-y-3">
              <div className="max-w-md space-y-1.5">
                <label className="text-sm font-medium" htmlFor="sector-add">
                  Agregar sector
                </label>
                <SelectInput
                  id="sector-add"
                  className="w-full"
                  value=""
                  onChange={(event) => {
                    if (event.target.value) addSector(Number(event.target.value));
                  }}
                >
                  <option value="">Seleccione un sector</option>
                  {sectorGroups.map(([fruit, list]) => (
                    <optgroup key={fruit} label={fruit}>
                      {list.map((sector) => (
                        <option key={sector.id} value={sector.id}>
                          {sector.sectorName}
                          {sector.varietyName ? ` · ${sector.varietyName}` : ""} ·{" "}
                          {formatNumber(sector.area ?? 0, { maximumFractionDigits: 2 })} ha
                        </option>
                      ))}
                    </optgroup>
                  ))}
                </SelectInput>
              </div>

              {selectedSectorRows.length === 0 ? (
                <p className="text-sm text-muted-foreground">
                  Todavía no seleccionó sectores.
                </p>
              ) : (
                <DataTable
                  columns={sectorColumns}
                  rows={selectedSectorRows}
                  rowKey={(sector) => sector.id}
                  emptyMessage="Sin sectores seleccionados"
                />
              )}

              <p className="text-sm text-muted-foreground">
                Sectores seleccionados: {formData.selectedSectors.length} · Área total:{" "}
                {formatNumber(totalSelectedArea, { maximumFractionDigits: 2 })} ha
              </p>
              {errors.selectedSectors ? (
                <p className="text-xs text-destructive" role="alert">
                  {errors.selectedSectors.message}
                </p>
              ) : null}
            </div>
          )}
        </section>

        <section className="space-y-4 rounded-lg border bg-card p-4">
          <div className="flex flex-wrap items-center justify-between gap-2">
            <h2 className="flex items-center gap-2 text-base font-semibold">
              <FlaskConical className="size-4" aria-hidden />
              Receta
            </h2>
            <label className="flex items-center gap-2 text-sm">
              <input
                type="checkbox"
                {...register("recipeEnabled")}
                checked={allowsRecipe}
                disabled
                className="size-4"
              />
              <span>
                {!selectedTask
                  ? "Seleccione una tarea para definir la receta"
                  : requiresRecipe
                    ? "Incluir receta (obligatoria para esta tarea)"
                    : "La tarea seleccionada no admite receta"}
              </span>
            </label>
          </div>

          {errors.recipeEnabled ? (
            <p className="text-xs text-destructive" role="alert">
              {errors.recipeEnabled.message}
            </p>
          ) : null}

          {!allowsRecipe ? (
            <p className="text-sm text-muted-foreground">
              {selectedTask
                ? "Esta tarea no utiliza receta: la orden no reservará materiales ni generará costos."
                : "Seleccione una tarea con rendimiento por eficiencia de material para cargar una receta."}
            </p>
          ) : (
            <div className="space-y-4">
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <Field
                  label="Máquina"
                  htmlFor="machineId"
                  required
                  error={errors.machineId?.message}
                >
                  <SelectInput
                    id="machineId"
                    {...register("machineId")}
                    invalid={!!errors.machineId}
                  >
                    <option value="">Seleccione la máquina</option>
                    {machines.map((machine) => (
                      <option key={machine.id} value={machine.id}>
                        {machine.name ?? `Máquina ${machine.id}`}
                      </option>
                    ))}
                  </SelectInput>
                </Field>

                <Field
                  label="Volumen de la máquina (lts)"
                  htmlFor="volumeMachine"
                  required
                  error={errors.volumeMachine?.message}
                  hint="La unidad de la máquina siempre es litros."
                >
                  <TextInput
                    id="volumeMachine"
                    inputMode="decimal"
                    {...register("volumeMachine")}
                    invalid={!!errors.volumeMachine}
                    placeholder="Ej: 1000"
                  />
                </Field>
              </div>

              <Field
                label="TRV (litros por hectárea)"
                htmlFor="trv"
                hint="Total de riego o volumen fungicida en lts/ha."
              >
                <TextInput
                  id="trv"
                  inputMode="decimal"
                  {...register("trv")}
                  placeholder="Ej: 1"
                />
              </Field>

              <div className="flex flex-wrap items-center justify-between gap-2">
                <h3 className="text-sm font-semibold">Materiales</h3>
                <Button variant="secondary" size="sm" icon={Plus} onClick={addRow}>
                  Agregar material
                </Button>
              </div>

              {recipeRows.length === 0 ? (
                <p className="text-sm text-muted-foreground">
                  Agregue al menos un material con su cantidad requerida. La
                  cantidad estimada se calcula automáticamente.
                </p>
              ) : (
                <div className="space-y-3">
                  {recipeRows.map((row, index) => {
                    // Usar formData.rows para valores reactivos (amountRequired, etc.)
                    const formRow = formData.rows[index];
                    const rowMaterials = row.categoryId
                      ? materials.filter(
                          (m) =>
                            (m.category?.id ?? m.categoryId) ===
                            Number(row.categoryId),
                        )
                      : [];
                    const rowMaterial = materials.find(
                      (m) => m.id === Number(row.materialId),
                    );
                    const rowDoseUnits = doseUnitsFor(rowMaterial);
                    const estimated = computeEstimatedAmount(formRow, passes);
                    
                    // Verificar stock disponible para el material seleccionado.
                    // El stock está en la unidad base del material; la estimación
                    // llega en la unidad de dosis elegida, por lo que hay que
                    // convertirla antes de comparar.
                    const materialStock = rowMaterial
                      ? stock.find((s) => s.materialId === rowMaterial.id)
                      : undefined;
                    const stockDisponible = materialStock?.stockDisponible ?? 0;
                    const baseUnit =
                      rowMaterial?.unitOfMeasure ?? materialStock?.unitOfMeasure;
                    const estimatedInBaseUnit = convertAmount(
                      estimated.value,
                      estimated.unit,
                      baseUnit,
                    );
                    const hayStockInsuficiente =
                      passes != null &&
                      rowMaterial != null &&
                      estimatedInBaseUnit > stockDisponible;
                    return (
                      <div
                        key={row.key}
                        className="grid grid-cols-1 gap-2 rounded-md border p-3 lg:grid-cols-[1fr_1fr_auto]"
                      >
                        <SelectInput
                          aria-label="Categoría"
                          className="w-full"
                          value={row.categoryId}
                          onChange={(event) =>
                            update(index, {
                              ...row,
                              categoryId: event.target.value,
                              materialId: "",
                            })
                          }
                        >
                          <option value="">Categoría</option>
                          {sortedCategories.map((category) => (
                            <option key={category.id} value={category.id}>
                              {category.description}
                            </option>
                          ))}
                        </SelectInput>

                        <SelectInput
                          aria-label="Material"
                          className="w-full"
                          value={row.materialId}
                          disabled={!row.categoryId}
                          onChange={(event) =>
                            onSelectMaterial(index, event.target.value)
                          }
                        >
                          <option value="">Material</option>
                          {rowMaterials.map((material) => (
                            <option key={material.id} value={material.id}>
                              {materialLabel(material)}
                            </option>
                          ))}
                        </SelectInput>

                        <button
                          type="button"
                          className="inline-flex size-9 items-center justify-center justify-self-end rounded-md text-muted-foreground hover:bg-accent hover:text-destructive"
                          aria-label="Quitar material"
                          onClick={() => remove(index)}
                        >
                          <Trash2 className="size-4" aria-hidden />
                        </button>

                        <div className="grid grid-cols-2 gap-2 sm:grid-cols-3 lg:col-span-3">
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Cant. requerida
                            </span>
                            <TextInput
                              inputMode="decimal"
                              {...register(`rows.${index}.amountRequired`)}
                              placeholder="0"
                            />
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Unidad
                            </span>
                            <SelectInput
                              {...register(`rows.${index}.amountRequiredUnit`)}
                              disabled={rowDoseUnits.length === 0}
                              title={
                                rowMaterial?.unitOfMeasure
                                  ? `Unidad base del material: ${rowMaterial.unitOfMeasure}. Solo se permiten unidades compatibles.`
                                  : undefined
                              }
                            >
                              {rowDoseUnits.map((unit) => (
                                <option key={unit.value} value={unit.value}>
                                  {unit.label}
                                </option>
                              ))}
                            </SelectInput>
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Cant. estimada (automática)
                            </span>
                            <TextInput
                              value={
                                passes == null
                                  ? "—"
                                  : formatNumber(estimated.value, {
                                      maximumFractionDigits: 3,
                                    })
                              }
                              readOnly
                              disabled
                              title="Calculada como Maquinadas teóricas × Cant. requerida, en la unidad de la dosis."
                              className={hayStockInsuficiente ? "border-destructive bg-destructive/5" : ""}
                            />
                            {hayStockInsuficiente && materialStock && (
                              <p className="text-xs text-destructive flex items-center gap-1" role="alert">
                                <AlertCircle className="size-3" aria-hidden />
                                Stock insuficiente: disponible {formatNumber(stockDisponible, { maximumFractionDigits: 2 })} {baseUnit}, se requieren {formatNumber(estimatedInBaseUnit, { maximumFractionDigits: 2 })} {baseUnit}
                              </p>
                            )}
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Unidad
                            </span>
                            <TextInput
                              value={estimated.unit}
                              readOnly
                              disabled
                              title="Unidad de la dosis cargada."
                            />
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Marca
                            </span>
                            <TextInput
                              {...register(`rows.${index}.brand`)}
                            />
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Plaga / enfermedad
                            </span>
                            <TextInput
                              {...register(`rows.${index}.pestDisease`)}
                            />
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
              {errors.root?.message && errors.root.message.includes("receta") ? (
                <p className="text-xs text-destructive" role="alert">
                  {errors.root.message}
                </p>
              ) : null}
            </div>
          )}
        </section>

        <div className="flex justify-end">
          <Button variant="primary" type="submit" icon={Plus} loading={submitting || isFormSubmitting}>
            Crear orden de trabajo
          </Button>
        </div>
      </form>

      <Modal
        open={showRestoreModal}
        onClose={handleDiscardDraft}
        title="¿Restaurar borrador?"
        size="md"
      >
        <p className="text-sm text-muted-foreground">
          Se encontró un borrador guardado automáticamente. ¿Desea restaurarlo?
        </p>
        <div className="flex justify-end gap-2 mt-4">
          <Button variant="ghost" onClick={handleDiscardDraft}>
            Descartar
          </Button>
          <Button variant="primary" onClick={handleRestoreDraft}>
            Restaurar
          </Button>
        </div>
      </Modal>
    </div>
  );
}