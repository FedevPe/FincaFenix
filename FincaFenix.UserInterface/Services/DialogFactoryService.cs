using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace FincaFenix.UserInterface.Services
{
    public class DialogFactoryService
    {
        private readonly IDialogService _dialogService;
        public DialogFactoryService(IDialogService dialogService)
        {
            _dialogService = dialogService;
            
        }

        public async Task<IDialogReference> ShowFormDialogAsync<T>(string title) where T : ComponentBase
        {
            var options = new DialogOptions
            {
                FullScreen = false,
                CloseButton = true,
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.ExtraLarge,
                BackdropClick = false,
                BackgroundClass = "custom-dialog",
            };           

            return await _dialogService.ShowAsync<T>(title, options);
        }
    }
}
