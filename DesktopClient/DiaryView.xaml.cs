using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Windows;

namespace DesktopClient
{
    /// <summary>
    /// Description for DiaryView.
    /// </summary>
    public partial class DiaryView : Window
    {
        /// <summary>
        /// Initializes a new instance of the DiaryView class.
        /// </summary>
        public DiaryView()
        {
            InitializeComponent();
            Messenger.Default.Register<int>(this, "ShowInsertProgress", ShowInsertProgress);
            this.Unloaded += DiaryView_Unloaded;
        }

        void DiaryView_Unloaded(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Unregister(this);
        }

        private void ShowInsertProgress(int percent)
        {
            this.Dispatcher.Invoke((System.Action)delegate
            {
                this.pbar.Value = percent;
                if (percent != 100)
                    this.pbar.Visibility = System.Windows.Visibility.Visible;
                else
                {
                    this.pbar.Visibility = System.Windows.Visibility.Collapsed;
                    var src = (dataGrid.ItemsSource as ObservableCollection<DBModel.domainDiary>);
                    src.Clear();
                }
            });

        }
    }
}