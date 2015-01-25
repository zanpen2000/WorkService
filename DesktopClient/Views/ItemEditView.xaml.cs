using GalaSoft.MvvmLight.Messaging;
using System.Windows;
using System.Windows.Media;

namespace DesktopClient
{
    /// <summary>
    /// Description for ItemEditView.
    /// </summary>
    public partial class ItemEditView : Window
    {
        /// <summary>
        /// Initializes a new instance of the ItemEditView class.
        /// </summary>
        public ItemEditView()
        {
            InitializeComponent();
            Messenger.Default.Register<object>(this, "CloseItemEditView", (obj) => { this.Close(); });
            Messenger.Default.Register<bool>(this, "ItemNameExists", ItemNameExists);
        }

        private void ItemNameExists(bool obj)
        {
            Dispatcher.Invoke((System.Action)delegate
            {
                if (obj)
                {
                    _itemname.ToolTip = "项目名称已经存在";
                    _itemname.Background = new SolidColorBrush(Colors.OrangeRed);
                }
                else
                {
                    _itemname.ToolTip = "项目名称可用";
                    _itemname.Background = new SolidColorBrush(Colors.LightGreen);
                }
            });
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Messenger.Default.Send<string>(_itemname.Text, "CheckItemNameExists");
        }
    }
}