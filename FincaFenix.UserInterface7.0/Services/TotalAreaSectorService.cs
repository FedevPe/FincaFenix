namespace FincaFenix.UserInterface7._0.Services
{
    public class TotalAreaSectorService
    {
        public decimal? TotalAreaSectors { get; private set; } = 0; // Private set para controlar las actualizaciones

        // Evento para notificar a los suscriptores cuando el área cambia
        public event Action OnChange;

        public void SetTotalAreaSectors(decimal? newArea)
        {
            if (TotalAreaSectors != newArea) // Solo actualizar si el valor realmente cambió
            {
                TotalAreaSectors = newArea;
                NotifyStateChanged();
            }
        }
        public void SumArea(decimal? area)
        {
            TotalAreaSectors += area;
            SetTotalAreaSectors(TotalAreaSectors);
            NotifyStateChanged();
        }
        public void SubstractArea(decimal? area)
        {
            TotalAreaSectors -= area;
            SetTotalAreaSectors(TotalAreaSectors);
            NotifyStateChanged();
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
