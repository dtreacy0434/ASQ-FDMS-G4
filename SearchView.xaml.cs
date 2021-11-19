using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

/*
* FILE : SearchView.xaml.cs
* PROJECT : SENG3020 - Term Project Milestone 2
* PROGRAMMER : Faith Madore
* DESCRIPTION : .cs file for SearchView - Loading the window with information, button click event handlers
*/

namespace AircraftTelemetry
{
    public partial class SearchView : UserControl
    {
        /*
         * FUNCTION : Window_Loaded()
         * DESCRIPTION : Starts a DispatcherTimer to display an updating clock in SearchView
         * PARAMETERS : N/A
         * RETURNS : N/A
         */
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += Timer_Tick;
            LiveTime.Start();

            SearchInputBox.Visibility = Visibility.Visible;
        }


        /*
         * FUNCTION : Timer_Tick()
         * DESCRIPTION : Updates LiveTimeLabel with updated Date/Time
         * PARAMETERS : N/A
         * RETURNS : N/A
         */
        private void Timer_Tick(object sender, EventArgs e) => LiveTimeLabel.Content = DateTime.Now.ToString("yyyy/mm/dd HH:mm:ss");


        /*
         * FUNCTION : BtnSearch_Click()
         * DESCRIPTION : Button Click event handler for Search Button
         * PARAMETERS : object sender, RoutedEventArgs e
         * RETURNS : N/A
         */
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchInputBox.Visibility = Visibility.Visible;
        }


        /*
         * FUNCTION : BtnEnterSearch_Click()
         * DESCRIPTION : Button Click event handler for Enter Search Button - Validates input is correct and not null
         * PARAMETERS : object sender, RoutedEventArgs e
         * RETURNS : N/A
         */
        private void BtnEnterSearch_Click(object sender, RoutedEventArgs e)
        {
            string aircraftTailNo = InputTextBox.Text.ToUpper();

            if (aircraftTailNo != null && aircraftTailNo.Contains("C-"))
            {
                txtError.Visibility = Visibility.Hidden;
                DisplaySearchResults(aircraftTailNo);
                SearchInputBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtError.Text = "Please Enter a Valid Aircraft Tail Number ('C-FGAX')";
                txtError.Visibility = Visibility.Visible;
            }
        }

        /*
         * FUNCTION : DisplaySearchResults()
         * DESCRIPTION : Retrieves Data based on users searched date and displays in SearchView
         * PARAMETERS : string userSearch - Users entered search terms
         * RETURNS : N/A
         */
        private void DisplaySearchResults(string aircraftTailNo)
        {
            FlightTailNo.Text = "Aircraft Tail Number: " + aircraftTailNo;

            DatabaseController dbController = new DatabaseController();
            List<TelemData> tData = new List<TelemData>();
            tData = dbController.FlightDataTableConnection(aircraftTailNo);

            FlightDataGrid.ItemsSource = tData;
        }


        /*
         * FUNCTION : BtnCancelSearch_Click()
         * DESCRIPTION : Button Click event handler for Search Button - Cancel Search
         * PARAMETERS : object sender, RoutedEventArgs e
         * RETURNS : N/A
         */
        private void BtnCancelSearch_Click(object sender, RoutedEventArgs e)
        {
            txtError.Visibility = Visibility.Hidden;
            SearchInputBox.Visibility = Visibility.Collapsed;
        }



        /*
         * FUNCTION : BtnLog_Click()
         * DESCRIPTION : Button Click event handler for Log Button
         * PARAMETERS : object sender, RoutedEventArgs e
         * RETURNS : N/A
         */
        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            string aircraftTailNo = InputTextBox.Text.ToUpper();

            DatabaseController dbController = new DatabaseController();
            dbController.OutputAircraftFile(aircraftTailNo);
        }

        public SearchView()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(Window_Loaded);
        }
    }
}