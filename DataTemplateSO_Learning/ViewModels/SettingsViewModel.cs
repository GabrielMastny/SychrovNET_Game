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
using System.Windows.Threading; 

namespace DataTemplateSO_Learning.ViewModels
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
        private ObservableCollection<Visibility> stationInfoVisibility;
        private ConnectionManager connectManager;
        private struct StationCredentialsStruct
        {
            public string Ip;
            public string Login;
            public string Pass; 
        }
        #endregion
#region First Station Info
        public string StStationIp
        {
            get { return connectManager.StationInfoCollection[0].ip; }
        }
        public string StStationStatus
        {
            get { return connectManager.StationInfoCollection[0].status; }
        }
        public string StStationSignal
        {
            get { return connectManager.StationInfoCollection[0].signal; }
        }
        #endregion
#region Second Station Info
        public string NdStationIp
        {
            get { return connectManager.StationInfoCollection[1].ip; }
        }
        public string NdStationStatus
        {
            get { return connectManager.StationInfoCollection[1].status; }
        }
        public string NdStationSignal
        {
            get { return connectManager.StationInfoCollection[1].signal; }
        }
        #endregion
#region Third Station Info
        public string RdStationIp
        {
            get { return connectManager.StationInfoCollection[2].ip; }
        }
        public string RdStationStatus
        {
            get { return connectManager.StationInfoCollection[2].status; }
        }
        public string RdStationSignal
        {
            get { return connectManager.StationInfoCollection[2].signal; }
        }
        #endregion


        public void StationInfoRefresh(object sender, EventArgs e)
        {
            OnPropertyChanged("StStationIp");
            OnPropertyChanged("StStationStatus");
            OnPropertyChanged("StStationSignal");
            OnPropertyChanged("NdStationIp");
            OnPropertyChanged("NdStationStatus");
            OnPropertyChanged("NdStationSignal");
            OnPropertyChanged("RdStationIp");
            OnPropertyChanged("RdStationStatus");
            OnPropertyChanged("RdStationSignal");
        }
        #region Properties
        public ICommand SetButtonClick { get; set; }
        private ObservableCollection<Visibility> aPSsidVisibility;
        public ObservableCollection<Visibility> APSsidVisibility
        {
            get { return aPSsidVisibility; }
            set { aPSsidVisibility = value; }
        }
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

        public ObservableCollection<Visibility> StationInfoVisibility
        {
            get { return stationInfoVisibility; }
            set { stationInfoVisibility = value; OnPropertyChanged("StationInfoVisibility"); }
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
                        StationInfoVisibility[i] = Visibility.Visible;
                    }
                    else
                    {
                        stationCredentials[i].Visibility = Visibility.Hidden;
                        StationInfoVisibility[i] = Visibility.Hidden;
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
            set
            {
                ApCount = (int)value + 1;
                for (int i = 0; i < stationCredentials.Count; i++)
                {
                    if (i <=ApSliderValue)
                    {
                        APSsidVisibility[i] = Visibility.Visible;
                    }
                    else
                    {
                        APSsidVisibility[i] = Visibility.Hidden;
                    }
                }
            }
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

        


        public SettingsViewModel(switchToEnum type, ConnectionManager connectMan) : base(type)
        {
            stationCredentials = new ObservableCollection<TabItem>();
            stationCredentials.Add(new TabItem() { TabIndex=0, Header="station1", Visibility= Visibility.Visible, Style = Application.Current.FindResource("TabControl") as Style, Content= new StationCredentialsStruct() { Ip = "192.168.1.20",Login="ubnt", Pass = "morava" } });
            stationCredentials.Add(new TabItem() { TabIndex = 1, Header = "station2", Visibility = Visibility.Hidden, Style = Application.Current.FindResource("TabControl") as Style, Content = new StationCredentialsStruct() { Ip = "192.168.1.22",Login="ubnt", Pass = "morava" } });
            stationCredentials.Add(new TabItem() { TabIndex = 2, Header = "station3", Visibility = Visibility.Hidden, Style = Application.Current.FindResource("TabControl") as Style, Content = new StationCredentialsStruct() { Ip = "0.0.0.0",Login="Login", Pass = "Passwd" } });
            stationInfoVisibility = new ObservableCollection<Visibility>();
            stationInfoVisibility.Add(Visibility.Visible);
            stationInfoVisibility.Add(Visibility.Hidden);
            stationInfoVisibility.Add(Visibility.Hidden);
            aPSsidVisibility = new ObservableCollection<Visibility>();
            aPSsidVisibility.Add(Visibility.Visible);
            aPSsidVisibility.Add(Visibility.Hidden);
            aPSsidVisibility.Add(Visibility.Hidden);
            connectManager = connectMan;
            SelectedTabIndex = 0;
            SetButtonClick = new BaseCommand(SetNewStationCredentials);


        }

        private void SetNewStationCredentials(object obj)
        {
            StationCredentials[selectedTabIndex].Content = new StationCredentialsStruct() { Ip = currentIp, Login = currentLogin, Pass = currentPasswd }; OnPropertyChanged("StationCredentials");
            connectManager.SetUpStation((ConnectionManager.no)selectedTabIndex, currentIp, currentLogin, currentPasswd);
            
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
