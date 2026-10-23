import { DataTable } from "@/components/common/DataTable";
import type { Column } from "@/components/common/DataTable";
import { EmptyState } from "@/components/common/Feedback";
import type { DetailRecipeDTO, ShowWorkOrderDTO } from "@/types/api";
import { formatNumber } from "@/lib/utils";

interface RecipeTabProps {
  workOrder: ShowWorkOrderDTO;
}

function materialName(detail: DetailRecipeDTO): string {
  const material = detail.material;
  if (!material) return `Material #${detail.materialId}`;
  if (material.commercialName) {
    return `${material.articleName ?? ""} (${material.commercialName})`;
  }
  return material.articleName ?? `Material #${detail.materialId}`;
}

function Field({ label, value }: { label: string; value: React.ReactNode }) {
  return (
    <div className="space-y-0.5">
      <span className="text-xs font-medium text-muted-foreground">{label}</span>
      <span className="block text-sm">{value}</span>
    </div>
  );
}

export function RecipeTab({ workOrder }: RecipeTabProps) {
  const recipe = workOrder.recipe;

  if (!recipe) {
    return (
      <EmptyState
        title="La orden no tiene receta"
        description="Esta orden de trabajo se creó sin una receta de materiales asociada."
      />
    );
  }

  const columns: Column<DetailRecipeDTO>[] = [
    {
      key: "material",
      header: "Material",
      render: (detail) => materialName(detail),
    },
    {
      key: "category",
      header: "Categoría",
      render: (detail) =>
        detail.material?.category?.description ??
        (detail.categoryId ? `Categoría ${detail.categoryId}` : "-"),
    },
    {
      key: "brand",
      header: "Marca",
      render: (detail) => detail.brand ?? "-",
    },
    {
      key: "amountRequired",
      header: "Dosis por aplicación",
      align: "right",
      render: (detail) =>
        `${formatNumber(detail.amountRequired, { maximumFractionDigits: 4 })} ${detail.amountRequiredUnit ?? ""}`,
    },
    {
      key: "estimatedAmount",
      header: "Plan total",
      align: "right",
      render: (detail) =>
        `${formatNumber(detail.estimatedAmount, { maximumFractionDigits: 4 })} ${detail.estimatedAmountUnit ?? ""}`,
    },
    {
      key: "consumed",
      header: "Consumido",
      align: "right",
      render: (detail) =>
        detail.totalAmountConsumed > 0
          ? `${formatNumber(detail.totalAmountConsumed, { maximumFractionDigits: 4 })} ${detail.material?.unitOfMeasure ?? ""}`
          : "-",
    },
  ];

  return (
    <div className="space-y-4 rounded-lg border bg-card p-4">
      <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-5">
        <Field label="Nº de receta" value={recipe.numRecipe ?? "-"} />
        <Field label="Máquina" value={recipe.machine?.name ?? "-"} />
        <Field
          label="Volumen de máquina"
          value={`${formatNumber(recipe.volumeMachine)} ${recipe.volumeMachineUnit ?? ""}`}
        />
        <Field label="TRV (lts/ha)" value={formatNumber(recipe.trv)} />
        <Field label="Estado" value={recipe.status ?? "-"} />
      </div>

      <DataTable
        columns={columns}
        rows={recipe.details}
        rowKey={(d) => d.materialId}
      />
    </div>
  );
}
