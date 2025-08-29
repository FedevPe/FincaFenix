using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.RecipeDTO;

namespace FincaFenix.ViewModels.ViewModels.WorkOrderDetails
{
    public class CalculateMaterialConsumedViewModel
    {
        public decimal TotalPerfomance { get; private set; }

        public async Task CalculateEstimatedAndConsumedAmounts(RecipeWorkOrderDTO recipe, IEnumerable<DetailWorkOrderDTO> workOrderList, decimal sectorArea)
        {
            // Calcula el rendimiento total una sola vez.
            TotalPerfomance = workOrderList.Sum(d => d.Performance);

            foreach (var detail in recipe.Details)
            {
                // Calcula y asigna la cantidad estimada usando el parámetro sectorArea.
                detail.EstimatedAmount = CalculateAmountForSector(detail, recipe.TRV, sectorArea);

                // Calcula y asigna la cantidad consumida.
                detail.TotalAmountConsumed = CalculateAmount(detail, TotalPerfomance, recipe.TRV);
            }
            await Task.CompletedTask;
        }

        // Extrae la lógica de conversión de unidades para reutilizarla.
        private decimal GetAmountInLitersOrKilos(DetailRecipeDTO detail)
        {
            return (detail.AmountRequiredUnit == "cc" || detail.AmountRequiredUnit == "gr")
                ? detail.AmountRequired / 1000m
                : detail.AmountRequired;
        }

        private decimal CalculateAmount(DetailRecipeDTO detail, decimal totalPerformance, decimal trv)
        {
            var amountRequired = GetAmountInLitersOrKilos(detail);
            return (totalPerformance * amountRequired) / trv;
        }

        private decimal CalculateAmountForSector(DetailRecipeDTO detail, decimal trv, decimal areaSector)
        {
            var amountRequired = GetAmountInLitersOrKilos(detail);
            return ((trv * amountRequired) / 1000) * areaSector;
        }
    }
}