using GalaSoft.MvvmLight;
using DesktopClient.Model;

namespace DesktopClient.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// The <see cref="WelcomeTitle" /> property's name.
        /// </summary>
        public const string WelcomeTitlePropertyName = "WelcomeTitle";

        private string _welcomeTitle = string.Empty;

        /// <summary>
        /// Gets the WelcomeTitle property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                if (_welcomeTitle == value)
                {
                    return;
                }

                _welcomeTitle = value;
                RaisePropertyChanged(WelcomeTitlePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentPage" /> property's name.
        /// </summary>
        public const string CurrentPagePropertyName = "CurrentPage";

        private string _currentPage = "1";

        /// <summary>
        /// Sets and gets the CurrentPage property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                if (_currentPage == value)
                {
                    return;
                }

                _currentPage = value;
                RaisePropertyChanged(CurrentPagePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentUserNumber" /> property's name.
        /// </summary>
        public const string CurrentUserNumberPropertyName = "CurrentUserNumber";

        private string _currentUserNumber = "";

        /// <summary>
        /// Sets and gets the CurrentUserNumber property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentUserNumber
        {
            get
            {
                return _currentUserNumber;
            }

            set
            {
                if (_currentUserNumber == value)
                {
                    return;
                }

                _currentUserNumber = value;
                RaisePropertyChanged(CurrentUserNumberPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="CurrentUserName" /> property's name.
        /// </summary>
        public const string CurrentUserNamePropertyName = "CurrentUserName";

        private string _currentUserName = "";

        /// <summary>
        /// Sets and gets the CurrentUserName property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                return _currentUserName;
            }

            set
            {
                if (_currentUserName == value)
                {
                    return;
                }

                _currentUserName = value;
                RaisePropertyChanged(CurrentUserNamePropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.OnGetUserInfo += _dataService_OnGetUserInfo;
            _dataService.GetUserInfo();
        }

        void _dataService_OnGetUserInfo(object sender, ServiceContract.UserInfo e)
        {
            WelcomeTitle = e.User.name;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}