using FincaFenix.Entities.DTOs.ShowWorkOrder;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.Shared
{
    public partial class GeneralInformationWorkOrder
    {
        [Parameter] public ShowWorkOrderDTO InfoWO { get; set; }
        private string mudIcon = "fa-solid fa-chevron-down";
        private bool _expanded = false;

        private string GetStatusChipClass(string status) => status switch
        {
            "Cerrado" => "status-cerrado",
            "Pendiente" => "status-pendiente",
            "Activo" => "status-activo",
            "Creado" => "status-creado",
            _ => "status-creado"
        };

        private string GetStatusIcon(string status) => status switch
        {
            "Cerrado" => Icons.Material.Filled.CheckCircle,
            "Pendiente" => Icons.Material.Filled.Schedule,
            "Activo" => Icons.Material.Filled.PlayCircle,
            "Creado" => Icons.Material.Filled.NewReleases,
            _ => Icons.Material.Filled.HelpOutline
        };
        private void OnExpandCollapseClick()
        {
            _expanded = !_expanded;

            if (_expanded)
            {
                mudIcon = "fa-solid fa-chevron-up";
            }
            else
            {
                mudIcon = "fa-solid fa-chevron-down";
            }
        }
    }
}