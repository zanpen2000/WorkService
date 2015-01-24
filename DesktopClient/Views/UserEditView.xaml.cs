using GalaSoft.MvvmLight.Messaging;
using System.Windows;

namespace DesktopClient.Views
{
    /// <summary>
    /// Description for UserEditView.
    /// </summary>
    public partial class UserEditView : Window
    {
        /// <summary>
        /// Initializes a new instance of the UserEditView class.
        /// </summary>
        public UserEditView()
        {
            InitializeComponent();
            Messenger.Default.Register<object>(this, "Close", CloseWindow);
            Messenger.Default.Register<string>(this, "HasInValidData", HasInValidData);
            Messenger.Default.Register<int>(this, "SaveDone", SaveDone);
            Messenger.Default.Register<string>(this, "SetPasswordFromNet", SetPasswordFromNet);
            this.Unloaded += UserEditView_Unloaded;
        }

        private void SetPasswordFromNet(string obj)
        {
            txtPwd.Password = obj;
        }

        private void SaveDone(int obj)
        {
            this.Dispatcher.Invoke((System.Action)delegate
            {
                MessageBox.Show(string.Format("影响的行数：{0}", obj));
                if (obj.Equals(1))
                {
                    this.Close();
                }

            }, null);
        }



        private void HasInValidData(string obj)
        {
            this.Dispatcher.Invoke((System.Action)delegate
            {
                MessageBox.Show(obj);

            }, null);


        }

        void UserEditView_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }

        private void CloseWindow(object obj)
        {
            this.Dispatcher.Invoke((System.Action)delegate
            {
                this.Close();

            }, null);


        }
    }
}