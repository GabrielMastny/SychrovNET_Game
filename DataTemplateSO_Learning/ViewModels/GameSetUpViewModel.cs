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

namespace DataTemplateSO_Learning.ViewModels
{
    class GameSetUpViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private ConnectionManager connectMan;
        public GameSetUpViewModel(switchToEnum tp, ConnectionManager connectManager) : base(tp)
        {
            connectMan = connectManager;
            
            APSsidVisibility = new List<Visibility>(3);
            aPSsidVisibility.Add(new Visibility());
            aPSsidVisibility.Add(new Visibility());
            aPSsidVisibility.Add(new Visibility());
        }


        private ObservableCollection<ListBoxItem> stationCredentials;
        public ObservableCollection<ListBoxItem> StationCredentials
        {
            get { return stationCredentials; }
            set { stationCredentials = value; OnPropertyChanged("StationCredentials"); }

        }

        private ObservableCollection<ListBoxItem> apCredentials;
        public ObservableCollection<ListBoxItem> ApCredentials
        {
            get { return apCredentials; }
            set { apCredentials = value; OnPropertyChanged("ApCredentials"); }
        }
        
        private List<Visibility> aPSsidVisibility;
        public List<Visibility> APSsidVisibility
        {
            get { return aPSsidVisibility; }
            set { aPSsidVisibility = value; OnPropertyChanged("APSsidVisibility"); }

        }


        private int maxSlider;
        public int MaxSlider
        {
            get { return maxSlider; }
            set { maxSlider = value; OnPropertyChanged("MaxSlider"); }
        }
        public void UpdateSetUp()
        {
            StationCredentials = connectMan.StationsCollection;
            ApCredentials = connectMan.APsSsid;
            MaxSlider = connectMan.StationsCollection.Count-1;
            setActivePlayers();
            ApIndex= getApIndex();
        }
        private void setActivePlayers()
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < int.Parse(playersCount))
                {
                    Tabs[i].Visibility = Visibility.Visible;
                }
                else
                {
                    Tabs[i].Visibility = Visibility.Hidden;
                }
            }
        }

        public int getApIndex()
        {
            int toReturn = 0;
            for (int i = 0; i < connectMan.APsSsid.Count; i++)
            {
                if ((string)connectMan.APsSsid[i].Content == ((StationInfo)connectMan.StationsCollection[stationIndex].Content).CurrentAp)
                {
                    toReturn = i;
                }
            }
            return toReturn;
        }

        private ObservableCollection<TabItem> tabs;

        public ObservableCollection<TabItem> Tabs
        {
            get { return tabs; }
            set { tabs = value; OnPropertyChanged("Tabs"); }
        }
           
        public double PlayersSliderValue
        {
            get { return int.Parse(playersCount) - 1; }
            set
            {
                PlayersCount = ((int)value + 1).ToString();
                //if (stationCredentials == null)
                //{
                //    StationCredentials = connectMan.StationsCollection;
                //}
                //for (int i = 0; i < stationCredentials.Count; i++)
                //{
                //    if (i <= PlayersSliderValue)
                //    {
                //        APSsidVisibility[i] = Visibility.Visible;
                //    }
                //    else
                //    {
                //        APSsidVisibility[i] = Visibility.Hidden;
                //    }
                //}
            }
        }

        private int stationIndex = 0;
        public int Staionindex
        {
            get { return stationIndex; }
            set
            {
                stationIndex = value;
                OnPropertyChanged("Staionindex");
                ApIndex = getApIndex();
            }
        }

        private int apIndex;
        public int ApIndex
        {
            get { return apIndex; }
            set
            {
                apIndex = value;
                OnPropertyChanged("ApIndex");
            }
        }
        private string playersCount = "1";
        public string PlayersCount
        {
            get { return playersCount; }
            set
            {
                playersCount = value;
                setActivePlayers();
                OnPropertyChanged("PlayersCount");
            }
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
