"use client";

import { useCallback, useEffect, useState } from "react";
import { MoreVertical, Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { useAuth } from "@/app/providers/AuthProvider";
import { Button } from "@/components/common/Button";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { DataTable, type Column } from "@/components/common/DataTable";
import { Dropdown } from "@/components/common/Dropdown";
import { ErrorState, LoadingState } from "@/components/common/Feedback";
import { TextInput } from "@/components/common/FormControls";
import { Modal } from "@/components/common/Modal";
import { Pagination } from "@/components/common/Pagination";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy } from "@/lib/utils";
import type { FruitDTO } from "@/types/api";
import { FruitForm } from "../components/FruitForm";
import { deleteFruit, getFruitsPaged } from "../services/fruit.service";

const PAGE_SIZE = 10;

export function FrutasTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "FRUIT_CREATE");
  const canUpdate = hasPolicy(user?.policies, "FRUIT_UPDATE");
  const canDelete = hasPolicy(user?.policies, "FRUIT_DELETE");

  const [items, setItems] = useState<FruitDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<FruitDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<FruitDTO | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getFruitsPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las frutas.");
    } finally {
      setLoading(false);
    }
  }, [page, search]);

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
    toast.success("La fruta se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteFruit(pendingDelete.id);
      toast.success(`La fruta "${pendingDelete.description}" se eliminó.`);
      setPendingDelete(null);
      if (items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        void load();
      }
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar la fruta.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const columns: Column<FruitDTO>[] = [
    {
      key: "description",
      header: "Fruta",
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
  ];

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <TextInput
          value={searchInput}
          placeholder="Buscar por descripción…"
          className="sm:max-w-xs"
          onChange={(event) => setSearchInput(event.target.value)}
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
          Nueva fruta
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando frutas..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : (
        <>
          <DataTable
            columns={columns}
            rows={items}
            rowKey={(item) => item.id}
            emptyMessage="No hay frutas cargadas."
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
        title={editing ? "Editar fruta" : "Nueva fruta"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <FruitForm
          fruit={editing}
          onSaved={handleSaved}
          onCancel={() => {
            setFormOpen(false);
            setEditing(null);
          }}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar fruta"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación de la fruta "${pendingDelete.description}"? No se puede eliminar si tiene variedades asociadas.`
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
