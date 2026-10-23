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
import { Pagination } from "@/components/common/Pagination";
import { Badge } from "@/components/ui/badge";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy } from "@/lib/utils";
import type { FarmDTO } from "@/types/api";
import {
  deleteFarm,
  getFarmsPaged,
  reactivateFarm,
} from "../services/farm.service";
import { FarmForm } from "../components/FarmForm";

const PAGE_SIZE = 10;

export function FincasTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "FARM_CREATE");
  const canUpdate = hasPolicy(user?.policies, "FARM_UPDATE");
  const canDelete = hasPolicy(user?.policies, "FARM_DELETE");

  const [items, setItems] = useState<FarmDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<FarmDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<FarmDTO | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [pendingReactivate, setPendingReactivate] = useState<FarmDTO | null>(null);
  const [reactivating, setReactivating] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getFarmsPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        includeDeleted,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las fincas.");
    } finally {
      setLoading(false);
    }
  }, [page, search, includeDeleted]);

  useEffect(() => {
    void load();
  }, [load]);

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
    toast.success("La finca se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteFarm(pendingDelete.id);
      toast.success(`La finca "${pendingDelete.name}" se eliminó.`);
      setPendingDelete(null);
      if (items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        void load();
      }
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar la finca.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const confirmReactivate = async () => {
    if (!pendingReactivate) return;
    setReactivating(true);
    try {
      await reactivateFarm(pendingReactivate.id);
      toast.success(`La finca "${pendingReactivate.name}" se reactivó.`);
      setPendingReactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo reactivar la finca.");
      setPendingReactivate(null);
    } finally {
      setReactivating(false);
    }
  };

  const columns: Column<FarmDTO>[] = useMemo(
    () => [
      {
        key: "name",
        header: "Nombre",
        sortable: true,
        sortValue: (farm) => farm.name,
        render: (farm) => (
          <span className="flex items-center gap-2">
            <span className="font-medium">{farm.name}</span>
            {farm.isDeleted ? <Badge variant="outline">Eliminada</Badge> : null}
          </span>
        ),
      },
      {
        key: "actions",
        header: "Acciones",
        align: "right",
        render: (farm) => {
          if (farm.isDeleted) {
            if (!canUpdate) return null;
            return (
              <Dropdown
                trigger={<MoreVertical />}
                triggerLabel={`Acciones para ${farm.name}`}
                items={[
                  {
                    label: "Reactivar",
                    icon: RotateCcw,
                    onClick: () => setPendingReactivate(farm),
                  },
                ]}
              />
            );
          }
          return canUpdate || canDelete ? (
            <Dropdown
              trigger={<MoreVertical />}
              triggerLabel={`Acciones para ${farm.name}`}
              items={[
                {
                  label: "Editar",
                  icon: Pencil,
                  disabled: !canUpdate,
                  onClick: () => {
                    setEditing(farm);
                    setFormOpen(true);
                  },
                },
                {
                  label: "Eliminar",
                  icon: Trash2,
                  variant: "danger",
                  disabled: !canDelete,
                  onClick: () => setPendingDelete(farm),
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
            value={searchInput}
            placeholder="Buscar por nombre…"
            className="sm:max-w-xs"
            onChange={(event) => setSearchInput(event.target.value)}
          />
          <CheckboxRow
            label="Mostrar eliminadas"
            checked={includeDeleted}
            onChange={(checked) => {
              setIncludeDeleted(checked);
              setPage(1);
            }}
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
          Nueva finca
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando fincas..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : items.length === 0 ? (
        <EmptyState
          title="No hay fincas"
          description={
            search || includeDeleted
              ? "Ninguna finca coincide con los filtros aplicados."
              : "Todavía no se registraron fincas."
          }
        />
      ) : (
        <>
          <DataTable columns={columns} rows={items} rowKey={(farm) => farm.id} />
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
        title={editing ? "Editar finca" : "Nueva finca"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <FarmForm
          farm={editing}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar finca"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación de la finca "${pendingDelete.name}"?`
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
        title="Reactivar finca"
        message={
          pendingReactivate
            ? `¿Confirmás la reactivación de la finca "${pendingReactivate.name}"?`
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
