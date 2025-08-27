using FincaFenix.UserInterface7._0.Services;
using FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FincaFenix.UserInterface7._0.Pages
{
    public partial class DetailWorkOrder
    {
        [Inject] private GeneralInfoWorkOrderViewModel ViewModel { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; }
        [Inject] private LoadDetailsWorkOrderViewModel DetailsVM { get; set; }
        [Inject] public TextAppBarStateService TextApp { get; set; }
        [Inject] public NavigationManager NavigationManager { get; set; }
        [Parameter] public int IdWorkOrder { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TextApp.PageTitle = "Detalle de la orden de trabajo";
            await ViewModel.LoadDataWorkOrderById(IdWorkOrder);
            await DetailsVM.LoadActivityLog(IdWorkOrder);
            await Task.Delay(1500);
            StateHasChanged();
        }
        private async Task HandleStateChange()
        {
            //await ViewModel.LoadDataWorkOrderById(IdWorkOrder);
            //await DetailsVM.LoadActivityLog(IdWorkOrder);
            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
            StateHasChanged();
            await Task.CompletedTask;
        }
        // Enum para las pestañas
        public enum WorkOrderTab
        {
            Activities,
            Recipe,
            Consumption
        }

        private WorkOrderTab activeTab = WorkOrderTab.Activities;

        private async Task SetActiveTab(WorkOrderTab tab)
        {
            activeTab = tab;
            await Task.Delay(50);
            await ScrollToElement("data");            
        }

        private async Task ScrollToElement(string elementId)
        {
            await JSRuntime.InvokeVoidAsync("scrollToElement", elementId);
        }

        private string GetTabActiveClass(WorkOrderTab tab)
        {
            return activeTab == tab ? "active" : "";
        }
    }
}