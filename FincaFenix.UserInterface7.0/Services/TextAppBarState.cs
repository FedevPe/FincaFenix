using Microsoft.AspNetCore.Components;

namespace FincaFenix.UserInterface7._0.Services
{
    public class TextAppBarState
    {
        private Func<Func<Task>, Task> _invokeAsync;

        public void ConfigureDispatcher(Func<Func<Task>, Task> dispatcherInvoker)
        {
            _invokeAsync = dispatcherInvoker;
        }

        public event Action OnChange;

        private string _pageTitle = "Menú Principal";
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                if (_pageTitle != value)
                {
                    _pageTitle = value;
                    NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged()
        {
            if (_invokeAsync is not null)
                _ = _invokeAsync(() => {
                    OnChange?.Invoke();
                    return Task.CompletedTask;
                });
            else
                OnChange?.Invoke();
        }
    }
}
