﻿using DesktopClient.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;

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
        /// Initializes a new instance of the ItemEditViewModel class.
        /// </summary>
        public ItemEditViewModel(IDataService ds)
        {
            this._dataService = ds;

            CancelCommand = new RelayCommand(() =>
            {
                MessengerInstance.Send<object>(null, "CloseItemEditView");
            });
            _diaryItem = new DBModel.domainDiary();
            SaveCommand = new RelayCommand(_saveExecute, _canSaveExecute);
            Messenger.Default.Register<DBModel.domainDiary>(this, "SetDiaryItem", SetDiaryItem);
            Messenger.Default.Register<string>(this, "CheckItemNameExists", CheckItemNameExists);
            _dataService.OnItemNameExists += _dataService_OnItemNameExists;
        }

        void _dataService_OnItemNameExists(object sender, ItemNameExistsEventArg e)
        {
            this.ItemNameExists = e.Exists;
            Messenger.Default.Send<bool>(e.Exists, "ItemNameExists");
        }

        public bool ItemNameExists { get; set; }

        private void CheckItemNameExists(string itemname)
        {
            if (string.IsNullOrEmpty(itemname))
            {
                Messenger.Default.Send<bool>(false, "ItemNameExists");
            }
            else
            {
                _dataService.CheckItemNameExists(itemname);
            }
        }

        private void SetDiaryItem(DBModel.domainDiary item)
        {
            this.DiaryItem = item;
        }

        private bool _canSaveExecute()
        {
            
            return DiaryItem!=null && !string.IsNullOrEmpty(DiaryItem.item) && !string.IsNullOrEmpty(DiaryItem.dtext);
        }

        private void _saveExecute()
        {
            Messenger.Default.Send<DBModel.domainDiary>(this.DiaryItem, "ReturnItemContent");
            MessengerInstance.Send<object>(null, "CloseItemEditView");

        }

        public RelayCommand CancelCommand { get; set; }
        public RelayCommand SaveCommand { get; set; }
    }
}