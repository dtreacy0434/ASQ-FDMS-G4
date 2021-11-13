using System.ComponentModel;

/*
* FILE : ViewModelBase.cs
* PROJECT : SENG3020 - Term Project Milestone 2
* PROGRAMMER : Faith Madore
* DESCRIPTION : Implementation of INotifyPropertyChanged for handling of View Switching Events
*/

namespace AircraftTelemetry
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}