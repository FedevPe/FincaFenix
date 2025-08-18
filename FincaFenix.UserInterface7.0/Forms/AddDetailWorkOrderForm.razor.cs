using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
using FincaFenix.Entities.DTOs.WorkOrderDTOs;
using FincaFenix.UserInterface7._0.Validators.DetailWorkOrder;
using FincaFenix.UserInterface7._0.Validators.RegisterActivity;
using FincaFenix.ViewModels.ViewModels;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Forms
{
    public partial class AddDetailWorkOrderForm
    {
        [Inject] public GeneralInfoWorkOrderViewModel InfoVM { get; set; }
        [Inject] public RegistryActivityViewModel ViewModel { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }
        [Inject] private RegistryActivityViewModelValidator ValidatorVM { get; set; }
        [Inject] private DetailActivityUIValidator ValidatorAR { get; set; }

        [Parameter] public int WorkOrderId { get; set; }

        private MudForm form;
        private bool IsAddButtonDisabled =>
            ViewModel.ActivityDetails.Count >= (InfoVM.WOInfo?.SectorList.Count() ?? 0);

        protected override async Task OnInitializedAsync()
        {
            await InfoVM.LoadDataWorkOrderById(WorkOrderId);
            ViewModel.ActivityDetails.Add(new AddDetailWorkOrderDTO());

            await ViewModel.LoadEmployeesAsync(InfoVM.WOInfo.Farm.Id);

            ViewModel.WorkOrderId = WorkOrderId;
            ViewModel.ActivityDate = DateTime.Now;
        }
        private IEnumerable<DetailSectorFarmDTO> GetAvailableSectors(int currentSectorId)
        {
            // Obtiene los IDs de los sectores que ya están seleccionados en la tabla
            var selectedIds = ViewModel.ActivityDetails
                                        .Where(d => d.Info.SectorWorkedId != 0 && d.Info.SectorWorkedId != currentSectorId)
                                        .Select(d => d.Info.SectorWorkedId)
                                        .ToList();

            // Filtra la lista original de sectores para mostrar solo los no seleccionados
            return InfoVM.WOInfo.SectorList.Where(s => !selectedIds.Contains(s.Id));
        }
        private void AddDetailWorkOrder()
        {
            ViewModel.ActivityDetails.Add(new AddDetailWorkOrderDTO());
        }
        private void RemoveDetailWorkOrder(AddDetailWorkOrderDTO detail)
        {
            ViewModel.ActivityDetails.Remove(detail);
        }
        private async Task HandleSave()
        {
            // 1. Validar el formulario de la UI
            await form.Validate();
            if (!form.IsValid)
            {
                Snackbar.Add("Por favor, completá todos los campos requeridos.", MudBlazor.Severity.Error);
                return;
            }

            // ---

            // 2. Validar las horas trabajadas
            var totalHours = SumWorkHours();
            if (totalHours <= 0 || totalHours > 9)
            {
                Snackbar.Add("El total de horas trabajadas debe ser mayor a 0 y menor o igual a 9.", MudBlazor.Severity.Error);
                return;
            }

            // 3. Aquí es donde llamamos al Caso de Uso de la Capa de Aplicación
            // Le pasamos el ViewModel del formulario, que contiene todos los datos de entrada
            if (await ViewModel.SaveDetailWorkOrderAsync(ViewModel))
            {
                Snackbar.Add("Actividad registrada con éxito.", MudBlazor.Severity.Success);
                NavigationManager.NavigateTo($"/ordenestrabajo"); // Redirige después de guardar
            }
            else
            {
                Snackbar.Add("Error al registrar la actividad.", MudBlazor.Severity.Error);
            }

        }
        private decimal SumWorkHours()
        {
            decimal totalHours = 0;
            foreach (var detail in ViewModel.ActivityDetails)
            {
                totalHours += detail.Info.WorkedHours;
            }
            return totalHours;
        }
    }
}