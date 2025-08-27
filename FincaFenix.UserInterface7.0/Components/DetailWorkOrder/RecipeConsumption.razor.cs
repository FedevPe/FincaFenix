using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.Entities.DTOs.RecipeDTO;
using FincaFenix.ViewModels.ViewModels.WorkOrderDetails;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder
{
    public partial class RecipeConsumption
    {
        [Inject] private CalculateMaterialConsumedViewModel ViewModel {  get; set; }

        [Parameter] public RecipeWorkOrderDTO Recipe {  get; set; }
        [Parameter] public IEnumerable<DetailWorkOrderDTO> DetailWorkOrderList { get; set; }

        private decimal _totalPerformance;

        protected override async Task OnInitializedAsync()
        {
            await ViewModel.CalculateTotalAmountConsume(Recipe, DetailWorkOrderList);
            _totalPerformance = ViewModel.TotalPerfomance;
        }
    }
}