namespace FincaFenix.Entities.DTOs.EmployeeDTOs
{
    public class SaveEmployeeDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public List<int> FarmIds { get; set; } = new();
    }
}