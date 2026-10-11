import { FilterX } from "lucide-react";
import { Button } from "@/components/common/Button";
import type { FarmDTO, TaskDTO } from "@/types/api";
import { WORK_ORDER_STATUSES } from "../constants";

export interface WorkOrderFilterValues {
  status: string;
  farmId: string;
  taskId: string;
  dateFrom: string;
  dateTo: string;
}

export const EMPTY_FILTERS: WorkOrderFilterValues = {
  status: "",
  farmId: "",
  taskId: "",
  dateFrom: "",
  dateTo: "",
};

interface WorkOrderFiltersProps {
  filters: WorkOrderFilterValues;
  farms: FarmDTO[];
  tasks: TaskDTO[];
  onChange: (filters: WorkOrderFilterValues) => void;
  onReset: () => void;
}

const CONTROL_CLASSES =
  "h-9 w-full rounded-md border bg-background px-3 text-sm focus-visible:ring-2 focus-visible:ring-ring focus-visible:outline-none";

export function WorkOrderFilters({
  filters,
  farms,
  tasks,
  onChange,
  onReset,
}: WorkOrderFiltersProps) {
  const update = (patch: Partial<WorkOrderFilterValues>) =>
    onChange({ ...filters, ...patch });

  const hasFilters = Object.values(filters).some((value) => value !== "");

  return (
    <div className="grid grid-cols-1 gap-3 rounded-lg border bg-card p-3 sm:grid-cols-2 lg:grid-cols-6">
      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground" htmlFor="filter-status">
          Estado
        </label>
        <select
          id="filter-status"
          className={CONTROL_CLASSES}
          value={filters.status}
          onChange={(event) => update({ status: event.target.value })}
        >
          <option value="">Todos</option>
          {WORK_ORDER_STATUSES.map((status) => (
            <option key={status} value={status}>
              {status}
            </option>
          ))}
        </select>
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground" htmlFor="filter-farm">
          Finca
        </label>
        <select
          id="filter-farm"
          className={CONTROL_CLASSES}
          value={filters.farmId}
          onChange={(event) => update({ farmId: event.target.value })}
        >
          <option value="">Todas</option>
          {farms.map((farm) => (
            <option key={farm.id} value={farm.id}>
              {farm.name}
            </option>
          ))}
        </select>
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground" htmlFor="filter-task">
          Tarea
        </label>
        <select
          id="filter-task"
          className={CONTROL_CLASSES}
          value={filters.taskId}
          onChange={(event) => update({ taskId: event.target.value })}
        >
          <option value="">Todas</option>
          {tasks.map((task) => (
            <option key={task.id} value={task.id}>
              {task.description}
            </option>
          ))}
        </select>
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground" htmlFor="filter-from">
          Desde
        </label>
        <input
          id="filter-from"
          type="date"
          className={CONTROL_CLASSES}
          value={filters.dateFrom}
          max={filters.dateTo || undefined}
          onChange={(event) => update({ dateFrom: event.target.value })}
        />
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground" htmlFor="filter-to">
          Hasta
        </label>
        <input
          id="filter-to"
          type="date"
          className={CONTROL_CLASSES}
          value={filters.dateTo}
          min={filters.dateFrom || undefined}
          onChange={(event) => update({ dateTo: event.target.value })}
        />
      </div>

      <div className="flex items-end">
        <Button
          variant="secondary"
          icon={FilterX}
          onClick={onReset}
          disabled={!hasFilters}
          className="w-full"
        >
          Limpiar
        </Button>
      </div>
    </div>
  );
}
