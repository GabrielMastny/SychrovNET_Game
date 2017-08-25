using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows;

namespace DataTemplateSO_Learning
{
    class ConnectionManager : INotifyPropertyChanged
    {
        public ConnectionManager()
        {
            stationInfoCollection = new ObservableCollection<StationInfo>();
            stationInfoCollection.Add(new StationInfo() { Id=0, Name = "Station 1", SshCli = new SshClient("s", "s", "s"), ip = "0.0.0.0", signal = "N/A", status = "Not set" });
            stationInfoCollection.Add(new StationInfo() { Id=1, Name = "Station 2", SshCli = new SshClient("s", "s", "s"), ip = "0.0.0.0", signal = "N/A", status = "Not set" });
            stationInfoCollection.Add(new StationInfo() { Id=2, Name = "Station 3", SshCli = new SshClient("s", "s", "s"), ip = "0.0.0.0", signal = "N/A", status = "Not set" });

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Tick += StationInfoUpdate;
            Timer.Start();
        }

        private void StationInfoUpdate(object sender, EventArgs e)
        {
            for (int i = 0; i < stationInfoCollection.Count; i++)
            {
                if (stationInfoCollection[i].SshCli.IsConnected)
                {
                    stationInfoCollection[i].CurrentAp = GetSsid((no)i);
                    stationInfoCollection[i].signal = GetSignal((no)i);
                    stationInfoCollection[i].status = "Connected";
                }
                else if (stationInfoCollection[i].ip == "0.0.0.0")
                {
                    stationInfoCollection[i].status = "Not set";
                }
                else
                {
                    stationInfoCollection[i].status = "Disconnected";
                }

            }
        }

        public enum no
        {
            first,
            second,
            third
        }

        public DispatcherTimer Timer;
        private ObservableCollection<StationInfo> stationInfoCollection;



        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<ListBoxItem> aPsSsid;

        public ObservableCollection<ListBoxItem> APsSsid
        {
            get { return aPsSsid; }
            set
            {
                aPsSsid = value;
            }
        }
        private ObservableCollection<ListBoxItem> stationsCollection;
        public ObservableCollection<ListBoxItem> StationsCollection
        {
            get { return stationsCollection; }
            set
            {
                stationsCollection = value;
            }
        }

        

        public void SetUpStation(no number, string ip, string login, string pass)
        {
            if (stationInfoCollection[(int)number].SshCli.IsConnected)
            {
                stationInfoCollection[(int)number].SshCli.Disconnect();
            }
            stationInfoCollection[(int)number].ip = ip;
            stationInfoCollection[(int)number].SshCli = new SshClient(ip, login, pass);
            try
            {
                stationInfoCollection[(int)number].SshCli.Connect();
            }
            catch (Exception)
            {

                
            }
                
            
            
        }

        public ObservableCollection<StationInfo> StationInfoCollection
        {
            get { return stationInfoCollection; }
            set { stationInfoCollection = value; OnPropertyChanged("StationInfoCollection");  }
        }

        public ObservableCollection<string> GetStationInfo(no number,StationInfoType type)
        {
            ObservableCollection<string> toReturn = new ObservableCollection<string>();
            for (int i = 0; i < stationInfoCollection.Count; i++)
            {
                if (stationInfoCollection[i].SshCli.IsConnected)
                {
                    if (type == StationInfoType.signal)
                    {
                        toReturn.Add(GetSignal((no)i));
                    }
                    else if(type == StationInfoType.status)
                    {
                        toReturn.Add(stationInfoCollection[i].status);
                    }
                    else
                    {
                        toReturn.Add(stationInfoCollection[i].ip);
                    }
                }
                else
                {
                    toReturn.Add("N/A");
                }
            }
            return toReturn;


        }

        public string GetSignal(no number)
        {
            SshCommand command;
            string[] _results;
                command =  stationInfoCollection[(int)number].SshCli.RunCommand("iwconfig |grep Signal");
                _results = command.Result.Split(' ');
                _results = _results[14].Split('-');
            //test = int.Parse(results[1]);
                return _results[1];
            
        }

        public string GetSsid(no number)
        {
            SshCommand command;
            string[] _results;
            command = stationInfoCollection[(int)number].SshCli.RunCommand("iwconfig ath0 |grep ESSID");
            _results = command.Result.Split('"');
            return _results[1];
        }

        public void SetNewSsid(no number,string ssid)
        {
            SshCommand command;
            command = stationInfoCollection[(int)number].SshCli.RunCommand($"sed -i 's/wireless.1.ssid={GetSsid(number)}/wireless.1.ssid={ssid}/' /tmp/system.cfg ");
            
            command = stationInfoCollection[(int)number].SshCli.RunCommand($"sed -i 's/wpasupplicant.profile.1.network.1.ssid={GetSsid(number)}/wpasupplicant.profile.1.network.1.ssid={ssid}/' /tmp/system.cfg");
            command = stationInfoCollection[(int)number].SshCli.RunCommand("cfgmtd -f /tmp/system.cfg -w");
            command = stationInfoCollection[(int)number].SshCli.RunCommand("/usr/etc/rc.d/rc.softrestart save");
        }


        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }






    }
    public enum StationInfoType
    {
        ip,status,signal
    }

    public class StationInfo
    {
        public int Id;
        public string Name;
        public SshClient SshCli;
        public string ip;
        public string status;
        public string signal;
        public List<SignalQuality> BestTreshWorst;
        public string CurrentAp;

        public override string ToString()
        {
            return Name;
        }
    }

    public class SignalQuality
    {
        public string Ap;
        public string BestValue;
        public string Treshold;
        public string WorstValue;
    }

    
}
