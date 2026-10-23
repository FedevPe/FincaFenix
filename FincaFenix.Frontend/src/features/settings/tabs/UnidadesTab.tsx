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
import { CheckboxRow, TextInput } from "@/components/common/FormControls";
import { Modal } from "@/components/common/Modal";
import { Badge } from "@/components/ui/badge";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy } from "@/lib/utils";
import type { UnitOfMeasureDTO } from "@/types/api";
import {
  deleteUnitOfMeasure,
  getUnitsOfMeasure,
  reactivateUnitOfMeasure,
} from "../../materials/services/material.service";
import { UnitForm } from "../components/UnitForm";

export function UnidadesTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "UNIT_OF_MEASURE_CREATE");
  const canUpdate = hasPolicy(user?.policies, "UNIT_OF_MEASURE_UPDATE");
  const canDelete = hasPolicy(user?.policies, "UNIT_OF_MEASURE_DELETE");

  const [items, setItems] = useState<UnitOfMeasureDTO[]>([]);
  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<UnitOfMeasureDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<UnitOfMeasureDTO | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [pendingReactivate, setPendingReactivate] = useState<UnitOfMeasureDTO | null>(
    null,
  );
  const [reactivating, setReactivating] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await getUnitsOfMeasure(includeDeleted));
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las unidades de medida.");
    } finally {
      setLoading(false);
    }
  }, [includeDeleted]);

  useEffect(() => {
    void load();
  }, [load]);

  const handleSaved = () => {
    setFormOpen(false);
    setEditing(null);
    toast.success("La unidad de medida se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteUnitOfMeasure(pendingDelete.id);
      toast.success(`La unidad "${pendingDelete.description}" se eliminó.`);
      setPendingDelete(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar la unidad de medida.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const confirmReactivate = async () => {
    if (!pendingReactivate) return;
    setReactivating(true);
    try {
      await reactivateUnitOfMeasure(pendingReactivate.id);
      toast.success(`La unidad "${pendingReactivate.description}" se reactivó.`);
      setPendingReactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo reactivar la unidad de medida.");
      setPendingReactivate(null);
    } finally {
      setReactivating(false);
    }
  };

  const visibleItems = useMemo(() => {
    const term = search.trim().toLowerCase();
    const sorted = [...items].sort((a, b) =>
      a.description.localeCompare(b.description),
    );
    if (!term) return sorted;
    return sorted.filter((item) => item.description.toLowerCase().includes(term));
  }, [items, search]);

  const columns: Column<UnitOfMeasureDTO>[] = useMemo(
    () => [
      {
        key: "description",
        header: "Descripción",
        sortable: true,
        sortValue: (item) => item.description,
        render: (item) => (
          <span className="flex items-center gap-2">
            <span className="font-medium">{item.description}</span>
            {item.isDeleted ? <Badge variant="outline">Eliminada</Badge> : null}
          </span>
        ),
      },
      {
        key: "code",
        header: "Unidad",
        sortable: true,
        sortValue: (item) => item.code ?? "",
        render: (item) =>
          item.code ? (
            <span className="font-mono">{item.code}</span>
          ) : (
            <span className="text-muted-foreground">—</span>
          ),
      },
      {
        key: "actions",
        header: "Acciones",
        align: "right",
        render: (item) => {
          if (item.isDeleted) {
            if (!canUpdate) return null;
            return (
              <Dropdown
                trigger={<MoreVertical />}
                triggerLabel={`Acciones para ${item.code}`}
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
              triggerLabel={`Acciones para ${item.code}`}
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
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex flex-1 flex-col gap-3 sm:flex-row sm:items-center">
          <TextInput
            value={search}
            placeholder="Buscar por descripción…"
            className="sm:max-w-xs"
            onChange={(event) => setSearch(event.target.value)}
          />
          <CheckboxRow
            label="Mostrar eliminadas"
            checked={includeDeleted}
            onChange={setIncludeDeleted}
          />
        </div>
        <Button
          variant="primary"
          icon={Plus}
          disabled={!canCreate}
          onClick={() => {
            setEditing(null);
            setFormOpen(true);
          }}
        >
          Nueva unidad
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando unidades..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : visibleItems.length === 0 ? (
        <EmptyState
          title="No hay unidades de medida"
          description={
            search || includeDeleted
              ? "Ninguna unidad coincide con los filtros aplicados."
              : "Todavía no se registraron unidades de medida."
          }
        />
      ) : (
        <DataTable
          columns={columns}
          rows={visibleItems}
          rowKey={(item) => item.id}
        />
      )}

      <Modal
        open={formOpen}
        title={editing ? "Editar unidad de medida" : "Nueva unidad de medida"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <UnitForm
          unit={editing}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar unidad de medida"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación de la unidad "${pendingDelete.description}"?`
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
        title="Reactivar unidad de medida"
        message={
          pendingReactivate
            ? `¿Confirmás la reactivación de la unidad "${pendingReactivate.description}"?`
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
