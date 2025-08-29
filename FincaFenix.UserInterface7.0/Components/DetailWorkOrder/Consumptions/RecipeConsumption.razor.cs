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
        private decimal _areaSector;
        private RecipeWorkOrderDTO _recipe { get; set; }
        private IEnumerable<DetailWorkOrderDTO> _detailWorkOrderList { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _recipe = Recipe;
            _areaSector = TotalAreaWorked;
            _detailWorkOrderList = DetailWorkOrderList;
            await ViewModel.CalculateEstimatedAndConsumedAmounts(_recipe, _detailWorkOrderList, _areaSector);
            _totalPerformance = ViewModel.TotalPerfomance;
        }

    }
}