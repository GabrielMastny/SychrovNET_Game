using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataTemplateSO_Learning
{
    class SettingsViewModel : ViewModelBase, INotifyPropertyChanged
    {

#region Fields

        private int stationCount = 1;
        private int apCount = 1;
        private ObservableCollection<TabItem> stationCredentials;
        private int selectedTabIndex = 0;
        private string currentIp;
        private string currentLogin;
        private string currentPasswd;
        private SSH.SSH sshIns;

        private struct StationCredentialsStruct
        {
            public string Ip;
            public string Login;
            public string Pass; 
        }
        #endregion

        #region Properties
        public ICommand SwitchToMainMenu { get; set; }
        public string CurrentIp
        {
            get { return currentIp; }
            set { currentIp = value;OnPropertyChanged("CurrentIp"); }
        }

        public string CurrentLogin
        {
            get { return currentLogin; }
            set { currentLogin = value;  OnPropertyChanged("CurrentLogin"); }
        }

        public string CurrentPasswd
        {
            get { return currentPasswd; }
            set { currentPasswd = value; OnPropertyChanged("CurrentPasswd"); }
        }

        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set {
                selectedTabIndex = value;
                CurrentIp = ((StationCredentialsStruct)StationCredentials[value].Content).Ip;
                CurrentLogin = ((StationCredentialsStruct)StationCredentials[value].Content).Login;
                CurrentPasswd = ((StationCredentialsStruct)StationCredentials[value].Content).Pass;
                OnPropertyChanged("SelectedTabIndex");
                }
        }

        public double StationSliderValue
        {
            get { return ((stationCount-1)); }
            set
            {
                StationCount = (int)value + 1;
                for (int i = 0; i < stationCredentials.Count; i++)
                {
                    if (i <= StationSliderValue)
                    {
                        stationCredentials[i].Visibility = Visibility.Visible;
                    }
                    else
                    {
                        stationCredentials[i].Visibility = Visibility.Hidden;
                        if (selectedTabIndex == i)
                        {
                            SelectedTabIndex--;
                        }
                    }
                }
            }
        }

        public double ApSliderValue
        {
            get { return apCount - 1; }
            set { ApCount = (int)value + 1; }
        }

        public int StationCount
        {
            get { return stationCount; }
            set { stationCount = value; OnPropertyChanged("StationCount"); }
        }

        public int ApCount
        {
            get { return apCount; }
            set { apCount = value; OnPropertyChanged("ApCount"); }
        }

        public ObservableCollection<TabItem> StationCredentials
        {
            get
            {
                return stationCredentials;
            }
            set
            {
                stationCredentials = value;
                OnPropertyChanged("StationCredentials");
            }
        }
        #endregion

        


        public SettingsViewModel(switchToEnum type) : base(type)
        {
            stationCredentials = new ObservableCollection<TabItem>();
            stationCredentials.Add(new TabItem() { TabIndex=0, Header="station1", Visibility= Visibility.Visible, Style = Application.Current.FindResource("TabControl") as Style, Content= new StationCredentialsStruct() { Ip = "10.167.1.2",Login="admin", Pass = "morava" } });
            stationCredentials.Add(new TabItem() { TabIndex = 1, Header = "station2", Visibility = Visibility.Hidden, Style = Application.Current.FindResource("TabControl") as Style, Content = new StationCredentialsStruct() { Ip = "dd",Login="dD", Pass = "Dd" } });
            stationCredentials.Add(new TabItem() { TabIndex = 2, Header = "station3", Visibility = Visibility.Hidden, Style = Application.Current.FindResource("TabControl") as Style, Content = new StationCredentialsStruct() { Ip = "ff",Login="fF", Pass = "Ff" } });
            SelectedTabIndex = 0;
            SwitchToMainMenu = new BaseCommand(OpenMainMenu);

        }

        private void OpenMainMenu(object obj)
        {
            sshIns = new SSH.SSH(currentIp, CurrentLogin, currentPasswd);
            sshIns.Connect();
            MessageBox.Show(sshIns.GetSignal());
            sshIns.DisConnect();
        }




        public event PropertyChangedEventHandler PropertyChanged;

        

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
