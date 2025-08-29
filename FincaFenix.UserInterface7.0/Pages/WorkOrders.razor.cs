using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.UserInterface7._0.Components.Shared;
using FincaFenix.UserInterface7._0.Services;
using FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Pages
{
    public partial class WorkOrders
    {
        [Inject] public LoadWorkOrdersViewModel ViewModel { get; set; }
        [Inject] public TextAppBarStateService TextApp { get; set; }

        private bool renderWorkOrderCard = false;
        private bool _isLoading = true;
        private List<ShowWorkOrderDTO> _filteredWorkOrderList;
        protected override async Task OnInitializedAsync()
        {
            TextApp.PageTitle = "Ordenes de Trabajo";
            await LoadDataAsync();
            _filteredWorkOrderList = ViewModel.AllWorkOrderList;            
        }

        private async Task HandleToggleChanged(bool value)
        {
            renderWorkOrderCard = value;
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            _isLoading = true;
            // Llama a StateHasChanged para mostrar el indicador de carga
            StateHasChanged();

            if (renderWorkOrderCard)
            {
                await ViewModel.LoadWorkOrderListPaginated();
            }
            else
            {
                await ViewModel.LoadWorkOrderList();
            }

            _isLoading = false;
            // Llama a StateHasChanged para renderizar los datos
            StateHasChanged();
        }
        private void HandleFilterSelected(StatsCard.FilterWorkOrder filter)
        {
            // Aplica la lógica de filtrado en el componente padre.
            switch (filter)
            {
                case StatsCard.FilterWorkOrder.Active:
                    _filteredWorkOrderList = ViewModel.AllWorkOrderList.Where(wo => wo.Status == "Activo").ToList();
                    break;
                case StatsCard.FilterWorkOrder.Pending:
                    _filteredWorkOrderList = ViewModel.AllWorkOrderList.Where(wo => wo.Status == "Pendiente").ToList();
                    break;
                case StatsCard.FilterWorkOrder.Closed:
                    _filteredWorkOrderList = ViewModel.AllWorkOrderList.Where(wo => wo.Status == "Cerrado").ToList();
                    break;
                case StatsCard.FilterWorkOrder.None:
                default:
                    _filteredWorkOrderList = ViewModel.AllWorkOrderList;
                    break;
            }
            StateHasChanged();
        }
    }
}