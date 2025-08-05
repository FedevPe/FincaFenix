using FincaFenix.Entities.DTOs.DetailWorkOrderDTO;
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

        protected override async Task OnInitializedAsync()
        {
            await InfoVM.LoadDataWorkOrderById(WorkOrderId);

            if (!ViewModel.ActivityDetails.Any())
            {
                ViewModel.ActivityDetails.Add(new AddDetailWorkOrderDTO());
            }

            ViewModel.WorkOrderId = WorkOrderId;
            ViewModel.ActivityDate = DateTime.Now;
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
            Snackbar.Add($"Guardando actividad... HorasTotales = {SumWorkHours()}", MudBlazor.Severity.Info);
            if (SumWorkHours() > 9)
            {
                Snackbar.Add("El total de horas trabajadas no puede ser mayor a 9 horas.", MudBlazor.Severity.Warning);
                return;
            }
            else if (SumWorkHours() <= 8)
            {
                Snackbar.Add("El total de horas trabajadas debe ser mayor o igual a 8 horas.", MudBlazor.Severity.Warning);
                return;
            }

            // 2. Aquí es donde llamamos al Caso de Uso de la Capa de Aplicación
            // Le pasamos el ViewModel del formulario, que contiene todos los datos de entrada
            try
            {
                if (await ViewModel.SaveDetailWorkOrderAsync(ViewModel))
                {
                    Snackbar.Add("Actividad registrada con éxito.", MudBlazor.Severity.Success);
                    NavigationManager.NavigateTo($"/ordenestrabajo"); // Redirige después de guardar
                }
                else
                {
                    Snackbar.Add("Error al registrar la actividad. Inténtelo de nuevo.", MudBlazor.Severity.Error);
                }
            }
            catch (Exception ex)
            {
                // Capturar errores inesperados del caso de uso o la persistencia
                Snackbar.Add($"Ocurrió un error inesperado: {ex.Message}", MudBlazor.Severity.Error);
                // Considera loggear el error completo (ex) aquí para depuración
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