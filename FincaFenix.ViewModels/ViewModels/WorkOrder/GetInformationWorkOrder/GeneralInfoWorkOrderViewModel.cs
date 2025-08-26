using FincaFenix.Entities.DTOs.ShowWorkOrder;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels.WorkOrder.GetInformationWorkOrder
{
    public class GeneralInfoWorkOrderViewModel(
        IWorkOrderController workOrder)
    {
        public ShowWorkOrderDTO WOInfo { get; private set; } = new ShowWorkOrderDTO();

        // Propiedades adicionales del ViewModel (ej. estado de carga, mensajes de error, etc.)
        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public async Task LoadDataWorkOrderById(int id)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                WOInfo = await workOrder.GetWorkOrderAndRecipeByIdWorkorder(id);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar la información: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
