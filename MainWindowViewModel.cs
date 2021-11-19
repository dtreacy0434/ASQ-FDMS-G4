using ASQ_AircraftTelemetry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

/*
* FILE : MainWindowViewModel.cs
* PROJECT : SENG3020 - Term Project Milestone 2
* PROGRAMMER : Faith Madore
* DESCRIPTION : ViewModel for MainWindow.xaml - Implements commands used for switching Views
*/

namespace AircraftTelemetry
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ICommand _gotoRealTimeCommand;
        private ICommand _gotoSearchCommand;
        private object _currentView;
        private object _realTimeView;
        private object _searchView;

        public MainWindowViewModel()
        {
            _realTimeView = new RealTimeView();
            _searchView = new SearchView();

            CurrentView = _realTimeView;
        }

        public object GotoRealTimeCommand => _gotoRealTimeCommand ??= new RelayCommand(x => { GotoRealTime(); });

        public ICommand GotoSearchCommand => _gotoSearchCommand ??= (_gotoSearchCommand = new RelayCommand(x => { GotoSearch(); }));

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        /*
        * FUNCTION : GotoRealTime()
        * DESCRIPTION : Change View to RealTimeView
        * PARAMETERS : N/A
        * RETURNS : N/A
        */
        private void GotoRealTime()
        {
            CurrentView = _realTimeView;
        }

        /*
         * FUNCTION : GotoSearch()
         * DESCRIPTION : Change View to SearchView
         * PARAMETERS : N/A
         * RETURNS : N/A
         */
        private void GotoSearch()
        {
            CurrentView = _searchView;
        }
    }
}