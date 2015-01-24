using GalaSoft.MvvmLight.Messaging;
using System.Windows;

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
            Messenger.Default.Register<object>(this, "CloseWindow", (obj) => { this.Close(); });
        }
    }
}