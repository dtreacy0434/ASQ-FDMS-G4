using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

/*
* FILE : RealTimeView.xaml.cs
* PROJECT : SENG3020 - Term Project Milestone 2
* PROGRAMMER : Faith Madore
* DESCRIPTION : .cs file for RealTimeView - Loading the window with information, button click event handlers
*/

namespace AircraftTelemetry
{
    public partial class RealTimeView : UserControl
    {
        /*
         * FUNCTION : Window_Loaded()
         * DESCRIPTION : Retrieves aircraft data to consistently update display
         * Starts a DispatcherTimer to display an updating clock in RealTimeView
         * PARAMETERS : N/A
         * RETURNS : N/A
         */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += Timer_Tick;
            LiveTime.Start();
        }

        /*
         * FUNCTION : Timer_Tick()
         * DESCRIPTION : Updates LiveTimeLabel with updated Date/Time
         * PARAMETERS : N/A
         * RETURNS : N/A
         */
        private void Timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss");

            string aircraftTailNo = cboAircraft.Text;

            if (cboAircraft.SelectedIndex > -1)
            {
                DatabaseController _ = new DatabaseController();
                TelemData data = new TelemData();
                data = _.FlightDataTableConnectionLive(aircraftTailNo);
                txtAccelX.Text = data.X.ToString();
                txtAccelY.Text = data.Y.ToString();
                txtAccelZ.Text = data.Z.ToString();
                txtWeight.Text = data.Weight.ToString();
                txtAlt.Text = data.Altitude.ToString();
                txtPitch.Text = data.Pitch.ToString();
                txtBank.Text = data.Bank.ToString();

            }
        }

        /*
         * FUNCTION : BtnSearch_Click()
         * DESCRIPTION : Button Click event handler for Search Button
         * PARAMETERS : object sender, RoutedEventArgs e
         * RETURNS : N/A
         */
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            // Go to SearchView Window for searching
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            if (viewModel.GotoSearchCommand.CanExecute(null))
            {
                viewModel.GotoSearchCommand.Execute(null);
            }
        }


        public RealTimeView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Window_Loaded);
        }


    }
}