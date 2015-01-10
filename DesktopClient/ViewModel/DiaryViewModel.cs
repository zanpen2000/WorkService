using DesktopClient.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.ObjectModel;
using System.Windows;

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

        private DBModel.domainDiary _diaryItem;

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
        /// The <see cref="DiaryItems" /> property's name.
        /// </summary>
        public const string DiaryItemsPropertyName = "DiaryItems";

        private ObservableCollection<DBModel.domainDiary> _diaryItems;

        /// <summary>
        /// Sets and gets the DiaryItems property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public ObservableCollection<DBModel.domainDiary> DiaryItems
        {
            get
            {
                return _diaryItems;
            }

            set
            {
                if (_diaryItems == value)
                {
                    return;
                }

                _diaryItems = value;
                RaisePropertyChanged(DiaryItemsPropertyName);
            }
        }

        /// <summary>
        /// Initializes a new instance of the DiaryViewModel class.
        /// </summary>
        public DiaryViewModel(IDataService dataService)
        {
            int diaryId = -1;
            _diaryItems = new ObservableCollection<DBModel.domainDiary>();
            this.DiaryId = diaryId;
            _dataService = dataService;
            _dataService.OnLoadDiary += _dataService_OnLoadDiary;
            if (diaryId != -1)
            {
                _dataService.LoadDiary(diaryId);
            }

            _dataService.OnLoadDiarys += _dataService_OnLoadDiarys;
            
            InsertCommand = new RelayCommand(_insertExecute);
            DiaryItem = new DBModel.domainDiary();
        }

        private void _insertExecute()
        {
            DiaryItem.userId = 2;
            DiaryItem.date = DateTime.Now.Date;
            DiaryItem.valid = true;
            DiaryItem.fileId = 0;
            _dataService.InsertDiary(DiaryItem);
           
        }

       


        void _dataService_OnLoadDiarys(object sender, ServiceContract.DiarysEventArgs e)
        {
            DiaryItems = new ObservableCollection<DBModel.domainDiary>(e.Items);
        }

        void _dataService_OnLoadDiary(object sender, ServiceContract.DiaryEventArgs e)
        {
            this.DiaryItem = e.Diary;
        }

        public RelayCommand InsertCommand { get; set; }




    }
}