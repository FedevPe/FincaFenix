using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.PDF;
using FincaFenix.UserInterface7._0.Services;
using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder
{
    public partial class DetailWorkOrderActions
    {
        [Parameter] public ShowWorkOrderDTO InfoWO { get; set; }
        [Inject] private PDFService PDFService { get; set; }

        private string GetStatusChipClass(string status) => status switch
        {
            "Cerrado" => "status-cerrado",
            "Pendiente" => "status-pendiente",
            "Activo" => "status-activo",
            "Creado" => "status-creado",
            _ => "status-creado"
        };

        public async Task GeneratePDF(ShowWorkOrderDTO infoWO)
        {
            var document = new WorkOrderPDF(infoWO);
            await PDFService.DownloadPdfAsync(document, $"Orden_de_trabajo_N°_{infoWO.OrderNum}");
        }
    }
}