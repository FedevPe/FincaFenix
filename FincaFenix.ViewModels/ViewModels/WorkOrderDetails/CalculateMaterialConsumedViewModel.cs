using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.RecipeDTO;

namespace FincaFenix.ViewModels.ViewModels.WorkOrderDetails
{
    public class CalculateMaterialConsumedViewModel
    {
        public decimal TotalPerfomance { get; set; }
        public async Task CalculateTotalAmountConsume(RecipeWorkOrderDTO recipeWorkOrder, IEnumerable<DetailWorkOrderDTO> detailWorkOrderList)
        {
            TotalPerfomance = detailWorkOrderList.Sum(d => d.Performance);

            foreach (var detail in recipeWorkOrder.Details)
            {
                detail.TotalAmountConsumed = CalculateTotalAmount(detail, TotalPerfomance, recipeWorkOrder.TRV);
            }
            await Task.CompletedTask;
        }

        private decimal CalculateTotalAmount(DetailRecipeDTO detail, decimal totalPerformance, decimal trv)
        {
            var amountRequired = (detail.AmountRequiredUnit == "cc" || detail.AmountRequiredUnit == "gr")
                ? detail.AmountRequired / 1000m
                : detail.AmountRequired;

            return (totalPerformance * amountRequired) / trv;
        }
    }
}
