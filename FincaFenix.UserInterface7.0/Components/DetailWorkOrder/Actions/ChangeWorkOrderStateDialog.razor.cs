using FincaFenix.ViewModels.ViewModels.UpdateWorkOrder;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface7._0.Components.DetailWorkOrder.Actions
{
    public partial class ChangeWorkOrderStateDialog
    {
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public string CurrentStatus { get; set; } = "";
        [Parameter] public int WorkOrderId { get; set; }

        [Inject] private UpdateWorkOrderViewModel UpdateWorkOrderViewModel { get; set; }
        [Inject] private ISnackbar Snackbar { get; set; }
        [Inject] private NavigationManager NavigationManager { get; set; }

        private string selectedState = null;
        private bool isProcessing = false;

        protected override void OnParametersSet()
        {
            if (IsVisible)
            {
                // Reset selection when dialog opens
                selectedState = null;
            }
        }

        private void CloseDialog() => MudDialog.Cancel();

        private void SelectState(string state)
        {
            selectedState = state;
        }

        private async Task ConfirmStateChange()
        {
            if (selectedState == null || selectedState == CurrentStatus)
            {
                Snackbar.Add("Debe seleccionar un estado diferente al actual", Severity.Warning);
                return;
            }

            isProcessing = true;
            StateHasChanged();

            try
            {
                var result = await UpdateWorkOrderViewModel.UpdateWorkOrderStatus(WorkOrderId, selectedState);
                await Task.Delay(1500);

                if (result)
                {
                    string message = $"El estado de la orden se ha modificado correctamente.";

                    Snackbar.Add(message, Severity.Success);
                    MudDialog.Close(DialogResult.Ok(result));
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error al cambiar el estado: {ex.Message}", Severity.Error);
                MudDialog.Close(DialogResult.Cancel());
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
            }
        }

        private string GetStatusChipClass(string status) => status switch
        {
            "Cerrado" => "status-cerrado",
            "Pendiente" => "status-pendiente",
            _ => "status-activo"
        };

        private List<StateOption> GetAvailableStates()
        {
            var allStates = new List<StateOption>
            {
                new StateOption("Activo", "Orden en progreso, trabajo en curso"),
                new StateOption("Pendiente", "Orden pausada temporalmente"),
                new StateOption("Cerrado", "Orden completada y finalizada")
            };

            return allStates.Where(s => IsStateTransitionAllowed(CurrentStatus, s.Value)).ToList();
        }

        private bool IsStateTransitionAllowed(string currentStatus, string newStatus)
        {
            if (currentStatus == newStatus) return false;

            return currentStatus switch
            {
                "Activo" => newStatus is "Pendiente" or "Cerrado",
                "Pendiente" => newStatus is "Activo" or "Cerrado",
                "Cerrado" => false, // Normalmente no se puede cambiar desde cerrado
                _ => true
            };
        }

        // Clases auxiliares
        public class StateOption
        {
            public string Value { get; set; }
            public string Description { get; set; }

            public StateOption(string value, string description)
            {
                Value = value;
                Description = description;
            }
        }

        public class StateChangeResult
        {
            public bool Success { get; set; }
            public string OldStatus { get; set; }
            public string NewStatus { get; set; }
            public string Message { get; set; }
            public DateTime ChangedAt { get; set; } = DateTime.Now;
        }
    }
}
