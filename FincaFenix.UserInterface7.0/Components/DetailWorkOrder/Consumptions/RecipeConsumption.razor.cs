using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.ViewModels.ViewModels.WorkOrderDetails;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder.Consumptions
{
    public partial class RecipeConsumption
    {
        [Inject] private CalculateMaterialConsumedViewModel ViewModel { get; set; }

        [Parameter] public RecipeWorkOrderDTO Recipe { get; set; }
        [Parameter] public IEnumerable<DetailWorkOrderDTO> DetailWorkOrderList { get; set; }
        [Parameter] public decimal TotalAreaWorked { get; set; }

        private decimal _totalPerformance;
        private RecipeWorkOrderDTO _calculatedRecipe; // Almacenará la receta con los valores calculados

        protected override async Task OnParametersSetAsync()
        {
            // Llamamos al ViewModel para calcular los valores
            // y asignamos el resultado a la variable local.
            // Usamos el mismo ViewModel para todos los cálculos.
            _calculatedRecipe = await ViewModel.CalculateEstimatedAndConsumedAmounts(
                Recipe,
                DetailWorkOrderList,
                TotalAreaWorked
            );
            _totalPerformance = ViewModel.TotalPerfomance;
        }

    }
}