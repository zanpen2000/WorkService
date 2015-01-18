using DesktopClient.Model;
using GalaSoft.MvvmLight;

namespace DesktopClient.ViewModel
{
    /// <summary>
    /// This class contains properties that a View can data bind to.
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ItemEditViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        /// <summary>
        /// Initializes a new instance of the ItemEditViewModel class.
        /// </summary>
        public ItemEditViewModel(IDataService ds)
        {
            this._dataService = ds;
        }
    }
}