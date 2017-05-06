using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LeanCloud.Realtime.Test.Integration.WPFNetFx45.Controller;
using LeanCloud.Realtime.Test.Integration.WPFNetFx45.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeanCloud.Realtime.Test.Integration.WPFNetFx45.ViewModel
{
    public class TeamViewModel : ViewModelBase
    {
        public TeamViewModel()
        {
            DoSelectTeam = new RelayCommand(() => DoTeamSelectExecute(), () => true);
        }
        public ICommand DoSelectTeam { get; private set; }
        private async void DoTeamSelectExecute()
        {
            await LoadAllUsersInTeamAsync();
            this.DoSelected = true;
        }
        private ObservableCollection<Team> _teams;
        public ObservableCollection<Team> Teams
        {
            get
            {
                return _teams;
            }
            private set
            {
                _teams = value;
                RaisePropertyChanged("Teams");
            }
        }
        private bool _logIned;
        public bool LogIned
        {
            get
            {
                return _logIned;
            }
            private set
            {
                _logIned = value;
                RaisePropertyChanged("LogIned");
            }
        }
        private bool _doSelected;
        public bool DoSelected
        {
            get
            {
                return _doSelected;
            }
            private set
            {
                _doSelected = value;
                RaisePropertyChanged("DoSelected");
            }
        }
        private Team _selectedTeam;
        public Team SelectdTeam
        {
            get
            {
                return _selectedTeam;
            }
            set
            {
                _selectedTeam = value;
                RaisePropertyChanged("SelectdTeam");
            }
        }
        private ObservableCollection<AVUser> _usrsInTeam;
        public ObservableCollection<AVUser> UsersInTeam
        {
            get
            {
                return _usrsInTeam;
            }
            set
            {
                _usrsInTeam = value;
                RaisePropertyChanged("UsersInTeam");
            }
        }
        public TeamController teamContoller = new TeamController();

        public async Task InitByUserAsync(AVUser user)
        {
            this.LogIned = user.IsAuthenticated;
            var teams = await teamContoller.LoadAllTeamsByUser(user: user);
            if (Teams == null) Teams = new ObservableCollection<Team>();
            teams.ToList().ForEach(x =>
            {
                Teams.Add(x);
            });
        }
        public async Task LoadAllUsersInTeamAsync()
        {
            if (UsersInTeam == null) UsersInTeam = new ObservableCollection<AVUser>();
            var users = await teamContoller.LoadAllUsersInTeam(team: SelectdTeam);
            users.ToList().ForEach(x =>
            {
                UsersInTeam.Add(x);
            });
        }
    }
}
