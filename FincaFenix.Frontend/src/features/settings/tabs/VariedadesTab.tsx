"use client";

import { useCallback, useEffect, useState } from "react";
import { MoreVertical, Pencil, Plus, Trash2 } from "lucide-react";
import { toast } from "sonner";
import { useAuth } from "@/app/providers/AuthProvider";
import { Button } from "@/components/common/Button";
import { ConfirmDialog } from "@/components/common/ConfirmDialog";
import { DataTable, type Column } from "@/components/common/DataTable";
import { Dropdown } from "@/components/common/Dropdown";
import {
  ErrorState,
  LoadingState,
} from "@/components/common/Feedback";
import { Field, SelectInput, TextInput } from "@/components/common/FormControls";
import { Modal } from "@/components/common/Modal";
import { Pagination } from "@/components/common/Pagination";
import { Badge } from "@/components/ui/badge";
import { getProblemDetails } from "@/lib/api.client";
import { hasPolicy } from "@/lib/utils";
import type { FruitDTO, FruitVarietyDTO } from "@/types/api";
import { VarietyForm } from "../components/VarietyForm";
import {
  deleteFruitVariety,
  getFruits,
  getFruitVarietiesPaged,
} from "../services/fruit.service";

const PAGE_SIZE = 10;

export function VariedadesTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "FRUIT_VARIETY_CREATE");
  const canUpdate = hasPolicy(user?.policies, "FRUIT_VARIETY_UPDATE");
  const canDelete = hasPolicy(user?.policies, "FRUIT_VARIETY_DELETE");

  const [items, setItems] = useState<FruitVarietyDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [fruits, setFruits] = useState<FruitDTO[]>([]);
  const [fruitFilter, setFruitFilter] = useState<string>("");
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<FruitVarietyDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<FruitVarietyDTO | null>(null);
  const [deleting, setDeleting] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getFruitVarietiesPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        fruitId: fruitFilter ? Number(fruitFilter) : undefined,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las variedades.");
    } finally {
      setLoading(false);
    }
  }, [page, search, fruitFilter]);

  useEffect(() => {
    void load();
  }, [load]);

  useEffect(() => {
    void (async () => {
      try {
        setFruits(await getFruits());
      } catch {
        setFruits([]);
      }
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
    toast.success("La variedad se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteFruitVariety(pendingDelete.id);
      toast.success(`La variedad "${pendingDelete.description}" se eliminó.`);
      setPendingDelete(null);
      if (items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        void load();
      }
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar la variedad.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const columns: Column<FruitVarietyDTO>[] = [
    {
      key: "description",
      header: "Variedad",
      sortable: true,
      sortValue: (item) => item.description,
      render: (item) => <span className="font-medium">{item.description}</span>,
    },
    {
      key: "fruit",
      header: "Fruta",
      hidden: "md",
      render: (item) =>
        item.fruitName ? (
          item.fruitName
        ) : (
          <span className="text-muted-foreground">Sin asignar</span>
        ),
    },
    {
      key: "organic",
      header: "Orgánica",
      hidden: "md",
      render: (item) =>
        item.isOrganic ? (
          <Badge variant="secondary">Sí</Badge>
        ) : (
          <span className="text-muted-foreground">No</span>
        ),
    },
    {
      key: "actions",
      header: "",
      align: "right",
      hidden: "sm",
      render: (item) =>
        canUpdate || canDelete ? (
          <Dropdown
            trigger={<MoreVertical />}
            triggerLabel={`Acciones para ${item.description}`}
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
      <div className="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div className="grid flex-1 gap-3 sm:grid-cols-2 lg:max-w-2xl">
          <Field label="Fruta" htmlFor="varietyFruitFilter">
            <SelectInput
              id="varietyFruitFilter"
              value={fruitFilter}
              onChange={(event) => {
                setFruitFilter(event.target.value);
                setPage(1);
              }}
            >
              <option value="">Todas</option>
              {fruits.map((fruit) => (
                <option key={fruit.id} value={fruit.id}>
                  {fruit.description}
                </option>
              ))}
            </SelectInput>
          </Field>
          <Field label="Buscar" htmlFor="varietySearch">
            <TextInput
              id="varietySearch"
              value={searchInput}
              placeholder="Buscar por descripción…"
              onChange={(event) => setSearchInput(event.target.value)}
            />
          </Field>
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
          Nueva variedad
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando variedades..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : (
        <>
          <DataTable
            columns={columns}
            rows={items}
            rowKey={(item) => item.id}
            emptyMessage="No hay variedades cargadas."
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
        title={editing ? "Editar variedad" : "Nueva variedad"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <VarietyForm
          variety={editing}
          onSaved={handleSaved}
          onCancel={() => {
            setFormOpen(false);
            setEditing(null);
          }}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar variedad"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación de la variedad "${pendingDelete.description}"? No se puede eliminar si tiene sectores asociados.`
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
