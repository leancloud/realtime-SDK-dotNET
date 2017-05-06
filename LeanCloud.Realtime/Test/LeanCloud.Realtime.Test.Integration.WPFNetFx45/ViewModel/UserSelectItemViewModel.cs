using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel
{
    public class UserSelectItemViewModel : ViewModelBase
    {
        public bool IsSelected { get; set; }
        public UserInfoViewModel UserInfo { get; set; }
    }

    public class UserSelectViewModel : ViewModelBase
    {
        public UserSelectViewModel(IEnumerable<AVUser> _avUsers = null, IEnumerable<UserInfoViewModel> _userInfo = null)
        {
            SelectedUsers = new ObservableCollection<UserSelectItemViewModel>();
            if (_avUsers != null)
            {
                _avUsers.ToList().ForEach(x =>
                {
                    SelectedUsers.Add(new UserSelectItemViewModel()
                    {
                        IsSelected = false,
                        UserInfo = new UserInfoViewModel(x)
                    });
                });
            }
            if (_userInfo != null)
            {
                _userInfo.ToList().ForEach(x =>
                {
                    SelectedUsers.Add(new UserSelectItemViewModel()
                    {
                        IsSelected = false,
                        UserInfo = x
                    });
                });
            }
        }
        private ObservableCollection<UserSelectItemViewModel> _selectedUsers;
        public ObservableCollection<UserSelectItemViewModel> SelectedUsers
        {
            get
            {
                return _selectedUsers;
            }
            set
            {
                _selectedUsers = value;
                RaisePropertyChanged("SelectedUsers");
            }
        }
        public void Reset()
        {
            SelectedUsers.ToList().ForEach(x => x.IsSelected = false);
        }
    }
}
