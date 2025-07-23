using FincaFenix.Entities.POCOEntities;
using FincaFenix.Validations.Guards;

namespace FincaFenix.Validations.Validators
{
    public class WorkOrderValidator
    {
        public static void Validate(WorkOrderEntity workOrder, bool validateIdWorkOrder = false)
        {
            if (workOrder == null)
            {
                throw new ArgumentNullException(nameof(workOrder));
            }
            if (validateIdWorkOrder)
            {
                Guard.Against(workOrder.Id, nameof(workOrder.Id)).IsEqualTo(0);
            }
            //Fecha de inicio de la oren
            Guard.Against(workOrder.CreatedDate, nameof(workOrder.CreatedDate))
                .IsNull()
                .LessThan(DateTime.Now);
            
            //Finca seleccionada
            Guard.Against(workOrder.FarmId, nameof(workOrder.FarmId))
                .IsEqualTo(0, "La orden de trabajo debe tener una FINCA relacionada.");
            //Tarea seleccionada
            Guard.Against(workOrder.TaskId, nameof(workOrder.TaskId))
                .IsEqualTo(0, "La orden de trabajo debe tener una TAREA relacionada.");
            //Sectores seleccionados
            foreach(var sector in workOrder.WorkedSectors)
            {
                Guard.Against(sector.Id, nameof(sector.Id)).IsEqualTo(0, "El sector seleccionado no es valido.");
            }
        }
    }
}
