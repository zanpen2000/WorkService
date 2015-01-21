using System.Windows;
using DesktopClient.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace DesktopClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            Messenger.Default.Register<object>(this, "ShowDiaryView", ShowDiaryView);
            Messenger.Default.Register<DBModel.codeUsers>(this, "ShowUserEditView", ShowUserEditView);
            this.Unloaded += MainWindow_Unloaded;
        }

        private void ShowUserEditView(DBModel.codeUsers usr)
        {
            var view = new Views.UserEditView();
            view.Show();
            Messenger.Default.Send<DBModel.codeUsers>(usr, "SetUser");
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }

        private void ShowDiaryView(object obj)
        {
            new ItemEditView().Show();
        }
    }
}