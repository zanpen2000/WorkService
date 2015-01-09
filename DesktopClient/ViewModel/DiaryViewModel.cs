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
    public class DiaryViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        public int DiaryId { get; set; }

        /// <summary>
        /// The <see cref="DiaryItem" /> property's name.
        /// </summary>
        public const string DiaryItemPropertyName = "DiaryItem";

        private DBModel.domainDiary _diaryItem = null;

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

        /// <summary>
        /// Initializes a new instance of the DiaryViewModel class.
        /// </summary>
        public DiaryViewModel(IDataService dataService, int diaryId = -1)
        {
            this.DiaryId = diaryId;
            _dataService = dataService;
            if (diaryId != -1)
            {
                _dataService.OnLoadDiary += _dataService_OnLoadDiary;
                _dataService.LoadDiary(diaryId);
            }
        }

        void _dataService_OnLoadDiary(object sender, ServiceContract.DiaryEventArgs e)
        {
            this.DiaryItem = e.Diary;
        }
    }
}