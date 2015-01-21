using DesktopClient.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace DesktopClient.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class UserEditViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the UserEditViewModel class.
        /// </summary>
        public UserEditViewModel(IDataService dataService)
        {
            _dataService = dataService;
            SaveCommand = new RelayCommand(_saveExecute);
            CancelCommand = new RelayCommand(_cancelExecute);
            Messenger.Default.Register<DBModel.codeUsers>(this, "SetUser", SetUser);
            
        }

        private void SetUser(DBModel.codeUsers obj)
        {
            this.User = obj;
        }

        private void _cancelExecute()
        {
            Messenger.Default.Send<object>(null, "Close");
        }

        private void _saveExecute()
        {
            string vmsg = User.Validate();
            if (!string.IsNullOrEmpty(vmsg))
            {
                Messenger.Default.Send<string>(vmsg, "HasInValidData");
                return;
            }

            //save and close
            _dataService.OnSaved += _dataService_OnSaved;
            _dataService.InsertUser(User);

        }

        void _dataService_OnSaved(object sender, ServiceContract.RowAffectedEventArgs e)
        {
            Messenger.Default.Send<int>(e.RowAffected, "SaveDone");
        }

        /// <summary>
        /// The <see cref="User" /> property's name.
        /// </summary>
        public const string UserPropertyName = "User";

        private DBModel.codeUsers _user = new DBModel.codeUsers();

        /// <summary>
        /// Sets and gets the User property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public DBModel.codeUsers User
        {
            get
            {
                return _user;
            }
            set
            {
                Set(UserPropertyName, ref _user, value);
            }
        }

        public RelayCommand SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
    }
}