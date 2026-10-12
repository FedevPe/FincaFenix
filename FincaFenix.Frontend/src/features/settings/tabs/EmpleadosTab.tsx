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
import { hasPolicy } from "@/lib/utils";
import type { EmployeeDTO, FarmDTO } from "@/types/api";
import {
  deleteEmployee,
  getEmployeesPaged,
  reactivateEmployee,
} from "../services/employee.service";
import { EmployeeForm } from "../components/EmployeeForm";
import { getFarms } from "../../work-orders/services/workOrder.service";

const PAGE_SIZE = 10;

export function EmpleadosTab() {
  const { user } = useAuth();
  const canCreate = hasPolicy(user?.policies, "EMPLOYEE_CREATE");
  const canUpdate = hasPolicy(user?.policies, "EMPLOYEE_UPDATE");
  const canDelete = hasPolicy(user?.policies, "EMPLOYEE_DELETE");

  const [items, setItems] = useState<EmployeeDTO[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [page, setPage] = useState(1);
  const [searchInput, setSearchInput] = useState("");
  const [search, setSearch] = useState("");
  const [includeDeleted, setIncludeDeleted] = useState(false);
  const [farmFilter, setFarmFilter] = useState("");
  const [farms, setFarms] = useState<FarmDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [formOpen, setFormOpen] = useState(false);
  const [editing, setEditing] = useState<EmployeeDTO | null>(null);
  const [pendingDelete, setPendingDelete] = useState<EmployeeDTO | null>(null);
  const [deleting, setDeleting] = useState(false);
  const [pendingReactivate, setPendingReactivate] = useState<EmployeeDTO | null>(
    null,
  );
  const [reactivating, setReactivating] = useState(false);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const result = await getEmployeesPaged({
        pageNumber: page,
        pageSize: PAGE_SIZE,
        search: search || undefined,
        includeDeleted,
        farmId: farmFilter ? Number(farmFilter) : undefined,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
    } catch (err) {
      const pd = getProblemDetails(err);
      setError(pd?.detail || "No se pudieron cargar los empleados.");
    } finally {
      setLoading(false);
    }
  }, [page, search, includeDeleted, farmFilter]);

  useEffect(() => {
    void load();
  }, [load]);

  useEffect(() => {
    void (async () => {
      try {
        setFarms(await getFarms());
      } catch {
        setFarms([]);
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

  const farmName = useCallback(
    (farmId: number) => farms.find((farm) => farm.id === farmId)?.name ?? `#${farmId}`,
    [farms],
  );

  const handleSaved = () => {
    setFormOpen(false);
    setEditing(null);
    toast.success("El empleado se guardó correctamente.");
    void load();
  };

  const confirmDelete = async () => {
    if (!pendingDelete) return;
    setDeleting(true);
    try {
      await deleteEmployee(pendingDelete.id);
      toast.success("El empleado se eliminó.");
      setPendingDelete(null);
      if (items.length === 1 && page > 1) {
        setPage(page - 1);
      } else {
        void load();
      }
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo eliminar el empleado.");
      setPendingDelete(null);
    } finally {
      setDeleting(false);
    }
  };

  const confirmReactivate = async () => {
    if (!pendingReactivate) return;
    setReactivating(true);
    try {
      await reactivateEmployee(pendingReactivate.id);
      toast.success("El empleado se reactivó.");
      setPendingReactivate(null);
      void load();
    } catch (err) {
      const pd = getProblemDetails(err);
      toast.error(pd?.detail || "No se pudo reactivar el empleado.");
      setPendingReactivate(null);
    } finally {
      setReactivating(false);
    }
  };

  const fullName = (employee: EmployeeDTO) =>
    `${employee.name} ${employee.lastName ?? ""}`.trim();

  const columns: Column<EmployeeDTO>[] = useMemo(
    () => [
      {
        key: "name",
        header: "Nombre",
        sortable: true,
        sortValue: (employee) => fullName(employee),
        render: (employee) => (
          <span className="flex items-center gap-2">
            <span className="font-medium">{fullName(employee)}</span>
            {employee.isDeleted ? <Badge variant="outline">Eliminado</Badge> : null}
          </span>
        ),
      },
      {
        key: "farms",
        header: "Fincas",
        render: (employee) =>
          employee.farmIds && employee.farmIds.length > 0
            ? employee.farmIds.map(farmName).join(", ")
            : "-",
      },
      {
        key: "actions",
        header: "",
        align: "right",
        render: (employee) => {
          if (employee.isDeleted) {
            if (!canUpdate) return null;
            return (
              <Dropdown
                trigger={<MoreVertical />}
                triggerLabel="Acciones"
                items={[
                  {
                    label: "Reactivar",
                    icon: RotateCcw,
                    onClick: () => setPendingReactivate(employee),
                  },
                ]}
              />
            );
          }
          return canUpdate || canDelete ? (
            <Dropdown
              trigger={<MoreVertical />}
              triggerLabel="Acciones"
              items={[
                {
                  label: "Editar",
                  icon: Pencil,
                  disabled: !canUpdate,
                  onClick: () => {
                    setEditing(employee);
                    setFormOpen(true);
                  },
                },
                {
                  label: "Eliminar",
                  icon: Trash2,
                  variant: "danger",
                  disabled: !canDelete,
                  onClick: () => setPendingDelete(employee),
                },
              ]}
            />
          ) : null;
        },
      },
    ],
    [canUpdate, canDelete, farmName],
  );

  return (
    <div className="space-y-4">
      <div className="flex flex-col gap-3 lg:flex-row lg:items-end lg:justify-between">
        <div className="grid flex-1 gap-3 sm:grid-cols-2 lg:max-w-2xl">
          <Field label="Finca" htmlFor="employeeFarmFilter">
            <SelectInput
              id="employeeFarmFilter"
              value={farmFilter}
              onChange={(event) => {
                setFarmFilter(event.target.value);
                setPage(1);
              }}
            >
              <option value="">Todas</option>
              {farms.map((farm) => (
                <option key={farm.id} value={farm.id}>
                  {farm.name}
                </option>
              ))}
            </SelectInput>
          </Field>
          <Field label="Buscar" htmlFor="employeeSearch">
            <TextInput
              id="employeeSearch"
              value={searchInput}
              placeholder="Nombre o apellido…"
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
            Nuevo empleado
          </Button>
        </div>
      </div>

      {loading ? (
        <LoadingState label="Cargando empleados..." />
      ) : error ? (
        <ErrorState message={error} onRetry={load} />
      ) : items.length === 0 ? (
        <EmptyState
          title="No hay empleados"
          description={
            search || includeDeleted || farmFilter
              ? "Ningún empleado coincide con los filtros aplicados."
              : "Todavía no se registraron empleados."
          }
        />
      ) : (
        <>
          <DataTable
            columns={columns}
            rows={items}
            rowKey={(employee) => employee.id}
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
        title={editing ? "Editar empleado" : "Nuevo empleado"}
        onClose={() => {
          setFormOpen(false);
          setEditing(null);
        }}
      >
        <EmployeeForm
          employee={editing}
          farms={farms}
          onClose={() => {
            setFormOpen(false);
            setEditing(null);
          }}
          onSaved={handleSaved}
        />
      </Modal>

      <ConfirmDialog
        open={pendingDelete !== null}
        title="Eliminar empleado"
        message={
          pendingDelete
            ? `¿Confirmás la eliminación del empleado "${fullName(pendingDelete)}"?`
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
        title="Reactivar empleado"
        message={
          pendingReactivate
            ? `¿Confirmás la reactivación del empleado "${fullName(pendingReactivate)}"?`
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
