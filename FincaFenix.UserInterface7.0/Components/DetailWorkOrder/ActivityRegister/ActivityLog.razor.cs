using FincaFenix.Entities.DTOs.DetailWorkOrderDTO.GetDetailWorkOrder;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder.ActivityRegister
{
    public partial class ActivityLog
    {
        private string _searchString;

        [Parameter] public IEnumerable<DetailWorkOrderDTO> ActivityList { get; set; }

        // Filt
        // Filtro rápido para buscar por nombre de operario
        private Func<DetailWorkOrderDTO, bool> _quickFilter => activity =>
        {
            if (string.IsNullOrWhiteSpace(_searchString))
                return true;
            if (activity.Employee.Name.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;
            if (activity.Employee.LastName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };
    }
}