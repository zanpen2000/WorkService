using DesktopClient.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Controls;

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
            SaveCommand = new RelayCommand<object>(_saveExecute, _canSaveExecute);

            CancelCommand = new RelayCommand(_cancelExecute);
            Messenger.Default.Register<DBModel.codeUsers>(this, "SetUser", SetUser);

        }

        private bool _canSaveExecute(object arg)
        {
            var pwdbox = arg as PasswordBox;
            var pwd = pwdbox.Password;

            if (string.IsNullOrEmpty(this.User.mail) ||
                string.IsNullOrEmpty(pwd) ||
                string.IsNullOrEmpty(this.User.mailto) ||
                string.IsNullOrEmpty(this.User.name) ||
                string.IsNullOrEmpty(this.User.number)
                 )
            {
                return false;
            }
            return true;
        }

        private void SetUser(DBModel.codeUsers obj)
        {
            this.User = obj;
            Messenger.Default.Send<string>(
                _3rd.Security.Decode(this.User.mailpwd),
                "SetPasswordFromNet");
        }

        private void _cancelExecute()
        {
            Messenger.Default.Send<object>(null, "CloseUserEditView");
        }

        private void _saveExecute(object obj)
        {
            string vmsg = User.Validate();
            if (!string.IsNullOrEmpty(vmsg))
            {
                Messenger.Default.Send<string>(vmsg, "HasInValidData");
                return;
            }

            var pwdbox = obj as PasswordBox;
            User.mailpwd = pwdbox.Password;
            User.mailpwd = _3rd.Security.Encode(User.mailpwd);

            //save and close
            _dataService.OnSavedToDatabase += _dataService_OnSaved;
            _dataService.EditUser(User);

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

        public RelayCommand<object> SaveCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
    }
}