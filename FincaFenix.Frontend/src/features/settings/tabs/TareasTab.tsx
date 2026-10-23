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
import type { TaskDTO } from "@/types/api";
import { rendimientoModeLabel } from "../../tasks/constants";
import {
  deleteTask,
  getTasksPaged,
  reactivateTask,
} from "../../tasks/services/task.service";
import { TaskForm } from "../../tasks/components/TaskForm";

const PAGE_SIZE = 10;

export function TareasTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "TASK_CREATE");
  const canUpdate = hasPolicy(user?.policies, "TASK_UPDATE");
  const canDelete = hasPolicy(user?.policies, "TASK_DELETE");

  const [items, setItems] = useState<TaskDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editingTask, setEditingTask] = useState<TaskDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<TaskDTO | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [pendingReactivate, setPendingReactivate] = useState<TaskDTO | null>(null);
  const [reactivating, setReactivating] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getTasksPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        includeDeleted,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar las tareas.");
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
    setEditingTask(null);
    toast.success("La tarea se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteTask(pendingDelete.id);
      toast.success(`La tarea "${pendingDelete.description}" se eliminó.`);
      setPendingDelete(null);
      if (items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        void load();
      }
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar la tarea.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const confirmReactivate = async () => {
    if (!pendingReactivate) return;
    setReactivating(true);
    try {
      await reactivateTask(pendingReactivate.id);
      toast.success(`La tarea "${pendingReactivate.description}" se reactivó.`);
      setPendingReactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo reactivar la tarea.");
      setPendingReactivate(null);
    } finally {
      setReactivating(false);
    }
  };

  const columns: Column<TaskDTO>[] = useMemo(
    () => [
      {
        key: "description",
        header: "Descripción",
        sortable: true,
        sortValue: (task) => task.description,
        render: (task) => (
          <span className="flex items-center gap-2">
            <span className="font-medium">{task.description}</span>
            {task.isDeleted ? <Badge variant="outline">Eliminada</Badge> : null}
          </span>
        ),
      },
      {
        key: "mode",
        header: "Modo de rendimiento",
        hidden: "md",
        sortable: true,
        sortValue: (task) => rendimientoModeLabel(task.rendimientoMode),
        render: (task) => rendimientoModeLabel(task.rendimientoMode),
      },
      {
        key: "actions",
        header: "",
        align: "right",
        hidden: "sm",
        render: (task) => {
          if (task.isDeleted) {
            if (!canUpdate) return null;
            return (
              <Dropdown
                trigger={<MoreVertical />}
                triggerLabel={`Acciones para ${task.description}`}
                items={[
                  {
                    label: "Reactivar",
                    icon: RotateCcw,
                    onClick: () => setPendingReactivate(task),
                  },
                ]}
              />
            );
          }
          return canUpdate || canDelete ? (
            <Dropdown
              trigger={<MoreVertical />}
              triggerLabel={`Acciones para ${task.description}`}
              items={[
                {
                  label: "Editar",
                  icon: Pencil,
                  disabled: !canUpdate,
                  onClick: () => {
                    setEditingTask(task);
                    setFormOpen(true);
                  },
                },
                {
                  label: "Eliminar",
                  icon: Trash2,
                  variant: "danger",
                  disabled: !canDelete,
                  onClick: () => setPendingDelete(task),
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
            placeholder="Buscar por descripción…"
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
            setEditingTask(null);
            setFormOpen(true);
          }}
        >
          Nueva tarea
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando tareas..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : items.length === 0 ? (
        <EmptyState
          title="No hay tareas"
          description={
            search || includeDeleted
              ? "Ninguna tarea coincide con los filtros aplicados."
              : "Todavía no se registraron tareas."
          }
        />
      ) : (
        <>
          <DataTable columns={columns} rows={items} rowKey={(task) => task.id} />
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
        title={editingTask ? "Editar tarea" : "Nueva tarea"}
        onClose={() => {
          setFormOpen(false);
          setEditingTask(null);
        }}
      >
        <TaskForm
          task={editingTask}
          onClose={() => {
            setFormOpen(false);
            setEditingTask(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar tarea"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación de la tarea "${pendingDelete.description}"?`
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
        title="Reactivar tarea"
        message={
          pendingReactivate
            ? `¿Confirmás la reactivación de la tarea "${pendingReactivate.description}"?`
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
