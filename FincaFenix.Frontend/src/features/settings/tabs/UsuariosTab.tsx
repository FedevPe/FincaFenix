"use client";

import { useCallback, useEffect, useMemo, useState } from "react";
import { MoreVertical, Pencil, Plus, RotateCcw, UserX } from "lucide-react";
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
import type { UserDTO } from "@/types/api";
import {
  deactivateUser,
  getRoles,
  getUsersPaged,
  reactivateUser,
} from "../services/user.service";
import { UserForm } from "../components/UserForm";

const PAGE_SIZE = 10;

export function UsuariosTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "USER_CREATE");
  const canUpdate = hasPolicy(user?.policies, "USER_UPDATE");

  const [items, setItems] = useState<UserDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [includeInactive, setIncludeInactive] = useState(false);
  const [roles, setRoles] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<UserDTO | null>(null);
  const [pendingDeactivate, setPendingDeactivate] = useState<UserDTO | null>(null);
  const [deactivating, setDeactivating] = useState(false);
  const [pendingReactivate, setPendingReactivate] = useState<UserDTO | null>(null);
  const [reactivating, setReactivating] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getUsersPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        includeInactive,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar los usuarios.");
    } finally {
      setLoading(false);
    }
  }, [page, search, includeInactive]);

  useEffect(() => {
    void load();
  }, [load]);

  useEffect(() => {
    void (async () => {
      try {
        setRoles(await getRoles());
      } catch {
        setRoles([]);
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
    toast.success("El usuario se guardó correctamente.");
    void load();
  };

  const confirmDeactivate = async () => {
    if (!pendingDeactivate) return;
    setDeactivating(true);
    try {
      await deactivateUser(pendingDeactivate.id);
      toast.success(`El usuario "${pendingDeactivate.userName}" se desactivó.`);
      setPendingDeactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo desactivar el usuario.");
      setPendingDeactivate(null);
    } finally {
      setDeactivating(false);
    }
  };

  const confirmReactivate = async () => {
    if (!pendingReactivate) return;
    setReactivating(true);
    try {
      await reactivateUser(pendingReactivate.id);
      toast.success(`El usuario "${pendingReactivate.userName}" se reactivó.`);
      setPendingReactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo reactivar el usuario.");
      setPendingReactivate(null);
    } finally {
      setReactivating(false);
    }
  };

  const fullName = (item: UserDTO) =>
    [item.name, item.lastName].filter(Boolean).join(" ").trim();

  const columns: Column<UserDTO>[] = useMemo(
    () => [
      {
        key: "userName",
        header: "Usuario",
        sortable: true,
        sortValue: (item) => item.userName,
        render: (item) => (
          <span className="flex items-center gap-2">
            <span className="font-medium">{item.userName}</span>
            {!item.isActive ? <Badge variant="outline">Inactivo</Badge> : null}
          </span>
        ),
      },
      {
        key: "name",
        header: "Nombre",
        render: (item) => fullName(item) || "-",
      },
      {
        key: "email",
        header: "Email",
        hidden: "md",
        render: (item) => item.email ?? "-",
      },
      {
        key: "roles",
        header: "Rol",
        render: (item) => item.roles.join(", ") || "-",
      },
      {
        key: "actions",
        header: "",
        align: "right",
        hidden: "sm",
        render: (item) => {
          if (!canUpdate) return null;
          return (
            <Dropdown
              trigger={<MoreVertical />}
              triggerLabel={`Acciones para ${item.userName}`}
              items={[
                {
                  label: "Editar",
                  icon: Pencil,
                  onClick: () => {
                    setEditing(item);
                    setFormOpen(true);
                  },
                },
                item.isActive
                  ? {
                      label: "Desactivar",
                      icon: UserX,
                      variant: "danger" as const,
                      onClick: () => setPendingDeactivate(item),
                    }
                  : {
                      label: "Reactivar",
                      icon: RotateCcw,
                      onClick: () => setPendingReactivate(item),
                    },
              ]}
            />
          );
        },
      },
    ],
    [canUpdate],
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
        <div className="flex flex-1 flex-col gap-3 sm:flex-row sm:items-center">
          <TextInput
            value={searchInput}
            placeholder="Buscar por usuario, email o nombre…"
            className="sm:max-w-xs"
            onChange={(event) => setSearchInput(event.target.value)}
          />
          <CheckboxRow
            label="Mostrar inactivos"
            checked={includeInactive}
            onChange={(checked) => {
              setIncludeInactive(checked);
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
          Nuevo usuario
        </Button>
      </div>

      {loading ? (
        <LoadingState label="Cargando usuarios..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : items.length === 0 ? (
        <EmptyState
          title="No hay usuarios"
          description={
            search || includeInactive
              ? "Ningún usuario coincide con los filtros aplicados."
              : "Todavía no se registraron usuarios."
          }
        />
      ) : (
        <>
          <DataTable columns={columns} rows={items} rowKey={(item) => item.id} />
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
        title={editing ? "Editar usuario" : "Nuevo usuario"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <UserForm
          user={editing}
          roles={roles}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDeactivate !== null}
        title="Desactivar usuario"
        message={
          pendingDeactivate
            ? `¿Confirmás la desactivación del usuario "${pendingDeactivate.userName}"? No podrá iniciar sesión.`
            : ""
        }
        confirmLabel="Desactivar"
        variant="danger"
        loading={deactivating}
        onConfirm={confirmDeactivate}
        onCancel={() => setPendingDeactivate(null)}
      />

      <ConfirmDialog
        open={pendingReactivate !== null}
        title="Reactivar usuario"
        message={
          pendingReactivate
            ? `¿Confirmás la reactivación del usuario "${pendingReactivate.userName}"?`
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
