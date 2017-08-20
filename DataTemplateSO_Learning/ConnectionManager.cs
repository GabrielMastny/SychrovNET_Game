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

namespace DataTemplateSO_Learning
{
    class ConnectionManager : INotifyPropertyChanged
    {
        public ConnectionManager()
        {
            sshClients = new List<SshClient>();
            sshClients.Add(new SshClient("s", "s", "s"));
            sshClients.Add(new SshClient("s", "s", "s"));
            sshClients.Add(new SshClient("s", "s", "s"));
            stationInfoCollection = new ObservableCollection<StationInfo>();
            stationInfoCollection.Add(new StationInfo() { ip = "0.0.0.0", signal = "N/A", status = "Not set" });
            stationInfoCollection.Add(new StationInfo() { ip = "0.0.0.0", signal = "N/A", status = "Not set" });
            stationInfoCollection.Add(new StationInfo() { ip = "0.0.0.0", signal = "N/A", status = "Not set" });

            Timer = new DispatcherTimer();
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            Timer.Tick += StationInfoUpdate;
            Timer.Start();
        }

        private void StationInfoUpdate(object sender, EventArgs e)
        {
            for (int i = 0; i < sshClients.Count; i++)
            {
                if (sshClients[i].IsConnected)
                {
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
        private List<SshClient> sshClients;
        private ObservableCollection<StationInfo> stationInfoCollection;

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetUpStation(no number, string ip, string login, string pass)
        {
            if (sshClients[(int)number].IsConnected)
            {
                sshClients[(int)number].Disconnect();
            }
            stationInfoCollection[(int)number].ip = ip;
            sshClients[(int)number] = new SshClient(ip, login, pass);
            sshClients[(int)number].Connect();
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
                if (sshClients[i].IsConnected)
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

        private string GetSignal(no number)
        {
            SshCommand command;
            string[] _results;
                command =  sshClients[(int)number].RunCommand("iwconfig |grep Signal");
                _results = command.Result.Split(' ');
                _results = _results[14].Split('-');
                //test = int.Parse(results[1]);
                return _results[1];
            
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
        public string ip;
        public string status;
        public string signal;
    }

    
}
