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
import { parseDecimal } from "@/lib/utils";
import type {
  CurrencyDTO,
  MaterialCategoryDTO,
  MaterialDTO,
  UnitOfMeasureDTO,
} from "@/types/api";
import {
  createMaterial,
  updateMaterial,
} from "../../materials/services/material.service";

interface MaterialFormProps {
  material?: MaterialDTO | null;
  categories: MaterialCategoryDTO[];
  units: UnitOfMeasureDTO[];
  currencies: CurrencyDTO[];
  onClose: () => void;
  onSaved: () => void;
}

export function MaterialForm({
  material,
  categories,
  units,
  currencies,
  onClose,
  onSaved,
}: MaterialFormProps) {
  const [articleName, setArticleName] = useState("");
  const [commercialName, setCommercialName] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [unitOfMeasureId, setUnitOfMeasureId] = useState("");
  const [currencyId, setCurrencyId] = useState("1");
  const [referenceCost, setReferenceCost] = useState("");
  const [codeSap, setCodeSap] = useState("");
  const [descriptionSap, setDescriptionSap] = useState("");
  const [brand, setBrand] = useState("");
  const [description, setDescription] = useState("");

  const [errors, setErrors] = useState<Record<string, string>>({});
  const [submitError, setSubmitError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    setArticleName(material?.articleName ?? "");
    setCommercialName(material?.commercialName ?? "");
    setCategoryId(material?.categoryId ? String(material.categoryId) : "");
    setUnitOfMeasureId(
      material?.unitOfMeasureId ? String(material.unitOfMeasureId) : "",
    );
    setCurrencyId(material?.currencyId ? String(material.currencyId) : "1");
    setReferenceCost(
      material?.referenceCost != null ? String(material.referenceCost) : "",
    );
    setCodeSap(material?.codeSap ?? "");
    setDescriptionSap(material?.descriptionSap ?? "");
    setBrand(material?.brand ?? "");
    setDescription(material?.description ?? "");
    setErrors({});
    setSubmitError(null);
  }, [material]);

  const handleSubmit = async () => {
    setSubmitError(null);
    const next: Record<string, string> = {};
    if (!articleName.trim()) next.articleName = "El nombre del artículo es obligatorio.";
    if (!commercialName.trim())
      next.commercialName = "El nombre comercial es obligatorio.";
    if (!categoryId) next.categoryId = "Seleccioná una categoría.";
    if (!unitOfMeasureId) next.unitOfMeasureId = "Seleccioná una unidad de medida.";
    if (Object.keys(next).length > 0) {
      setErrors(next);
      return;
    }

    setSubmitting(true);
    try {
      const dto = {
        articleName: articleName.trim(),
        commercialName: commercialName.trim(),
        categoryId: Number(categoryId),
        unitOfMeasureId: Number(unitOfMeasureId),
        currencyId: Number(currencyId) || 1,
        referenceCost: referenceCost.trim() ? parseDecimal(referenceCost) : null,
        codeSap: codeSap.trim() || null,
        descriptionSap: descriptionSap.trim() || null,
        brand: brand.trim() || null,
        description: description.trim() || null,
      };
      if (material?.id) {
        await updateMaterial({ id: material.id, ...dto });
      } else {
        await createMaterial(dto);
      }
      onSaved();
    } catch (err) {
      const pd = getProblemDetails(err);
      setSubmitError(pd?.detail || "No se pudo guardar el material.");
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
        <Field
          label="Nombre del artículo"
          htmlFor="materialArticleName"
          required
          error={errors.articleName}
        >
          <TextInput
            id="materialArticleName"
            value={articleName}
            invalid={!!errors.articleName}
            onChange={(event) => {
              setArticleName(event.target.value);
              setErrors((prev) => ({ ...prev, articleName: "" }));
            }}
          />
        </Field>
        <Field
          label="Nombre comercial"
          htmlFor="materialCommercialName"
          required
          error={errors.commercialName}
        >
          <TextInput
            id="materialCommercialName"
            value={commercialName}
            invalid={!!errors.commercialName}
            onChange={(event) => {
              setCommercialName(event.target.value);
              setErrors((prev) => ({ ...prev, commercialName: "" }));
            }}
          />
        </Field>
        <Field
          label="Categoría"
          htmlFor="materialCategory"
          required
          error={errors.categoryId}
        >
          <SelectInput
            id="materialCategory"
            value={categoryId}
            invalid={!!errors.categoryId}
            onChange={(event) => {
              setCategoryId(event.target.value);
              setErrors((prev) => ({ ...prev, categoryId: "" }));
            }}
          >
            <option value="">Seleccionar…</option>
            {categories.map((category) => (
              <option key={category.id} value={category.id}>
                {category.description}
              </option>
            ))}
          </SelectInput>
        </Field>
        <Field
          label="Unidad de medida"
          htmlFor="materialUnit"
          required
          error={errors.unitOfMeasureId}
        >
          <SelectInput
            id="materialUnit"
            value={unitOfMeasureId}
            invalid={!!errors.unitOfMeasureId}
            onChange={(event) => {
              setUnitOfMeasureId(event.target.value);
              setErrors((prev) => ({ ...prev, unitOfMeasureId: "" }));
            }}
          >
            <option value="">Seleccionar…</option>
            {units.map((unit) => (
              <option key={unit.id} value={unit.id}>
                {unit.description}
              </option>
            ))}
          </SelectInput>
        </Field>
        <Field label="Divisa" htmlFor="materialCurrency">
          <SelectInput
            id="materialCurrency"
            value={currencyId}
            onChange={(event) => setCurrencyId(event.target.value)}
          >
            {currencies.map((currency) => (
              <option key={currency.id} value={currency.id}>
                {currency.code} · {currency.name}
              </option>
            ))}
          </SelectInput>
        </Field>
        <Field
          label="Costo de referencia"
          htmlFor="materialCost"
          hint="Opcional. En la divisa seleccionada."
        >
          <TextInput
            id="materialCost"
            value={referenceCost}
            inputMode="decimal"
            placeholder="0,00"
            onChange={(event) => setReferenceCost(event.target.value)}
          />
        </Field>
        <Field label="Código SAP" htmlFor="materialCodeSap">
          <TextInput
            id="materialCodeSap"
            value={codeSap}
            onChange={(event) => setCodeSap(event.target.value)}
          />
        </Field>
        <Field label="Descripción SAP" htmlFor="materialDescriptionSap">
          <TextInput
            id="materialDescriptionSap"
            value={descriptionSap}
            onChange={(event) => setDescriptionSap(event.target.value)}
          />
        </Field>
        <Field label="Marca" htmlFor="materialBrand">
          <TextInput
            id="materialBrand"
            value={brand}
            onChange={(event) => setBrand(event.target.value)}
          />
        </Field>
      </div>

      <Field label="Descripción" htmlFor="materialDescription">
        <TextInput
          id="materialDescription"
          value={description}
          onChange={(event) => setDescription(event.target.value)}
        />
      </Field>

      <div className="flex justify-end gap-2">
        <Button variant="ghost" onClick={onClose}>
          Cancelar
        </Button>
        <Button variant="primary" type="submit" loading={submitting}>
          {material?.id ? "Guardar cambios" : "Crear material"}
        </Button>
      </div>
    </form>
  );
}
