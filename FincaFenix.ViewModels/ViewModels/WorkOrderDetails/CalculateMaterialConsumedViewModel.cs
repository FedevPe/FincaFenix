using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.RecipeDTO;

namespace FincaFenix.ViewModels.ViewModels.WorkOrderDetails
{
    public class CalculateMaterialConsumedViewModel
    {
        public decimal TotalPerfomance { get; private set; }

        public async Task<RecipeWorkOrderDTO> CalculateEstimatedAndConsumedAmounts(RecipeWorkOrderDTO recipe, IEnumerable<DetailWorkOrderDTO> workOrderList, decimal sectorArea)
        {
            TotalPerfomance = workOrderList.Sum(d => d.Performance);

            // Creamos una nueva lista de detalles para no modificar la original
            var newDetails = new List<DetailRecipeDTO>();

            foreach (var detail in recipe.Details)
            {
                // Creamos una copia del objeto DetailRecipeDTO
                var newDetail = new DetailRecipeDTO
                {
                    Material = detail.Material,
                    Brand = detail.Brand,
                    AmountRequired = detail.AmountRequired,
                    AmountRequiredUnit = detail.AmountRequiredUnit
                };

                // Realizamos los cálculos y los asignamos a la nueva copia
                newDetail.EstimatedAmount = CalculateAmountEstimatedForSector(detail, recipe.VolumeMachine ,recipe.TRV, sectorArea);
                newDetail.EstimatedAmountUnit = GetAmountUnit(detail.AmountRequiredUnit);
                newDetail.TotalAmountConsumed = CalculateAmountConsumption(detail, recipe.VolumeMachine, TotalPerfomance);

                newDetails.Add(newDetail);
            }

            // Creamos una copia de la receta con los nuevos detalles
            var newRecipe = new RecipeWorkOrderDTO
            {
                Id = recipe.Id,
                NumRecipe = recipe.NumRecipe,
                Machine = recipe.Machine,
                VolumeMachine = recipe.VolumeMachine,
                VolumeMachineUnit = recipe.VolumeMachineUnit,
                TRV = recipe.TRV,
                Details = newDetails
            };

            await Task.CompletedTask;
            return newRecipe; // Devolvemos el nuevo objeto
        }

        // Extrae la lógica de conversión de unidades para reutilizarla.
        private decimal GetAmountInLitersOrKilos(DetailRecipeDTO detail)
        {
            return (detail.AmountRequiredUnit == "cc" || detail.AmountRequiredUnit == "gr")
                ? detail.AmountRequired / 1000m
                : detail.AmountRequired;
        }

        private decimal CalculateAmountConsumption(DetailRecipeDTO detail, decimal machineCapacity, decimal totalPerformance)
        {
            var amountRequired = GetAmountInLitersOrKilos(detail);
            return (totalPerformance * amountRequired) / machineCapacity;
        }

        private decimal CalculateAmountEstimatedForSector(DetailRecipeDTO detail, decimal machineCapacity, decimal trv, decimal areaSector)
        {
            var amountRequired = GetAmountInLitersOrKilos(detail);
            decimal totalAplicationVolume = trv * areaSector;
            decimal estimatedAmount = (totalAplicationVolume * amountRequired) / machineCapacity;
            return estimatedAmount;
        }
        private string GetAmountUnit(string unit)
        {
            if (unit == "cc" || unit == "lts")
            {
                return "lts"; // Convertir a litros o kilos
            }
            else
            {
                return "kg";
            }
        }
    }
}