using GalaSoft.MvvmLight;
using DesktopClient.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Linq;



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

        #region Title
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
        #endregion

        #region DiaryItem
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
        #endregion

        #region DiaryItems
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
        #endregion

        #region Currentpage
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
        #endregion

        #region PickDate
        /// <summary>
        /// The <see cref="PickDate" /> property's name.
        /// </summary>
        public const string PickDatePropertyName = "PickDate";

        private DateTime _pickDate = DateTime.Today;

        /// <summary>
        /// Sets and gets the PickDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DateTime PickDate
        {
            get
            {
                return _pickDate;
            }

            set
            {
                if (_pickDate == value)
                {
                    return;
                }

                _pickDate = value;
                RaisePropertyChanged(PickDatePropertyName);
            }
        }
        #endregion

        #region UserInfo
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
        #endregion

        #region Commands
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
                        Messenger.Default.Send<DBModel.domainDiary>(new DBModel.domainDiary(), "ShowDiaryView");

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
        public RelayCommand SaveItemsCommand { get; set; }

        public RelayCommand<IList<DBModel.domainDiary>> EraseItemsCommand { get; set; }
        #endregion

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IDataService dataService)
        {
            _dataService = dataService;
            _dataService.OnGetUserInfo += _dataService_OnGetUserInfo;
            _dataService.OnDiaryItemsInsert += _dataService_OnDiaryItemsInsert;
            //生成文件和发送
            _dataService.OnServerSendDiary += _dataService_OnServerSendDiary;
            _dataService.OnSavedExcelFile += _dataService_OnSavedExcelFile;

            Messenger.Default.Send<bool>(true, "ShowBusy");
            Messenger.Default.Send<string>("获取用户信息...", "SetBusyContent");
            _dataService.GetUserInfo();
            _dataService.OnLoadDiarys += _dataService_OnLoadDiarys;

            DiaryItem = new DBModel.domainDiary();
            DiaryItems = new ObservableCollection<DBModel.domainDiary>();
            SendMailCommand = new RelayCommand(_sendMailExecute, _canSendMailExecute);
            EditUserCommand = new RelayCommand(_EditUserExecute, _canEditUserExecute);
            EraseItemsCommand = new RelayCommand<IList<DBModel.domainDiary>>(_eraseItemsExecute, _canEraseItemsExecute);
            SaveItemsCommand = new RelayCommand(_saveItemsExecute);
            Messenger.Default.Register<DBModel.domainDiary>(this, "ReturnItemContent", ReturnItemContent);
            Messenger.Default.Register<DateTime?>(this, "RetrieveContentByDate", RetrieveContentByDate);
        }

        private void _saveItemsExecute()
        {
            _dataService.InsertDiaryItems(this.DiaryItems);
        }

        /// <summary>
        /// 删除日志条目
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool _canEraseItemsExecute(IList<DBModel.domainDiary> arg)
        {
            //只允许删除未上传的

            return arg.Count > 0;
        }

        private void _eraseItemsExecute(IList<DBModel.domainDiary> obj)
        {
            //只删除未上传的
            foreach (var item in obj)
            {
                this.DiaryItems.Remove(item);
            }
        }

        private void RetrieveContentByDate(DateTime? obj)
        {
            LoadDiaryItemsByUserIdAndDate();
        }

        void _dataService_OnLoadDiarys(object sender, ServiceContract.DiarysEventArgs e)
        {
            this.DiaryItems = new ObservableCollection<DBModel.domainDiary>(e.Items);
            Messenger.Default.Send<bool>(false, "ShowBusy");
        }

        private void ReturnItemContent(DBModel.domainDiary obj)
        {
            this.DiaryItem = obj;
            this.DiaryItem.date = this.PickDate;
            this.DiaryItem.fileId = -1;
            this.DiaryItem.userId = this.UserInfo.id;
            this.DiaryItem.valid = true;

            var d = from n in DiaryItems where n.item == obj.item select n;

            if (DiaryItems.Count(di => di.item.Equals(obj.item)) > 0)
            {
                var el = DiaryItems.First(di => di.item.Trim().Equals(obj.item.Trim()));
                el.dtext = obj.dtext;
                el.increaseTime = obj.increaseTime;
                el.status = obj.increaseTime;
            }
            else
                this.DiaryItems.Add(this.DiaryItem);
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
            return this.DiaryItems.Count > 0;
        }

        private void _sendMailExecute()
        {
            Messenger.Default.Send<bool>(true, "ShowBusy");
            //未保存的先保存

            _dataService.InsertDiaryItems(this.DiaryItems);

            _dataService.SendDiary(this.UserInfo.number, this.PickDate);
        }

        void _dataService_OnSavedExcelFile(object sender, ServerExcelFilenameEventArg e)
        {
            Messenger.Default.Send<string>(string.Format("日志文件 {0} 已保存", e.ExcelFilename), "SetBusyContent");
        }

        void _dataService_OnServerSendDiary(object sender, ServerSendDiaryEventArg e)
        {
            if (e.Success)
            {
                LoadDiaryItemsByUserIdAndDate();
            }
            else
            {

            }
            Messenger.Default.Send<string>(e.Message, "SetBusyContent");
            System.Threading.Thread.Sleep(500);
            Messenger.Default.Send<bool>(false, "ShowBusy");
        }

        void _dataService_OnDiaryItemsInsert(object sender, ServiceContract.DiaryItemsInsertEventArgs e)
        {
            Messenger.Default.Send<int>(e.Percent, "SaveDiaryItemPercent");
            if (e.Percent==100)
            {
                LoadDiaryItemsByUserIdAndDate();
            }
        }

        void _dataService_OnGetUserInfo(object sender, ServiceContract.UserInfoEventArgs e)
        {
            this.UserInfo = e.UserInfo;
            Messenger.Default.Send<string>(this.UserInfo.mailpwd, "SetPasswordDisplay"
            );
            Messenger.Default.Send<bool>(false, "ShowBusy");
            LoadDiaryItemsByUserIdAndDate();
        }

        private void LoadDiaryItemsByUserIdAndDate()
        {
            if (this.UserInfo != null)
            {
                Messenger.Default.Send<bool>(true, "ShowBusy");
                Messenger.Default.Send<string>("获取所选日期的日志...", "SetBusyContent");

                _dataService.LoadDiaryItems(this.UserInfo.id, PickDate);
            }


        }

        ////public override void Cleanup()
        ////{
        ////    // Clean up if needed

        ////    base.Cleanup();
        ////}




    }
}