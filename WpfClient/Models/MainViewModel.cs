using AppLayer;
using DBModel;
using ServiceContract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace WpfClient.Models
{
    public class MainViewModel : IDocumentCallback, INotifyPropertyChanged
    {
        #region Propertys

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                OnPropertyChanged("Title");
            }
        }

        private string currentPage;
        public string CurrentPage
        {
            get { return currentPage; }
            set
            {
                currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        private string currentUserName;
        public string CurrentUserName
        {
            get { return currentUserName; }
            set
            {
                currentUserName = value;
                OnPropertyChanged("CurrentUserName");
            }
        }

        private ObservableCollection<viewUserDiarys> userDiarys;

        public ObservableCollection<viewUserDiarys> UserDiarys
        {
            get { return userDiarys; }
            set
            {
                userDiarys = value;
                OnPropertyChanged("UserDiarys");
            }
        }

        #endregion

        public string number { get; set; }

        public EntityEventCommand ProfileCommand { get; set; }

        public MainViewModel()
        {
            this.Title = "工作日志";
            number = AppSettings.Get("Number");
            ProfileCommand = new EntityEventCommand(init);
            currentPage = "1";
        }

        private void getUserInfo(string number)
        {
            InstanceContext context = new InstanceContext(this);
            AppLayer.ServiceCaller.Execute<IDocumentService>(context, svc =>
            {
                svc.GetUserInfo(number);
            });
        }

        private void getDiarys(string userId)
        {
            InstanceContext context = new InstanceContext(this);
            AppLayer.ServiceCaller.Execute<IDocumentService>(context, svc =>
            {
                svc.GetUserDiarys(userId, CurrentPage);
            });
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public void ReturnUserDiarys(IEnumerable<viewUserDiarys> diarys)
        {
            UserDiarys = new ObservableCollection<viewUserDiarys>(diarys);
        }

        public void ReturnUserInfo(codeUsers user)
        {
            this.CurrentUserName = user.name;
            this.Title = string.Format("{0}({1})", this.Title, user.name);
        }

        public void init()
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                getUserInfo(number);
                getDiarys(number);
            });
        }

        public event EventHandler<UserInfo> OnGetUserInfo;
    }
}
