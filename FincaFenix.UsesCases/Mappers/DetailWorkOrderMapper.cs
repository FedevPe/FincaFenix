using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.AddDetailWorkOrder;
using FincaFenix.Entities.POCOEntities;

namespace FincaFenix.UsesCases.Mappers
{
    public class DetailWorkOrderMapper
    {
        public static DetailWorkOrderEntity ToEntity(AddDetailWorkOrderDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto), "Detail work order DTO cannot be null.");
            }
            return new DetailWorkOrderEntity
            {
                WorkOrderId = dto.OrderId,
                EmployeeId = dto.EmployeeId,
                SectorWorkedId = dto.Info.SectorWorkedId,
                Performance = dto.Info.Performance,
                WorkedHours = dto.Info.WorkedHours,
                Description = dto.Info.Description.ToUpper() ?? string.Empty,
                ActivityDate = (DateTime)dto.ActivityDate
            };
        }
    }
}
