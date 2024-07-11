using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace prism_simpletemplate.ViewModels
{
    /// <summary>
    /// Represents the view model for the main window.
    /// </summary>
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            this._regionManager = regionManager;
            NavigateCommand = new DelegateCommand<string>(OnNavigate);
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                    journal.GoBack();
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                    journal.GoForward();
            });


        }
        private readonly IRegionManager _regionManager;
        private IRegionNavigationJournal journal;

        /// <summary>
        /// Gets the command to navigate to a specific region.
        /// </summary>
        public DelegateCommand<string> NavigateCommand { set; get; }

        /// <summary>
        /// Gets the command to navigate back in the region's navigation history.
        /// </summary>
        public DelegateCommand GoBackCommand { set; get; }

        /// <summary>
        /// Gets the command to navigate forward in the region's navigation history.
        /// </summary>
        public DelegateCommand GoForwardCommand { set; get; }

        private void OnNavigate(string obj)
        {
            _regionManager.Regions["ContentRegion"].RequestNavigate(obj, callback =>
            {
                if ((bool)callback.Result)
                {
                    journal = callback.Context.NavigationService.Journal;
                }
            });
        }
    }
}
