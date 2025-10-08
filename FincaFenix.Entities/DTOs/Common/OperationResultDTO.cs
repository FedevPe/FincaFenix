using FincaFenix.Entities.DTOs.Validation;

namespace FincaFenix.Entities.DTOs.Common
{
    public class OperationResultDTO
    {
        public bool Success { get; set; }
        public IEnumerable<ValidationDTO> Errors { get; set; } = new List<ValidationDTO>();
    }
}
