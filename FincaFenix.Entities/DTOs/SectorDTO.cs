namespace FincaFenix.Entities.DTOs
{
    public class SectorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public bool Seleccionado { get; set; } = false;
    }
}
