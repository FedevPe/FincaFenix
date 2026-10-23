"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { MoreVertical, Pencil, Plus, RotateCcw, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { useAuth } from "@/app/providers/AuthProvider";
import { Button } from "@/components/common/Button";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { DataTable, type Column } from "@/components/common/DataTable";
import { Dropdown } from "@/components/common/Dropdown";
import {
  EmptyState,
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import {
  CheckboxRow,
  Field,
  SelectInput,
  TextInput,
} from "@/components/common/FormControls";
import { Modal } from "@/components/common/Modal";
import { Pagination } from "@/components/common/Pagination";
import { Badge } from "@/components/ui/badge";
import { getProblemDetails } from "@/lib/api.client";
import { formatNumber, hasPolicy } from "@/lib/utils";
import type {
  CurrencyDTO,
  MaterialCategoryDTO,
  MaterialDTO,
  UnitOfMeasureDTO,
} from "@/types/api";
import {
  deleteMaterial,
  getMaterialCategories,
  getMaterialsPaged,
  getUnitsOfMeasure,
  reactivateMaterial,
} from "../../materials/services/material.service";
import { getCurrencies } from "../../inventory/services/inventory.service";
import { MaterialForm } from "../components/MaterialForm";

const PAGE_SIZE = 10;

export function MaterialesTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "MATERIAL_CREATE");
  const canUpdate = hasPolicy(user?.policies, "MATERIAL_UPDATE");
  const canDelete = hasPolicy(user?.policies, "MATERIAL_DELETE");

  const [items, setItems] = useState<MaterialDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [categories, setCategories] = useState<MaterialCategoryDTO[]>([]);
  const [units, setUnits] = useState<UnitOfMeasureDTO[]>([]);
  const [currencies, setCurrencies] = useState<CurrencyDTO[]>([]);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<MaterialDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<MaterialDTO | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [pendingReactivate, setPendingReactivate] = useState<MaterialDTO | null>(
    null,
  );
  const [reactivating, setReactivating] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getMaterialsPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        categoryId: categoryFilter ? Number(categoryFilter) : undefined,
        includeDeleted,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar los materiales.");
    } finally {
      setLoading(false);
    }
  }, [page, search, categoryFilter, includeDeleted]);

  useEffect(() => {
    void load();
  }, [load]);

  useEffect(() => {
    void (async () => {
      const [categoryList, unitList, currencyList] = await Promise.all([
        getMaterialCategories().catch(() => [] as MaterialCategoryDTO[]),
        getUnitsOfMeasure().catch(() => [] as UnitOfMeasureDTO[]),
        getCurrencies().catch(() => [] as CurrencyDTO[]),
      ]);
      setCategories(categoryList);
      setUnits(unitList);
      setCurrencies(currencyList);
    })();
  }, []);

  useEffect(() => {
    const timer = setTimeout(() => {
      setSearch(searchInput.trim());
      setPage(1);
    }, 300);
    return () => clearTimeout(timer);
  }, [searchInput]);

  const handleSaved = () => {
    setFormOpen(false);
    setEditing(null);
    toast.success("El material se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteMaterial(pendingDelete.id);
      toast.success(`El material "${pendingDelete.articleName}" se eliminó.`);
      setPendingDelete(null);
      if (items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        void load();
      }
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar el material.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const confirmReactivate = async () => {
    if (!pendingReactivate) return;
    setReactivating(true);
    try {
      await reactivateMaterial(pendingReactivate.id);
      toast.success(`El material "${pendingReactivate.articleName}" se reactivó.`);
      setPendingReactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo reactivar el material.");
      setPendingReactivate(null);
    } finally {
      setReactivating(false);
    }
  };

  const sortedCategories = useMemo(
    () => [...categories].sort((a, b) => a.description.localeCompare(b.description)),
    [categories],
  );

  const columns: Column<MaterialDTO>[] = useMemo(
    () => [
      {
        key: "articleName",
        header: "Artículo",
        sortable: true,
        sortValue: (item) => item.articleName,
        render: (item) => (
          <span className="flex items-center gap-2">
            <span className="font-medium">{item.articleName}</span>
            {item.isDeleted ? <Badge variant="outline">Eliminado</Badge> : null}
          </span>
        ),
      },
      {
        key: "commercialName",
        header: "Nombre comercial",
        hidden: "md",
        render: (item) => item.commercialName,
      },
      {
        key: "category",
        header: "Categoría",
        hidden: "md",
        render: (item) => item.category?.description ?? "-",
      },
      {
        key: "unit",
        header: "Unidad",
        render: (item) => item.unitOfMeasure ?? "-",
      },
      {
        key: "cost",
        header: "Costo ref.",
        align: "right",
        hidden: "md",
        render: (item) =>
          item.referenceCost != null
            ? `${formatNumber(item.referenceCost, {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2,
              })} ${item.currencyCode ?? ""}`.trim()
            : "-",
      },
      {
        key: "actions",
        header: "",
        align: "right",
        hidden: "sm",
        render: (item) => {
          if (item.isDeleted) {
            if (!canUpdate) return null;
            return (
              <Dropdown
                trigger={<MoreVertical />}
                triggerLabel={`Acciones para ${item.articleName}`}
                items={[
                  {
                    label: "Reactivar",
                    icon: RotateCcw,
                    onClick: () => setPendingReactivate(item),
                  },
                ]}
              />
            );
          }
          return canUpdate || canDelete ? (
            <Dropdown
              trigger={<MoreVertical />}
              triggerLabel={`Acciones para ${item.articleName}`}
              items={[
                {
                  label: "Editar",
                  icon: Pencil,
                  disabled: !canUpdate,
                  onClick: () => {
                    setEditing(item);
                    setFormOpen(true);
                  },
                },
                {
                  label: "Eliminar",
                  icon: Trash2,
                  variant: "danger",
                  disabled: !canDelete,
                  onClick: () => setPendingDelete(item),
                },
              ]}
            />
          ) : null;
        },
      },
    ],
    [canUpdate, canDelete],
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div className="grid flex-1 gap-3 sm:grid-cols-2 lg:max-w-2xl">
          <Field label="Categoría" htmlFor="materialCategoryFilter">
            <SelectInput
              id="materialCategoryFilter"
              value={categoryFilter}
              onChange={(event) => {
                setCategoryFilter(event.target.value);
                setPage(1);
              }}
            >
              <option value="">Todas</option>
              {sortedCategories.map((category) => (
                <option key={category.id} value={category.id}>
                  {category.description}
                </option>
              ))}
            </SelectInput>
          </Field>
          <Field label="Buscar" htmlFor="materialSearch">
            <TextInput
              id="materialSearch"
              value={searchInput}
              placeholder="Artículo o nombre comercial…"
              onChange={(event) => setSearchInput(event.target.value)}
            />
          </Field>
        </div>
        <div className="flex items-center gap-3">
          <CheckboxRow
            label="Mostrar eliminados"
            checked={includeDeleted}
            onChange={(checked) => {
              setIncludeDeleted(checked);
              setPage(1);
            }}
          />
          <Button
            variant="primary"
            icon={Plus}
            disabled={!canCreate}
            onClick={() => {
              setEditing(null);
              setFormOpen(true);
            }}
          >
            Nuevo material
          </Button>
        </div>
      </div>

      {loading ? (
        <LoadingState label="Cargando materiales..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : items.length === 0 ? (
        <EmptyState
          title="No hay materiales"
          description={
            search || includeDeleted || categoryFilter
              ? "Ningún material coincide con los filtros aplicados."
              : "Todavía no se registraron materiales."
          }
        />
      ) : (
        <>
          <DataTable
            columns={columns}
            rows={items}
            rowKey={(item) => item.id}
          />
          <Pagination
            page={page}
            pageSize={PAGE_SIZE}
            totalCount={totalCount}
            onPageChange={setPage}
          />
        </>
      )}

      <Modal
        open={formOpen}
        title={editing ? "Editar material" : "Nuevo material"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
        size="lg"
      >
        <MaterialForm
          material={editing}
          categories={sortedCategories}
          units={units}
          currencies={currencies}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar material"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación del material "${pendingDelete.articleName}"?`
            : ""
        }
        confirmLabel="Eliminar"
        variant="danger"
        loading={deleting}
        onConfirm={confirmDelete}
        onCancel={() => setPendingDelete(null)}
      />

      <ConfirmDialog
        open={pendingReactivate !== null}
        title="Reactivar material"
        message={
          pendingReactivate
            ? `¿Confirmás la reactivación del material "${pendingReactivate.articleName}"?`
            : ""
        }
        confirmLabel="Reactivar"
        variant="primary"
        loading={reactivating}
        onConfirm={confirmReactivate}
        onCancel={() => setPendingReactivate(null)}
      />
    </div>
  );
}
