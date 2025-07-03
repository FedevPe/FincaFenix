using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels
{
    public class WorkOrderDataViewModel(
        IWorkOrderController workOrder)
    {
        public ShowInfoAddActivityFormDTO FormData { get; private set; } = new ShowInfoAddActivityFormDTO();

        // Propiedades adicionales del ViewModel (ej. estado de carga, mensajes de error, etc.)
        public bool IsLoading { get; private set; }
        public string? ErrorMessage { get; private set; }
        public async Task LoadDataWorkOrderById(int id)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                var info = await workOrder.GetWorkOrderInfoById(id);
                // Si info es null (ej. no se encontró la orden), manejarlo
                if (info != null)
                {
                    FormData = info; // Asignar el DTO completo
                }
                else
                {
                    // Manejar el caso de orden no encontrada
                    ErrorMessage = $"Orden de trabajo con ID {id} no fue encontrada.";
                    FormData = new ShowInfoAddActivityFormDTO(); // O dejarlo en null para que el UI maneje el estado
                }
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
