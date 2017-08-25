using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DataTemplateSO_Learning.ViewModels;
using DataTemplateSO_Learning.Views;

namespace DataTemplateSO_Learning.ViewModels
{
    class NavigationViewModel : INotifyPropertyChanged
    {

        public ICommand SwitchToMainMenu { get; set; }
        public ICommand SwitchToSettings { get; set; }
        public ICommand SwitchToCalibrate { get; set; }
        public ICommand SwitchToGame { get; set; }
        public ICommand SwitchToGameSetUp { get; set; }
        public ICommand SwitchToOneP { get; set; }
        public ICommand SwitchToTwoP { get; set; }
        public ICommand SwitchToThreeP { get; set; }
        private List<ViewModelBase> views;
        private ConnectionManager connectionManager;
        private object selectedViewModel;
        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }
        

        public NavigationViewModel()
        {
            connectionManager = new ConnectionManager();
            views = new List<ViewModelBase>();
            views.Add(new GameViewModel(switchToEnum.Game));//game
            views.Add(new CalibrateViewModel(switchToEnum.Calibration,connectionManager));//calibr
            views.Add(new SettingsViewModel(switchToEnum.Settings, connectionManager));
            views.Add(new MainMenuViewModel(switchToEnum.MainMenu));//about
            views.Add(new MainMenuViewModel(switchToEnum.MainMenu));//mainmenu
            views.Add(new GameSetUpViewModel(switchToEnum.GameSetUp,connectionManager));
            SwitchToMainMenu = new BaseCommand(OpenMainMenu);
            SwitchToSettings = new BaseCommand(OpenSettings);
            SwitchToCalibrate = new BaseCommand(OpenCalibrate);
            SwitchToGame = new BaseCommand(OpenGame);
            SwitchToGameSetUp = new BaseCommand(OpenGameSetUp);
            SwitchToOneP = new BaseCommand(OpenOneP);
            SwitchToTwoP = new BaseCommand(OpenThreeP);
            SelectedViewModel = views[4];
        }

        private void OpenOneP(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenThreeP(object obj)
        {
            throw new NotImplementedException();
        }

        private void OpenGameSetUp(object obj)
        {
            SwitchTo(switchToEnum.GameSetUp);
        }

        private void OpenGame(object obj)
        {
            
                SwitchTo(switchToEnum.Game);
            
            
        }

        private void SwitchTo(switchToEnum switchTo)
        {
            
                if (selectedViewModel is SettingsViewModel)
                {
                ((SettingsViewModel)views[2]).GenerateAPList();
                ((SettingsViewModel)views[2]).CreateStationCollection();
                connectionManager.Timer.Tick -= ((SettingsViewModel)views[2]).StationInfoRefresh;
                    
                    
                }
            
            switch (switchTo)
            {
                
                case switchToEnum.Game:SelectedViewModel = views[0];
                    break;
                case switchToEnum.Calibration:
                    {
                        SelectedViewModel = views[1];
                        
                        ((CalibrateViewModel)views[1]).ApIndex = ((CalibrateViewModel)SelectedViewModel).getApIndex();
                        
                        
                    }
                    break;
                case switchToEnum.Settings:
                    {
                        connectionManager.Timer.Tick += ((SettingsViewModel)views[2]).StationInfoRefresh;
                        SelectedViewModel = views[2];
                    }
                    break;
                case switchToEnum.About: SelectedViewModel = views[3];
                    break;
                case switchToEnum.MainMenu: SelectedViewModel = views[4];
                    break;
                case switchToEnum.GameSetUp:
                    {
                        SelectedViewModel = views[5];
                        ((GameSetUpViewModel)SelectedViewModel).Tabs = ((SettingsViewModel)views[2]).StationCredentials;
                        ((GameSetUpViewModel)SelectedViewModel).UpdateSetUp();
                        
                    }
                    break;
                default:
                    break;
            }
        }

        private void OpenMainMenu(object obj)
        {
            SwitchTo(switchToEnum.MainMenu);
            
        }
        private void OpenSettings(object obj)
        {
            SwitchTo(switchToEnum.Settings);
            
        }

        private void OpenCalibrate(object obj)
        {
            SwitchTo(switchToEnum.Calibration);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }

    public class BaseCommand : ICommand
    {
        private Predicate<object> _canExecute;
        private Action<object> _method;
        public event EventHandler CanExecuteChanged;

        public BaseCommand(Action<object> method)
            : this(method, null)
        {
        }

        public BaseCommand(Action<object> method, Predicate<object> canExecute)
        {
            _method = method;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
            {
                return true;
            }

            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _method.Invoke(parameter);
        }
    }

    public enum switchToEnum
    {
        Game = 0,
        Calibration = 1,
        Settings = 2,
        About = 3,
        MainMenu = 4,
        GameSetUp =5,
        OneP = 6,
        TwoP = 7,
        ThreeP = 8
    }
}
