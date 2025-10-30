using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using FincaFenix.UsesCases.Controllers;

namespace FincaFenix.ViewModels.ViewModels.WorkOrderDetails
{
    public class RegistryActivityViewModel(
        IDetailWorkOrderController detailWO,
        IEmployeeController emController)
    {
        public int WorkOrderId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? ActivityDate { get; set; } = DateTime.Now;
        public IEnumerable<EmployeeDTO> Employees { get; set; } = new List<EmployeeDTO>();
        public List<AddDetailWorkOrderDTO> ActivityDetails { get; set; } = new List<AddDetailWorkOrderDTO>();

        public async Task LoadEmployeesAsync(int idFarm)
        {
            Employees = await emController.GetAllEmployeesAsync(idFarm);
        }

        public async Task<bool> SaveDetailWorkOrderAsync(RegistryActivityViewModel viewModel)
        {
            try
            {
                if (EmployeeId <= 0)
                {
                    return false;
                }
                if (!ActivityDetails.Any())
                {
                    return false; 
                }
                foreach (var detailDto in ActivityDetails)
                {
                    detailDto.OrderId = WorkOrderId;
                    detailDto.EmployeeId = EmployeeId;
                    detailDto.ActivityDate = ActivityDate;
                    await detailWO.CreateDetailWorkOrder(detailDto);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar actividad en ViewModel: {ex.Message}");
                return false;
            }
        }
    }
}
