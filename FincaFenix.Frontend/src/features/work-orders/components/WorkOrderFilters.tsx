"use client";

import * as React from "react";
import { Calendar, FilterX } from "lucide-react";
import { Button } from "@/components/common/Button";
import { cn } from "@/lib/utils";
import type { FarmDTO, TaskDTO } from "@/types/api";
import { WORK_ORDER_STATUSES } from "../constants";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import {
  Popover,
  PopoverTrigger,
  PopoverContent,
} from "@/components/ui/popover";
import { Calendar as CalendarComponent } from "@/components/ui/calendar";
import { format } from "date-fns";

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

  const formatDateValue = (dateStr: string) => {
    if (!dateStr) return "Seleccionar fecha";
    try {
      return format(new Date(dateStr), "yyyy-MM-dd");
    } catch {
      return dateStr;
    }
  };

  const disabledFrom = filters.dateTo
    ? (day: Date) => day > new Date(filters.dateTo)
    : undefined;
  const disabledTo = filters.dateFrom
    ? (day: Date) => day < new Date(filters.dateFrom)
    : undefined;

  return (
    <div className="grid grid-cols-1 gap-3 rounded-lg border bg-card p-3 sm:grid-cols-2 lg:grid-cols-6">
      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground">
          Estado
        </label>
        <Select value={filters.status} onValueChange={(value) => update({ status: value })}>
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Todos" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todos</SelectItem>
            {WORK_ORDER_STATUSES.map((status) => (
              <SelectItem key={status} value={status}>
                {status}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground">
          Finca
        </label>
        <Select value={filters.farmId} onValueChange={(value) => update({ farmId: value })}>
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Todas" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todas</SelectItem>
            {farms.map((farm) => (
              <SelectItem key={farm.id} value={String(farm.id)}>
                {farm.name}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-1.5 w-auto">
        <label className="text-xs font-medium text-muted-foreground">
          Tarea
        </label>
        <Select value={filters.taskId} onValueChange={(value) => update({ taskId: value })}>
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Todas" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="">Todas</SelectItem>
            {tasks.map((task) => (
              <SelectItem key={task.id} value={String(task.id)}>
                {task.description}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground">
          Desde
        </label>
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              className={cn("w-full justify-start text-left font-normal", !filters.dateFrom && "text-muted-foreground")}
            >
              <Calendar className="mr-2 h-4 w-4" aria-hidden="true" />
              {filters.dateFrom ? formatDateValue(filters.dateFrom) : "Seleccionar fecha"}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start">
            <CalendarComponent
              mode="single"
              selected={filters.dateFrom ? new Date(filters.dateFrom) : undefined}
              onSelect={(date) => update({ dateFrom: date ? format(date, "dd-MM-yyyy") : "" })}
              disabled={disabledFrom}
            />
          </PopoverContent>
        </Popover>
      </div>

      <div className="space-y-1.5">
        <label className="text-xs font-medium text-muted-foreground">
          Hasta
        </label>
        <Popover>
          <PopoverTrigger asChild>
            <Button
              variant="outline"
              className={cn("w-full justify-start text-left font-normal", !filters.dateTo && "text-muted-foreground")}
            >
              <Calendar className="mr-2 h-4 w-4" aria-hidden="true" />
              {filters.dateTo ? formatDateValue(filters.dateTo) : "Seleccionar fecha"}
            </Button>
          </PopoverTrigger>
          <PopoverContent className="w-auto p-0" align="start">
            <CalendarComponent
              mode="single"
              selected={filters.dateTo ? new Date(filters.dateTo) : undefined}
              onSelect={(date) => update({ dateTo: date ? format(date, "yyyy-MM-dd") : "" })}
              disabled={disabledTo}
            />
          </PopoverContent>
        </Popover>
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