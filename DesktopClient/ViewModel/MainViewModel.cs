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
        /// The <see cref="DiaryItem" /> property's name.
        /// </summary>
        public const string DiaryItemPropertyName = "DiaryItem";

        private DBModel.domainDiary _diaryItem;

        /// <summary>
        /// Sets and gets the DiaryItem property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DBModel.domainDiary DiaryItem
        {
            get
            {
                return _diaryItem;
            }

            set
            {
                if (_diaryItem == value)
                {
                    return;
                }

                _diaryItem = value;
                RaisePropertyChanged(DiaryItemPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="DiaryItems" /> property's name.
        /// </summary>
        public const string DiaryItemsPropertyName = "DiaryItems";

        private ObservableCollection<DBModel.domainDiary> _diaryItems;

        /// <summary>
        /// Sets and gets the DiaryItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<DBModel.domainDiary> DiaryItems
        {
            get
            {
                return _diaryItems;
            }

            set
            {
                if (_diaryItems == value)
                {
                    return;
                }

                _diaryItems = value;
                RaisePropertyChanged(DiaryItemsPropertyName);
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

        private DBModel.codeUsers _userInfo = null;

        /// <summary>
        /// Sets and gets the UserInfo property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DBModel.codeUsers UserInfo
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

        public RelayCommand SendMailCommand { get; set; }
        public RelayCommand EditUserCommand { get; set; }

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

            SendMailCommand = new RelayCommand(_sendMailExecute, _canSendMailExecute);
            EditUserCommand = new RelayCommand(_EditUserExecute, _canEditUserExecute);
        }

        private bool _canEditUserExecute()
        {
            return this.UserInfo != null && this.UserInfo.id > 0;
        }

        private void _EditUserExecute()
        {
            Messenger.Default.Send<DBModel.codeUsers>(this.UserInfo, "ShowUserEditView");

        }

        private bool _canSendMailExecute()
        {
            return true;
        }

        private void _sendMailExecute()
        {
            /*
             调用服务器的生成Excel日志并发送邮件服务
             */

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