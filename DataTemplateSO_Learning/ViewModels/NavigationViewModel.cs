using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using DataTemplateSO_Learning.ViewModels;

namespace DataTemplateSO_Learning.ViewModels
{
    class NavigationViewModel : INotifyPropertyChanged
    {

        public ICommand SwitchToMainMenu { get; set; }
        public ICommand SwitchToSettings { get; set; }
        public ICommand SwitchToCalibrate { get; set; }
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
            views.Add(new MainMenuViewModel(switchToEnum.MainMenu));//game
            views.Add(new CalibrateViewModel(switchToEnum.Calibration,connectionManager));//calibr
            views.Add(new SettingsViewModel(switchToEnum.Settings, connectionManager));
            views.Add(new MainMenuViewModel(switchToEnum.MainMenu));//about
            views.Add(new MainMenuViewModel(switchToEnum.MainMenu));//mainmenu
            SwitchToMainMenu = new BaseCommand(OpenMainMenu);
            SwitchToSettings = new BaseCommand(OpenSettings);
            SwitchToCalibrate = new BaseCommand(OpenCalibrate);
            SelectedViewModel = views[0];
        }
        

        private void SwitchTo(switchToEnum switchTo)
        {
            if (selectedViewModel is SettingsViewModel)
            {
                connectionManager.Timer.Tick -= ((SettingsViewModel)views[2]).StationInfoRefresh;
            }
            switch (switchTo)
            {
                
                case switchToEnum.Game:SelectedViewModel = views[0];
                    break;
                case switchToEnum.Calibration: SelectedViewModel = views[1];
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
        MainMenu = 4
    }
}
