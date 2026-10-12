"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { MoreVertical, Pencil, Plus, Trash2 } from "lucide-react";
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
import { TextInput } from "@/components/common/FormControls";
import { Modal } from "@/components/common/Modal";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy } from "@/lib/utils";
import type { MaterialCategoryDTO } from "@/types/api";
import {
  deleteMaterialCategory,
  getMaterialCategories,
} from "../../materials/services/material.service";
import { CategoryForm } from "../components/CategoryForm";

export function CategoriasTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "MATERIAL_CATEGORY_CREATE");
  const canUpdate = hasPolicy(user?.policies, "MATERIAL_CATEGORY_UPDATE");
  const canDelete = hasPolicy(user?.policies, "MATERIAL_CATEGORY_DELETE");

  const [items, setItems] = useState<MaterialCategoryDTO[]>([]);
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<MaterialCategoryDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<MaterialCategoryDTO | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      setItems(await getMaterialCategories());
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las categorías.");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void load();
  }, [load]);

  const handleSaved = () => {
    setFormOpen(false);
    setEditing(null);
    toast.success("La categoría se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteMaterialCategory(pendingDelete.id);
      toast.success(`La categoría "${pendingDelete.description}" se eliminó.`);
      setPendingDelete(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar la categoría.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
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

  const columns: Column<MaterialCategoryDTO>[] = useMemo(
    () => [
      {
        key: "description",
        header: "Descripción",
        sortable: true,
        sortValue: (item) => item.description,
        render: (item) => <span className="font-medium">{item.description}</span>,
      },
      {
        key: "actions",
        header: "",
        align: "right",
        render: (item) =>
          canUpdate || canDelete ? (
            <Dropdown
              trigger={<MoreVertical />}
              triggerLabel="Acciones"
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
          ) : null,
      },
    ],
    [canUpdate, canDelete],
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <TextInput
          value={search}
          placeholder="Buscar por descripción…"
          className="sm:max-w-xs"
          onChange={(event) => setSearch(event.target.value)}
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
          Nueva categoría
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando categorías..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : visibleItems.length === 0 ? (
        <EmptyState
          title="No hay categorías"
          description={
            search
              ? "Ninguna categoría coincide con la búsqueda."
              : "Todavía no se registraron categorías."
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
        title={editing ? "Editar categoría" : "Nueva categoría"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <CategoryForm
          category={editing}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar categoría"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación de la categoría "${pendingDelete.description}"? No se puede eliminar si tiene materiales asociados.`
            : ""
        }
        confirmLabel="Eliminar"
        variant="danger"
        loading={deleting}
        onConfirm={confirmDelete}
        onCancel={() => setPendingDelete(null)}
      />
    </div>
  );
}
