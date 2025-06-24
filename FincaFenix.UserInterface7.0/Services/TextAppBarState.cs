namespace FincaFenix.UserInterface7._0.Services
{
    public class TextAppBarState
    {
        private readonly SynchronizationContext _syncContext;

        public TextAppBarState()
        {
            // Capturamos el contexto de sincronización del hilo de UI al momento de construcción
            _syncContext = SynchronizationContext.Current!;
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
            // Ejecutamos el evento dentro del hilo correcto del render
            _syncContext.Post(_ => OnChange?.Invoke(), null);
        }
    }
}
