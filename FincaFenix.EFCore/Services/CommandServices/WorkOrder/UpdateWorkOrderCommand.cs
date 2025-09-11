using FincaFenix.EFCore.Context;
using FincaFenix.Gateways.Interfaces.CommandServices.WorkOrder;
using Microsoft.EntityFrameworkCore;

namespace FincaFenix.EFCore.Services.CommandServices.WorkOrder
{
    public class UpdateWorkOrderCommand(
        FincaFenixContext context) : IUpdateWorkOrderCommand
    {
        public Task<bool> UpdateWorkOrder()
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateWorkOrderState(int workOrderId, string newStatus)
        {
            try
            {
                // 1. Buscar la orden de trabajo en la base de datos por su ID
                var workOrder = await context.WorkOrders.FirstOrDefaultAsync(wo => wo.Id == workOrderId);

                // Si la orden de trabajo no se encuentra, retornamos false
                if (workOrder == null)
                {
                    return false;
                }

                // 2. Actualizar el estado de la orden de trabajo
                if (!string.IsNullOrEmpty(newStatus))
                {
                    workOrder.Status = newStatus;

                    if(newStatus == "Cerrado")
                    {
                        workOrder.EndDate = DateTime.Now;
                    }
                }

                // 3. Guardar los cambios en la base de datos de forma asíncrona
                int changesSaved = await context.SaveChangesAsync();

                // 4. Retornar true si se guardó al menos 1 cambio, de lo contrario false
                return changesSaved > 0;
            }
            catch (Exception)
            {
                // En caso de cualquier error, relanzamos la excepción para un manejo superior
                throw;
            }
        }
    }
}
