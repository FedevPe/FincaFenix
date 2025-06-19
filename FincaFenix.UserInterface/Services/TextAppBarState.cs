namespace FincaFenix.UserInterface.Services{
    public class TextAppBarState
    {
        public event Action OnChange;
        private string _pageTitle = "Menu Principal";
        public string PageTitle
        {
            get => _pageTitle;
            set {
                if (_pageTitle != value) {
                    _pageTitle = value;
                    NotifyStateChanged();
                }
            }
        }
        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
