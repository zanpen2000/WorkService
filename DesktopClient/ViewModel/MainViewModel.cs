using GalaSoft.MvvmLight;
using DesktopClient.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

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
        /// The <see cref="UserInfo" /> property's name.
        /// </summary>
        public const string UserInfoPropertyName = "UserInfo";

        private DBModel.viewUserInfo _userInfo = null;

        /// <summary>
        /// Sets and gets the UserInfo property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DBModel.viewUserInfo UserInfo
        {
            get
            {
                return _userInfo;
            }

            set
            {
                if (_userInfo == value)
                {
                    return;
                }

                _userInfo = value;
                RaisePropertyChanged(UserInfoPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="Diarys" /> property's name.
        /// </summary>
        public const string DiarysPropertyName = "Diarys";



        private ObservableCollection<DBModel.viewUserDiarys> _diarys = null;

        /// <summary>
        /// Sets and gets the Diarys property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<DBModel.viewUserDiarys> Diarys
        {
            get
            {
                return _diarys;
            }

            set
            {
                if (_diarys == value)
                {
                    return;
                }

                _diarys = value;
                RaisePropertyChanged(DiarysPropertyName);
            }
        }

        private RelayCommand _addCommand;

        /// <summary>
        /// Gets the AddCommand.
        /// </summary>
        public RelayCommand AddCommand
        {
            get
            {
                return _addCommand
                    ?? (_addCommand = new RelayCommand(
                    () =>
                    {
                        Messenger.Default.Send<object>(null, "ShowDiaryView");

                    }));
            }
        }



        private RelayCommand _closeCommand;

        /// <summary>
        /// Gets the CloseCommand.
        /// </summary>
        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(
                    () =>
                    {
                        App.Current.Shutdown();
                    }));
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.OnGetUserInfo += _dataService_OnGetUserInfo;
            _dataService.OnGetUserDiarys += _dataService_OnGetUserDiarys;

            _dataService.GetUserInfo();
            _dataService.GetDiarys(CurrentPage);
        }

        void _dataService_OnGetUserDiarys(object sender, ServiceContract.ViewDiarysEventArgs e)
        {
            this.Diarys = new ObservableCollection<DBModel.viewUserDiarys>(e.Diarys);
        }

        void _dataService_OnGetUserInfo(object sender, ServiceContract.UserInfoEventArgs e)
        {
            this.UserInfo = e.UserInfo;
        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}
    }
}