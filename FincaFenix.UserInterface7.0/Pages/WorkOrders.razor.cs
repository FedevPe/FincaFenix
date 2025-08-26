using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UserInterface7._0.Components;
using FincaFenix.UserInterface7._0.Services;
using FincaFenix.ViewModels.ViewModels;
using FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Pages
{
    public partial class WorkOrders
    {
        [Inject] public LoadWorkOrdersViewModel ViewModel { get; set; }
        [Inject] public TextAppBarStateService TextApp { get; set; }

        private bool renderWorkOrderCard = false;
        private string textContent = "Ordenes de Trabajo";
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            TextApp.PageTitle = "Ordenes de Trabajo";
            await LoadDataAsync();
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
        private async Task HandleSearch(SearchBarViewModel filter)
        {
            _isLoading = true;
            StateHasChanged(); // Muestra el indicador de carga

            try
            {
                // Lógica para filtrar la lista de órdenes de trabajo
                // Aquí debes llamar a un nuevo método en tu ViewModel
                // que acepte los parámetros de búsqueda.

                // Ejemplo:
                await ViewModel.LoadFilteredWorkOrdersAsync(filter);
            }
            catch (Exception ex)
            {
                // Manejar errores
            }
            finally
            {
                _isLoading = false;
                StateHasChanged(); // Oculta el indicador y renderiza los datos
            }
        }
    }
}