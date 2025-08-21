using FincaFenix.UserInterface7._0.Services;
using FincaFenix.ViewModels.ViewModels;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Pages
{
    public partial class DetailWorkOrder
    {
        [Inject] private GeneralInfoWorkOrderViewModel InfoVM { get; set; }
        [Inject] private LoadDetailsWorkOrderViewModel DetailsVM { get; set; }
        [Inject] public TextAppBarStateService TextApp { get; set; }
        [Parameter] public int IdWorkOrder { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TextApp.PageTitle = "Detalle de la orden de trabajo";
            await InfoVM.LoadDataWorkOrderById(IdWorkOrder);
            await DetailsVM.LoadActivityLog(IdWorkOrder);
            await Task.Delay(0);
            StateHasChanged();
        }
        // Enum para las pestañas
        public enum WorkOrderTab
        {
            Activities,
            Recipe
        }

        private WorkOrderTab activeTab = WorkOrderTab.Activities;

        private void SetActiveTab(WorkOrderTab tab)
        {
            activeTab = tab;
        }

        private string GetTabActiveClass(WorkOrderTab tab)
        {
            return activeTab == tab ? "active" : "";
        }
    }
}