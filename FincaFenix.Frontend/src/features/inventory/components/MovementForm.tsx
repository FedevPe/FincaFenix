"use client";

import { useEffect, useState } from "react";
import { AlertCircle } from "lucide-react";
import { Button } from "@/components/common/Button";
import { Field, TextInput, SelectInput } from "@/components/common/FormControls";
import { getProblemDetails } from "@/lib/api.client";
import { parseDecimal } from "@/lib/utils";
import type {
  CurrencyDTO,
  MaterialCategoryDTO,
  MaterialRecipeDTO,
} from "@/types/api";
import {
  getMaterialCategories,
  getMaterials,
} from "../../materials/services/material.service";
import { materialLabel } from "../../materials/material.utils";
import { getFarms } from "../../work-orders/services/workOrder.service";
import {
  getCurrencies,
  registerMovement,
} from "../services/inventory.service";
import { INVENTORY_MOVEMENT_TYPES, movementTypeLabel } from "../constants";

interface MovementFormProps {
  onClose: () => void;
  onSaved: (resultingStock: number) => void;
}

export function MovementForm({ onClose, onSaved }: MovementFormProps) {
  const [materials, setMaterials] = useState<MaterialRecipeDTO[]>([]);
  const [categories, setCategories] = useState<MaterialCategoryDTO[]>([]);
  const [farms, setFarms] = useState<{ id: number; name: string }[]>([]);
  const [currencies, setCurrencies] = useState<CurrencyDTO[]>([]);

  const [categoryId, setCategoryId] = useState("");
  const [materialId, setMaterialId] = useState("");
  const [farmId, setFarmId] = useState("");
  const [movementType, setMovementType] = useState<string>("Ingreso");
  const [amount, setAmount] = useState("");
  const [unitCost, setUnitCost] = useState("");
  const [currencyId, setCurrencyId] = useState("");
  const [stockMinimum, setStockMinimum] = useState("");
  const [observations, setObservations] = useState("");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    let active = true;
    void (async () => {
      try {
        const [materialList, categoryList, farmList, currencyList] =
          await Promise.all([
            getMaterials(),
            getMaterialCategories(),
            getFarms(),
            getCurrencies(),
          ]);
        if (!active) return;
        setMaterials(materialList);
        setCategories(categoryList);
        setFarms(farmList);
        setCurrencies(currencyList);
        if (currencyList.length > 0) {
          const ars = currencyList.find((c) => c.code === "ARS");
          setCurrencyId(String((ars ?? currencyList[0]).id));
        }
      } catch {
        if (!active) return;
        setSubmitError("No se pudieron cargar los datos auxiliares del formulario.");
      }
    })();
    return () => {
      active = false;
    };
  }, []);

  const validate = (): boolean => {
    const next: Record<string, string> = {};

    if (!materialId) next.material = "El material es obligatorio.";
    if (!farmId) next.farm = "La finca es obligatoria.";
    if (!currencyId) next.currency = "La divisa es obligatoria.";
    if (parseDecimal(amount) <= 0) {
      next.amount = "La cantidad debe ser mayor a cero.";
    }
    if (unitCost.trim() && parseDecimal(unitCost) < 0) {
      next.unitCost = "El costo unitario no puede ser negativo.";
    }
    if (stockMinimum.trim() && parseDecimal(stockMinimum) < 0) {
      next.stockMinimum = "El stock mínimo no puede ser negativo.";
    }

    setErrors(next);
    return Object.keys(next).length === 0;
  };

  const handleSubmit = async () => {
    setSubmitError(null);
    if (!validate()) return;

    setSubmitting(true);
    try {
      const result = await registerMovement({
        materialId: Number(materialId),
        farmId: Number(farmId),
        movementType,
        amount: parseDecimal(amount),
        unitCost: unitCost.trim() ? parseDecimal(unitCost) : null,
        currencyId: Number(currencyId),
        stockMinimum: stockMinimum.trim() ? parseDecimal(stockMinimum) : null,
        observations: observations.trim() ? observations.trim() : null,
      });
      onSaved(result.resultingStock);
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo registrar el movimiento.");
    } finally {
      setSubmitting(false);
    }
  };

  const sortedCategories = [...categories].sort((a, b) =>
    a.description.localeCompare(b.description),
  );
  const filteredMaterials = categoryId
    ? materials.filter(
        (m) => (m.category?.id ?? m.categoryId) === Number(categoryId),
      )
    : [];

  const selectedMaterial = materials.find((m) => m.id === Number(materialId));
  const unit = selectedMaterial?.unitOfMeasure?.trim() || "";
  const unitSuffix = unit ? ` (${unit})` : "";
  const unitLabel = unit ? `por ${unit}` : "por unidad base";

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

      <Field label="Categoría" htmlFor="categoryId" required>
        <SelectInput
          id="categoryId"
          value={categoryId}
          onChange={(event) => {
            setCategoryId(event.target.value);
            setMaterialId("");
          }}
        >
          <option value="">Seleccionar categoría…</option>
          {sortedCategories.map((category) => (
            <option key={category.id} value={category.id}>
              {category.description}
            </option>
          ))}
        </SelectInput>
      </Field>

      <Field label="Material" htmlFor="materialId" required error={errors.material}>
        <SelectInput
          id="materialId"
          value={materialId}
          disabled={!categoryId}
          invalid={!!errors.material}
          onChange={(event) => {
            setMaterialId(event.target.value);
            setErrors((prev) => ({ ...prev, material: "" }));
          }}
        >
          <option value="">
            {categoryId ? "Seleccionar material…" : "Seleccione una categoría…"}
          </option>
          {filteredMaterials.map((material) => (
            <option key={material.id} value={material.id}>
              {materialLabel(material)}
            </option>
          ))}
        </SelectInput>
      </Field>

      <Field label="Finca" htmlFor="farmId" required error={errors.farm}>
        <SelectInput
          id="farmId"
          value={farmId}
          invalid={!!errors.farm}
          onChange={(event) => {
            setFarmId(event.target.value);
            setErrors((prev) => ({ ...prev, farm: "" }));
          }}
        >
          <option value="">Seleccionar finca…</option>
          {farms.map((farm) => (
            <option key={farm.id} value={farm.id}>
              {farm.name}
            </option>
          ))}
        </SelectInput>
      </Field>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field label="Tipo de movimiento" htmlFor="movementType" required>
          <SelectInput
            id="movementType"
            value={movementType}
            onChange={(event) => setMovementType(event.target.value)}
          >
            {INVENTORY_MOVEMENT_TYPES.map((type) => (
              <option key={type} value={type}>
                {movementTypeLabel(type)}
              </option>
            ))}
          </SelectInput>
        </Field>

        <Field
          label={`Cantidad${unitSuffix}`}
          htmlFor="amount"
          required
          error={errors.amount}
          hint="En la unidad base del material."
        >
          <TextInput
            id="amount"
            inputMode="decimal"
            value={amount}
            invalid={!!errors.amount}
            placeholder="Ej: 100"
            onChange={(event) => {
              setAmount(event.target.value);
              setErrors((prev) => ({ ...prev, amount: "" }));
            }}
          />
        </Field>
      </div>

      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <Field
          label={`Costo unitario${unitSuffix} (opcional)`}
          htmlFor="unitCost"
          error={errors.unitCost}
          hint={`Costo por ${unit || "unidad base"}. Actualiza el costo de referencia del material.`}
        >
          <TextInput
            id="unitCost"
            inputMode="decimal"
            value={unitCost}
            invalid={!!errors.unitCost}
            placeholder="Ej: 1500,50"
            onChange={(event) => {
              setUnitCost(event.target.value);
              setErrors((prev) => ({ ...prev, unitCost: "" }));
            }}
          />
        </Field>

        <Field label="Divisa" htmlFor="currencyId" required error={errors.currency}>
          <SelectInput
            id="currencyId"
            value={currencyId}
            invalid={!!errors.currency}
            onChange={(event) => {
              setCurrencyId(event.target.value);
              setErrors((prev) => ({ ...prev, currency: "" }));
            }}
          >
            <option value="">Seleccionar divisa…</option>
            {currencies.map((currency) => (
              <option key={currency.id} value={currency.id}>
                {currency.code} · {currency.name}
              </option>
            ))}
          </SelectInput>
        </Field>
      </div>

      <Field
        label={`Stock mínimo${unitSuffix} (opcional)`}
        htmlFor="stockMinimum"
        error={errors.stockMinimum}
        hint="Umbral para alertas de stock bajo."
      >
        <TextInput
          id="stockMinimum"
          inputMode="decimal"
          value={stockMinimum}
          invalid={!!errors.stockMinimum}
          placeholder="Ej: 10"
          onChange={(event) => {
            setStockMinimum(event.target.value);
            setErrors((prev) => ({ ...prev, stockMinimum: "" }));
          }}
        />
      </Field>

      <Field label="Observaciones (opcional)" htmlFor="observations">
        <TextInput
          id="observations"
          value={observations}
          placeholder="Detalle del movimiento"
          onChange={(event) => setObservations(event.target.value)}
        />
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          Registrar movimiento
        </Button>
      </div>
    </form>
  );
}
