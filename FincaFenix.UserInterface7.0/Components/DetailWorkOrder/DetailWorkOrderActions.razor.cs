using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.PDF;
using FincaFenix.UserInterface7._0.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder
{
    public partial class DetailWorkOrderActions
    {
        [Parameter] public ShowWorkOrderDTO InfoWO { get; set; }
        [Parameter] public EventCallback OnStateChanged { get; set; }
        [Inject] private PDFService PDFService { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private IDialogService DialogService { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }

        private async Task OpenChangeStateDialog(int workOrderId, string currentStatus)
        {
            if (InfoWO.Status != "Cerrado")
            {
                var options = new DialogOptions()
                {
                    MaxWidth = MaxWidth.Small,
                    BackdropClick = false,
                    Position = DialogPosition.Center,
                    CloseOnEscapeKey = false,
                    BackgroundClass = "my-custom-dialog",
                    FullWidth = true
                };
                var parameters = new DialogParameters<ChangeWorkOrderStateDialog>();
                parameters.Add(x => x.CurrentStatus, currentStatus);
                parameters.Add(x => x.WorkOrderId, workOrderId);

                var dialog = DialogService.Show<ChangeWorkOrderStateDialog>("Cambiar Estado", parameters, options);
                var result = await dialog.Result;

                if (!result.Canceled)
                {
                    await OnStateChanged.InvokeAsync();
                }
            }
            else
            {
                Snackbar.Add("No es posible modificar el estado de la orden.", Severity.Info);
            }
        }

        // Método existente para obtener las clases CSS del estado
        private string GetStatusChipClass(string status) => status switch
        {
            "Cerrado" => "status-cerrado",
            "Pendiente" => "status-pendiente",
            _ => "status-activo"
        };

        // Método existente
        private void OpenActivityForm()
        {
            NavigationManager.NavigateTo($"/actividad/registrar/{InfoWO.Id}");
        }
        // Método existente
        public async Task GeneratePDF(ShowWorkOrderDTO infoWO)
        {
            var document = new WorkOrderPDF(infoWO);
            await PDFService.DownloadPdfAsync(document, $"Orden_de_trabajo_N°_{infoWO.OrderNum}");
        }        
    }
}