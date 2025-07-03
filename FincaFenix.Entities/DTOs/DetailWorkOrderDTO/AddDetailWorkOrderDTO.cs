using System.ComponentModel.DataAnnotations;

namespace FincaFenix.Entities.DTOs.DetailWorkOrderDTO
{
    public class AddDetailWorkOrderDTO
    {
        public int OrderId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime ActivityDate { get; set; }
        public InfoDetailWorkOrderDTO Info { get; set; } = new InfoDetailWorkOrderDTO();

    }
}
