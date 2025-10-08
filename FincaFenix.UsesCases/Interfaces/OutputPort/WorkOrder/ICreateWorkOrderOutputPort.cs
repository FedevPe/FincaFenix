using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.Validation;

namespace FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder
{
    public interface ICreateWorkOrderOutputPort
    {
        public OperationResultDTO Result { get; }
        Task Handle(int id);
        Task HandleErrors(IEnumerable<ValidationDTO> validationFailures);
    }
}
