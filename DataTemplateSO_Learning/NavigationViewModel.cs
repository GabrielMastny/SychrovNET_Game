using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace DataTemplateSO_Learning
{
    class NavigationViewModel : INotifyPropertyChanged
    {

        public ICommand SwitchToMainMenu { get; set; }
        public ICommand SwitchToSettings { get; set; }
        private List<ViewModelBase> views;
        private object selectedViewModel;
        public object SelectedViewModel
        {
            get { return selectedViewModel; }
            set { selectedViewModel = value; OnPropertyChanged("SelectedViewModel"); }
        }
        

        public NavigationViewModel()
        {
            views = new List<ViewModelBase>();
            views.Add(new MainMenuViewModel(switchToEnum.MainMenu));
            views.Add(new SettingsViewModel(switchToEnum.Settings));
            SwitchToMainMenu = new BaseCommand(OpenMainMenu);
            SwitchToSettings = new BaseCommand(OpenSettings);
            SelectedViewModel = views[0];
        }
        

        private void SwitchTo(switchToEnum switchTo)
        {
            switch (switchTo)
            {
                case switchToEnum.Game:SelectedViewModel = views[0];
                    break;
                case switchToEnum.Calibration: SelectedViewModel = views[0];
                    break;
                case switchToEnum.Settings: SelectedViewModel = views[1];
                    break;
                case switchToEnum.About: SelectedViewModel = views[0];
                    break;
                case switchToEnum.MainMenu: SelectedViewModel = views[0];
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
