using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.PDF;
using FincaFenix.UserInterface7._0.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.Shared
{
    public partial class WorkOrderTable
    {
        [Parameter] public IEnumerable<ShowWorkOrderDTO> WorkOrderList { get; set; }

        [Inject] private PDFService PDFService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        public void OpenActivityForm(string woSelectedStatus, int idWorkOrder, string workOrderNum)
        {
            if (woSelectedStatus == "Activo")
            {
                NavigationManager.NavigateTo($"/actividad/registrar/{idWorkOrder}");
            }
            else
            {
                Snackbar.Add($"La orden de trabajo N° {workOrderNum} no se encuentra activa.", MudBlazor.Severity.Info);
            }
        }
        public void OpenWorkOrderDetails(int idWorkOrder)
        {
            NavigationManager.NavigateTo($"/ordentrabajo/{idWorkOrder}/detalle/");
        }
        public async Task GeneratePDF(ShowWorkOrderDTO infoWO)
        {
            var document = new WorkOrderPDF(infoWO);
            await PDFService.DownloadPdfAsync(document, $"Orden_de_trabajo_N°_{infoWO.OrderNum}");
        }
    }
}