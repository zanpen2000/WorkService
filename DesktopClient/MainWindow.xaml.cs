using System.Windows;
using DesktopClient.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Data;

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
            Messenger.Default.Register<DBModel.domainDiary>(this, "ShowDiaryView", ShowDiaryView);
            Messenger.Default.Register<DBModel.codeUsers>(this, "ShowUserEditView", ShowUserEditView);
            Messenger.Default.Register<bool>(this, "ShowBusy", ShowBusy);
            Messenger.Default.Register<string>(this, "SetBusyContent", SetBusyContent);
            Messenger.Default.Register<int>(this, "SaveDiaryItemPercent", SaveDiaryItemPercent);
            Messenger.Default.Register<string>(this, "SetPasswordDisplay", SetPasswordDisplay);

            this.Unloaded += MainWindow_Unloaded;
        }

        private void SetPasswordDisplay(string obj)
        {
            Dispatcher.Invoke((Action)delegate
            {
                txtPwd.Password = obj;
            });
        }

        private void ShowBusy(bool obj)
        {
            Dispatcher.Invoke((Action)delegate
            {
                this._busy.IsBusy = obj;
            });
        }

        private void SaveDiaryItemPercent(int obj)
        {
            Dispatcher.Invoke((Action)delegate
            {
                this._busy.BusyContent = string.Format("上传服务器 {0}%", obj);

                this._saveProgress.Visibility = obj != 100 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                this._saveProgress.Value = obj;
            });
        }

        private void SetBusyContent(string obj)
        {
            Dispatcher.Invoke((Action)delegate
            {
                this._busy.BusyContent = obj;
            });
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

        private void ShowDiaryView(DBModel.domainDiary diary)
        {
            var vm = new ItemEditView();
            Messenger.Default.Send<DBModel.domainDiary>(diary, "SetDiaryItem");
            vm.ShowDialog();
        }

        private void Calendar_SelectedDatesChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var date = DateTime.Parse(e.AddedItems[0].ToString());
            Messenger.Default.Send<DateTime?>(date, "RetrieveContentByDate");
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Messenger.Default.Send<DBModel.domainDiary>((DBModel.domainDiary)datagrid.CurrentItem, "ShowDiaryView");
        }

        private void datagrid_LoadingRow(object sender, System.Windows.Controls.DataGridRowEventArgs e)
        {
            var drv = e.Row.Item as DBModel.domainDiary;
            
            //判断是否已经保存
            if (drv.fileId > 0)
            {
                var row = datagrid.ItemContainerGenerator.ContainerFromItem(e.Row.Item) as System.Windows.Controls.DataGridRow;
                row.Background = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGray);
            }
            
        }
    }
}