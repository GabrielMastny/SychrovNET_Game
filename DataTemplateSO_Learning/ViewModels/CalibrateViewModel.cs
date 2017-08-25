using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataTemplateSO_Learning.ViewModels
{
    class CalibrateViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public CalibrateViewModel(switchToEnum tp, ConnectionManager connectManager) : base(tp)
        {
            connectionManager = connectManager;
            StartCalibrating = new BaseCommand(StartCalibratingCommand);
            //ApIndex = getApIndex();
        }

        private double offSet = 1;

        public double OffSet
        {
            get { return offSet; }
            set { offSet = value; OnPropertyChanged("OffSet"); }
        }

        private bool isCalibrationOff = true;
        public bool IsCalibrationOff
        {
            get { return isCalibrationOff; }
            set { isCalibrationOff = value; OnPropertyChanged("IsCalibrationOff"); }
        }
        private string calibrateButtonText = "Calibrate";
        public string CalibrateButtonText
        {
            get { return calibrateButtonText; }
            set { calibrateButtonText = value;OnPropertyChanged("CalibrateButtonText"); }
        }

        private void StartCalibratingCommand(object obj)
        {

            
            

            if (isCalibrationOff)
            {
                CurrentSignal = connectionManager.GetSignal((ConnectionManager.no)stationIndex);
                if (((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst == null)
                {
                    ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst = Enumerable.Repeat<SignalQuality>(null, ApList.Count).ToList<SignalQuality>();
                }
                for (int i = 0; i < StationsCollection.Count; i++)
                {

                    if (((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[i] == null)
                    {
                        string stationSignal = ((StationInfo)StationsCollection[i].Content).signal;
                        ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex] = (new SignalQuality() { Ap = ApList[apIndex].ToString(), BestValue = stationSignal , WorstValue = stationSignal, Treshold = "N/A" });
                    }
                }
                BestValue = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[ApIndex].BestValue;
                Treshold = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[ApIndex].Treshold;
                WorstValue = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[ApIndex].WorstValue;
                connectionManager.Timer.Tick += Calibration;
                CalibrateButtonText = "Stop";

            }
            else
            {
                connectionManager.Timer.Tick -= Calibration;
                CalibrateButtonText = "Calibrate";
                OffSet = 1;
            }
            IsCalibrationOff = !isCalibrationOff;

        }

        private double CalculateOffset(string signal, string offsetTreshold)
        {
            double toReturn = 0;
            int chunks = 0;
            int actual = 0;
            int.TryParse(signal, out actual);
            int worst = 0;
            int.TryParse(WorstValue, out worst);
            int best = 0;
            int.TryParse(offsetTreshold, out best);
            if (worst != 0 && best != 0)
            {
                chunks = worst - best;
                int actualSignal = worst - actual;

                toReturn = 1 - ((double)(actual - best) / (double)(worst - best));
                
            }
            return toReturn;

        }

        private void Calibration(object sender, EventArgs e)
        {
            if (((StationInfo)StationsCollection[stationIndex].Content).CurrentAp != ((string)ApList[apIndex].Content))
            {
                changeSsId();
            }
            //ApIndex = getApIndex();
            int index = ((StationInfo)StationsCollection[stationIndex].Content).Id;
            CurrentSignal = connectionManager.GetStationInfo((ConnectionManager.no)index, StationInfoType.signal)[index];
            
            
            if (int.Parse(CurrentSignal) < int.Parse(((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].BestValue))
            {
                BestValue = CurrentSignal;
                ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].BestValue = CurrentSignal;
            }
            if (int.Parse(CurrentSignal) > int.Parse(((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].WorstValue))
            {
                WorstValue = CurrentSignal;
                ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].WorstValue = CurrentSignal;
            }
            int i = ((StationInfo)StationsCollection[stationIndex].Content).Id;
            if (connectionManager.StationInfoCollection[i].SshCli.IsConnected)
            {
                connectionManager.StationInfoCollection[i].CurrentAp = connectionManager.GetSsid((ConnectionManager.no)i);
                connectionManager.StationInfoCollection[i].signal = connectionManager.GetSignal((ConnectionManager.no)i);
                connectionManager.StationInfoCollection[i].status = "Connected";
            }
            else if (connectionManager.StationInfoCollection[i].ip == "0.0.0.0")
            {
                connectionManager.StationInfoCollection[i].status = "Not set";
            }
            else
            {
                connectionManager.StationInfoCollection[i].status = "Disconnected";
            }

            
        }

        public ICommand StartCalibrating { get; set; }
        private ConnectionManager connectionManager;
        private string bestValue = "N/A";

        public string BestValue
        {
            get { return bestValue; }
            set{ bestValue = value; OnPropertyChanged("BestValue"); }
        }

        private string treshold = "N/A";

        

        public string Treshold
        {
            get { return treshold; }
            set { treshold = value; OnPropertyChanged("Treshold"); }
        }

        private string worstvalue = "N/A";

        public string WorstValue
        {
            get { return worstvalue; }
            set { worstvalue = value; OnPropertyChanged("WorstValue"); }
        }
        private int stationIndex = 0;
        public int StationIndex
        {
            get { return stationIndex; }
            set
            {
                stationIndex = value;
                if (((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst != null)
                {
                    BestValue = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].BestValue;
                    WorstValue = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].WorstValue;
                    Treshold = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].Treshold;
                }
                else
                {
                    BestValue = "N/A";
                    WorstValue = "N/A";
                    Treshold = "N/A";
                }
                OnPropertyChanged("StationIndex");
            }
        }

        private int apIndex;
        public int ApIndex
        {
            get { return apIndex; }
            set
            {
                apIndex = value;
                if ( ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst != null)
                {
                    if (((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex] != null)
                    {
                        BestValue = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].BestValue;
                        WorstValue = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].WorstValue;
                        Treshold = ((StationInfo)StationsCollection[stationIndex].Content).BestTreshWorst[apIndex].Treshold;
                    }
                    else
                    {
                        BestValue = "N/A";
                        WorstValue = "N/A";
                        Treshold = "N/A";
                    }
                }
                
                
                OnPropertyChanged("ApIndex");
            }
        }

        private void changeSsId()
        {
            int id = ((StationInfo)StationsCollection[stationIndex].Content).Id;
            connectionManager.SetNewSsid(((ConnectionManager.no)id), ApList[apIndex].Content.ToString());
        }

        public int getApIndex()
        {
            int toReturn = 0;
            for (int i = 0; i < connectionManager.APsSsid.Count; i++)
            {
                if ((string)connectionManager.APsSsid[i].Content == ((StationInfo)connectionManager.StationsCollection[stationIndex].Content).CurrentAp)
                {
                    toReturn = i;
                }
            }
            return toReturn;
        }

        private string currentSignal = "N/A";

        public string CurrentSignal
        {
            get { return currentSignal; }
            set {
                currentSignal = value;
                if (treshold == "N/A")
                {
                    OffSet = CalculateOffset(currentSignal, bestValue);
                }
                else
                {
                    OffSet = CalculateOffset(currentSignal, treshold);
                }
                OnPropertyChanged("CurrentSignal");
            }
        }

        public ObservableCollection<ListBoxItem> ApList
        {
            get { return connectionManager.APsSsid; }
        }

        public ObservableCollection<ListBoxItem> StationsCollection
        {
            get { return connectionManager.StationsCollection; }
        }

#region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

#endregion
    }
}
