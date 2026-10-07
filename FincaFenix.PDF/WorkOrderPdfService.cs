using FincaFenix.Entities.DTOs.ShowWorkOrder;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace FincaFenix.PDF
{
    public class WorkOrderPdfService : IPdfGenerationService
    {
        public byte[] GenerateWorkOrderPdf(ShowWorkOrderDTO workOrder)
        {
            var document = new WorkOrderPDF(workOrder);
            return document.GeneratePdf();
        }
    }
}
