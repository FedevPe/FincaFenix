using FincaFenix.Entities.DTOs.ShowWorkOrder;

namespace FincaFenix.PDF
{
    public interface IPdfGenerationService
    {
        byte[] GenerateWorkOrderPdf(ShowWorkOrderDTO workOrder);
    }
}
