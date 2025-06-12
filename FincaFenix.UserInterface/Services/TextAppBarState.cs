namespace FincaFenix.UserInterface.Services
{
    public class TextAppBarState
    {
        public event Action OnChange;
        private string _pageTitle = "Inicio";
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
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
