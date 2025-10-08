using FincaFenix.Entities.DTOs.Common;
using FincaFenix.Entities.DTOs.Validation;
using FincaFenix.UsesCases.Interfaces.OutputPort.WorkOrder;

namespace FincaFenix.Presenters.Implementations.WorkOrder
{
    public class CreateWorkOrderPresenter : ICreateWorkOrderOutputPort
    {
        public OperationResultDTO Result { get; private set; } = new();

        public Task Handle(int id)
        {
            Result = new OperationResultDTO
            {
                Success = id != 0
            };
            return Task.CompletedTask;
        }

        public Task HandleErrors(IEnumerable<ValidationDTO> validationFailures)
        {
            Result = new OperationResultDTO
            {
                Success = false,
                Errors = validationFailures
            };
            return Task.CompletedTask;
        }
    }
}
