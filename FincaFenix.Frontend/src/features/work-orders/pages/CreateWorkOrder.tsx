"use client";

import { useEffect, useMemo, useState } from "react";
import { useRouter } from "next/navigation";
import { AlertCircle, ArrowLeft, FlaskConical, Plus, Trash2 } from "lucide-react";
import { PageHeader } from "@/components/common/PageHeader";
import { Button } from "@/components/common/Button";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { Field, TextInput, SelectInput } from "@/components/common/FormControls";
import { DataTable, type Column } from "@/components/common/DataTable";
import { useAuth } from "@/app/providers/AuthProvider";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy, formatNumber, parseDecimal } from "@/lib/utils";
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
import {
  materialLabel,
  unitCodeFor,
  unitFamilyOf,
} from "../../materials/material.utils";
import type { UnitFamily } from "../../materials/material.utils";

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
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [sectorsError, setSectorsError] = useState<string | null>(null);

  const [taskId, setTaskId] = useState("");
  const [farmId, setFarmId] = useState("");
  const [startDate, setStartDate] = useState(() => {
    const today = new Date();
    return `${today.getFullYear()}-${String(today.getMonth() + 1).padStart(2, "0")}-${String(
      today.getDate(),
    ).padStart(2, "0")}`;
  });
  const [description, setDescription] = useState("");

  const [selectedSectors, setSelectedSectors] = useState<number[]>([]);

  const [recipeEnabled, setRecipeEnabled] = useState(false);
  const [machineId, setMachineId] = useState("");
  const [volumeMachine, setVolumeMachine] = useState("");
  const [trv, setTrv] = useState("");
  const [rows, setRows] = useState<RecipeRow[]>([]);

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const selectedTask = tasks.find((task) => task.id === Number(taskId));
  const requiresRecipe = selectedTask?.rendimientoMode === "MaterialEfficiency";

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
    if (!farmId) {
      setSectors([]);
      setSelectedSectors([]);
      return;
    }
    setSectorsError(null);
    let active = true;
    void (async () => {
      try {
        const sectorList = await getSectorsByFarm(Number(farmId));
        if (!active) return;
        setSectors(sectorList);
        setSelectedSectors([]);
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
  }, [farmId]);

  useEffect(() => {
    if (requiresRecipe) setRecipeEnabled(true);
  }, [requiresRecipe]);

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
    setRows((prev) => [
      ...prev,
      {
        key: Date.now() + Math.random(),
        categoryId: "",
        materialId: "",
        amountRequired: "",
        amountRequiredUnit: "lts",
        brand: "",
        pestDisease: "",
      },
    ]);
  };

  const updateRow = (key: number, patch: Partial<RecipeRow>) => {
    setRows((prev) =>
      prev.map((row) => (row.key === key ? { ...row, ...patch } : row)),
    );
  };

  const removeRow = (key: number) => {
    setRows((prev) => prev.filter((row) => row.key !== key));
  };

  const onSelectMaterial = (rowKey: number, materialId: string) => {
    const material = materials.find((m) => m.id === Number(materialId));
    updateRow(rowKey, {
      materialId,
      amountRequiredUnit: materialId ? defaultDoseUnitFor(material) : "lts",
      brand: material?.brand ?? "",
    });
  };

  const addSector = (id: number) => {
    setSelectedSectors((prev) => (prev.includes(id) ? prev : [...prev, id]));
  };

  const removeSector = (id: number) => {
    setSelectedSectors((prev) => prev.filter((value) => value !== id));
  };

  const validate = (): boolean => {
    const next: Record<string, string> = {};

    if (!taskId) next.taskId = "La tarea es obligatoria.";
    if (!farmId) next.farmId = "La finca es obligatoria.";
    if (!startDate) next.startDate = "La fecha de inicio es obligatoria.";
    if (selectedSectors.length === 0)
      next.sectors = "Debe seleccionar al menos un sector.";

    if (recipeEnabled) {
      if (!machineId) next.machineId = "La máquina es obligatoria.";
      if (!volumeMachine || parseDecimal(volumeMachine) <= 0) {
        next.volumeMachine = "El volumen de la máquina debe ser mayor a cero.";
      }
      const validRows = rows.filter(
        (row) => row.materialId && parseDecimal(row.amountRequired) > 0,
      );
      if (validRows.length === 0) {
        next.recipe =
          "La receta debe tener al menos un material con cantidades completas.";
      }
    } else if (requiresRecipe) {
      next.recipe =
        "La tarea seleccionada requiere una receta con al menos un material.";
    }

    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!validate()) return;

    const now = new Date();
    const start = new Date(`${startDate}T00:00:00`);

    const areaTotal = sectors
      .filter((sector) => selectedSectors.includes(sector.id))
      .reduce((sum, sector) => sum + Number(sector.area || 0), 0);
    const passes = theoreticalPasses(
      areaTotal,
      parseDecimal(trv),
      parseDecimal(volumeMachine),
    );

    const recipeDetails =
      recipeEnabled && rows.length > 0
        ? rows
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
      taskId: Number(taskId),
      farmId: Number(farmId),
      description: description.trim() || undefined,
      sectorList: sectors
        .filter((sector) => selectedSectors.includes(sector.id))
        .map((sector) => ({ ...sector, selected: true })),
      createdDate: now.toISOString(),
      startDate: start.toISOString(),
      endDate: now.toISOString(),
      status: "Pendiente",
      totalArea: areaTotal,
      isDeleted: false,
      recipe:
        recipeEnabled && recipeDetails.length > 0
          ? {
              machineId: Number(machineId),
              volumeMachine: parseDecimal(volumeMachine),
              volumeMachineUnit: "lts",
              trv: parseDecimal(trv),
              status: "Activo",
              details: recipeDetails,
            }
          : null,
    };

    setSubmitting(true);
    try {
      const result = await createWorkOrder(payload);
      if (result.success) {
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
    .filter((sector) => selectedSectors.includes(sector.id))
    .reduce((sum, sector) => sum + Number(sector.area || 0), 0);

  const passes = theoreticalPasses(
    totalSelectedArea,
    parseDecimal(trv),
    parseDecimal(volumeMachine),
  );

  const selectedSectorRows = sectors.filter((sector) =>
    selectedSectors.includes(sector.id),
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
      <Button variant="ghost" icon={ArrowLeft} onClick={() => router.push("/work-orders")}>
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
        onSubmit={(event) => {
          event.preventDefault();
          void handleSubmit();
        }}
        noValidate
      >
        <section className="space-y-4 rounded-lg border bg-card p-4">
          <h2 className="text-base font-semibold">Datos generales</h2>

          <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
            <Field label="Tarea" htmlFor="taskId" required error={errors.taskId}>
              <SelectInput
                id="taskId"
                value={taskId}
                invalid={!!errors.taskId}
                onChange={(event) => {
                  setTaskId(event.target.value);
                  setErrors((prev) => ({ ...prev, taskId: "" }));
                }}
              >
                <option value="">Seleccione la tarea</option>
                {tasks.map((task) => (
                  <option key={task.id} value={task.id}>
                    {task.description}
                  </option>
                ))}
              </SelectInput>
            </Field>

            <Field label="Finca" htmlFor="farmId" required error={errors.farmId}>
              <SelectInput
                id="farmId"
                value={farmId}
                invalid={!!errors.farmId}
                onChange={(event) => {
                  setFarmId(event.target.value);
                  setErrors((prev) => ({ ...prev, farmId: "" }));
                }}
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
              error={errors.startDate}
              hint="Puede dejar la fecha actual o elegir una fecha futura."
            >
              <TextInput
                id="startDate"
                type="date"
                value={startDate}
                invalid={!!errors.startDate}
                onChange={(event) => {
                  setStartDate(event.target.value);
                  setErrors((prev) => ({ ...prev, startDate: "" }));
                }}
              />
            </Field>

            <Field label="Descripción" htmlFor="description">
              <TextInput
                id="description"
                value={description}
                placeholder="Detalle opcional de la orden"
                onChange={(event) => setDescription(event.target.value)}
              />
            </Field>
          </div>
        </section>

        <section className="space-y-4 rounded-lg border bg-card p-4">
          <h2 className="text-base font-semibold">Sectores de la finca</h2>

          {!farmId ? (
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
                <select
                  id="sector-add"
                  className="h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none"
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
                </select>
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
                Sectores seleccionados: {selectedSectors.length} · Área total:{" "}
                {formatNumber(totalSelectedArea, { maximumFractionDigits: 2 })} ha
              </p>
              {errors.sectors ? (
                <p className="text-xs text-destructive" role="alert">
                  {errors.sectors}
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
                checked={recipeEnabled}
                disabled={requiresRecipe}
                onChange={(event) => setRecipeEnabled(event.target.checked)}
                className="size-4"
              />
              <span>
                Incluir receta
                {requiresRecipe ? " (obligatoria para esta tarea)" : ""}
              </span>
            </label>
          </div>

          {!recipeEnabled ? (
            <p className="text-sm text-muted-foreground">
              Si no incluye receta, la orden no reservará materiales ni generará
              costos.
            </p>
          ) : (
            <div className="space-y-4">
              <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <Field
                  label="Máquina"
                  htmlFor="machineId"
                  required
                  error={errors.machineId}
                >
                  <SelectInput
                    id="machineId"
                    value={machineId}
                    invalid={!!errors.machineId}
                    onChange={(event) => {
                      setMachineId(event.target.value);
                      setErrors((prev) => ({ ...prev, machineId: "" }));
                    }}
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
                  error={errors.volumeMachine}
                  hint="La unidad de la máquina siempre es litros."
                >
                  <TextInput
                    id="volumeMachine"
                    inputMode="decimal"
                    value={volumeMachine}
                    invalid={!!errors.volumeMachine}
                    placeholder="Ej: 1000"
                    onChange={(event) => {
                      setVolumeMachine(event.target.value);
                      setErrors((prev) => ({ ...prev, volumeMachine: "" }));
                    }}
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
                  value={trv}
                  placeholder="Ej: 1"
                  onChange={(event) => setTrv(event.target.value)}
                />
              </Field>

              <div className="flex flex-wrap items-center justify-between gap-2">
                <h3 className="text-sm font-semibold">Materiales</h3>
                <Button variant="secondary" size="sm" icon={Plus} onClick={addRow}>
                  Agregar material
                </Button>
              </div>

              {rows.length === 0 ? (
                <p className="text-sm text-muted-foreground">
                  Agregue al menos un material con su cantidad requerida. La
                  cantidad estimada se calcula automáticamente.
                </p>
              ) : (
                <div className="space-y-3">
                  {rows.map((row) => {
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
                    const estimated = computeEstimatedAmount(row, passes);
                    return (
                      <div
                        key={row.key}
                        className="grid grid-cols-1 gap-2 rounded-md border p-3 lg:grid-cols-[1fr_1fr_auto]"
                      >
                        <select
                          aria-label="Categoría"
                          className="h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none"
                          value={row.categoryId}
                          onChange={(event) =>
                            updateRow(row.key, {
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
                        </select>

                        <select
                          aria-label="Material"
                          className="h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none disabled:opacity-50"
                          value={row.materialId}
                          disabled={!row.categoryId}
                          onChange={(event) =>
                            onSelectMaterial(row.key, event.target.value)
                          }
                        >
                          <option value="">Material</option>
                          {rowMaterials.map((material) => (
                            <option key={material.id} value={material.id}>
                              {materialLabel(material)}
                            </option>
                          ))}
                        </select>

                        <button
                          type="button"
                          className="inline-flex size-9 items-center justify-center justify-self-end rounded-md text-muted-foreground hover:bg-accent hover:text-destructive"
                          aria-label="Quitar material"
                          onClick={() => removeRow(row.key)}
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
                              value={row.amountRequired}
                              placeholder="0"
                              onChange={(event) =>
                                updateRow(row.key, {
                                  amountRequired: event.target.value,
                                })
                              }
                            />
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Unidad
                            </span>
                            <SelectInput
                              value={row.amountRequiredUnit}
                              disabled={rowDoseUnits.length === 0}
                              title={
                                rowMaterial?.unitOfMeasure
                                  ? `Unidad base del material: ${rowMaterial.unitOfMeasure}. Solo se permiten unidades compatibles.`
                                  : undefined
                              }
                              onChange={(event) =>
                                updateRow(row.key, {
                                  amountRequiredUnit: event.target.value,
                                })
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
                            />
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
                              value={row.brand}
                              onChange={(event) =>
                                updateRow(row.key, { brand: event.target.value })
                              }
                            />
                          </div>
                          <div className="space-y-1">
                            <span className="text-xs text-muted-foreground">
                              Plaga / enfermedad
                            </span>
                            <TextInput
                              value={row.pestDisease}
                              onChange={(event) =>
                                updateRow(row.key, {
                                  pestDisease: event.target.value,
                                })
                              }
                            />
                          </div>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}
            </div>
          )}
        </section>

        <div className="flex justify-end">
          <Button variant="primary" type="submit" icon={Plus} loading={submitting}>
            Crear orden de trabajo
          </Button>
        </div>
      </form>
    </div>
  );
}

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
