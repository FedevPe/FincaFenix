using System.Collections.ObjectModel;

namespace FincaFenix.UsesCases.BusinessObject.WorkOrderVO
{
    public class SectorListVO
    {
        private readonly IReadOnlyCollection<SectorVO> _sectores;
        public IReadOnlyCollection<SectorVO> Sectores => _sectores;

        public SectorListVO(IEnumerable<SectorVO> sectores)
        {
            if (sectores is null)
                throw new ArgumentNullException(nameof(sectores));

            var sectoresList = sectores.ToList();

            if (sectoresList.Count() == 0)
                throw new ArgumentException("La lista de cuadros seleccionados esta vacía", nameof(sectores));

            _sectores = new ReadOnlyCollection<SectorVO>(sectoresList);
        }
    }
}
