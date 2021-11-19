/*
* FILE : TcpConnection.cs
* PROJECT : SENG3020 - FDMS Ground Terminal System
* PROGRAMMER : Stephen Perrin, Faith Madore, Alex Palmer, Daniel Treacy
* FIRST VERSION : 2021-11-11
* DESCRIPTION :
* The functions in this file are used to create a asyncronous TCP/IP Socket server. The server
* handles getting data from the client, parses it into readable tokens, and returns the parsed
* data through an event handler.
*/
using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Threading;

namespace AircraftTelemetry
{
    /*
    * NAME : StateObject
    * PURPOSE : The StateObject class is an auxillary class to the TcpConnection class.
    * This class is meant to hold the receive buffer from the client, a string of the converted
    * receiveBuffer, and the Socket associated with the client. This object is meant to be
    * transfered between callbacks for continuity of the data between threads. 
    */
    public class StateObject
    {
        // Size of receive buffer.  
        public const int BufferSize = 1024;

        // Receive buffer.  
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;
    }

    /*
    * NAME : TcpConnection
    * PURPOSE : The TcpConnection is a class that creates, initializes, and monitors an asyncronous TCP/IP
    * Socket server. It has one regular static method for creating and listening to the server. An async 
    * callback method to accept a new client and a second async callback method to read any data from the
    * client. 
    * It also has a NewDataReceived event that is triggered when new data is received from the client. This
    * data is returned to the event handler method associated with the NewDataReceived event.
    * 
    * Class Usage:
    *       To associate an event handler method with the class's NewDataReceived event do:
    *       TcpConnection.StartListening();                                     **** This is for the initial startup of the server and must come before the next line
    *       TcpConnection.NewDataReceived += NewClientMsgHandler;               **** This is an example of assigning the EventHandler method (NewClientMsgHandler) to the NewDataReceived event
    *
    *       The EventHandler method must have a basic outline like such:
    *       public void NewClientMsgHandler(object data, EventArgs e)           **** Object data will hold the returned client data from the worker callback thread; EventArgs e will usually be Empty
    *       {
    *           TelemData tData = (TelemData)data;                              **** This line is an example of taking out the received client data from the Object data parameter
    *       }
    */
    public class TcpConnection
    {
        // Public Members 
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static event EventHandler NewDataReceived;

        public TcpConnection() { }

        /*
        * FUNCTION : StartListening
        * DESCRIPTION :
        *           This method initializes and binds the ip address and port to
        *           the listener socket and calls the next AcceptCallback callback
        *           method.
        * PARAMETERS :
        *   N/A
        * RETURNS :
        *   N/A
        */
        public static void StartListening()
        {
            // Variables 
            //AdressList may very based on where the program is ran  ----- This only works for MY IP will have to change for your
            IPHostEntry ipHostInfo = Dns.GetHostEntry("127.0.0.1");                             // Get host ip
            IPAddress ipAddress = ipHostInfo.AddressList[7];                                    // Convert previous line to IPAddress
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 7000);                         // Collect ip address and port to a local IP End Point

            // Create a TCP/IP socket.  
            Socket listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.  
            try
            {
                listener.Bind(localEndPoint);                                                   // Bind the ip and port to the listener
                listener.Listen(100);                                                           // Sets maximum backlog to 100 transmission from the client(s)
                listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);              // Starts the async callback method for accepting a new client
            }
            catch (Exception e)
            {
                // Creates an Error log file of the current exception
                StreamWriter writer = new StreamWriter("Listener Error - " + DateTime.Today.ToString().Remove(10) + ".txt");        // Error Log Filename 
                writer.WriteLine(DateTime.Now.ToString() + ": \t\t" + e.ToString());                                                // The Error
                
                // Close the text writer stream
                writer.Dispose();
                writer.Close();
            }
        }

        /*
        * FUNCTION : AcceptCallback
        * DESCRIPTION :
        *           This method listens to the server and waits for any new connection attempts.
        * PARAMETERS :
        *   IAsyncResult ar :   The result of the async callback method call. Contains the state object, listener in this case. 
        * RETURNS :
        *   N/A
        */
        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.  
            allDone.Set();

            // Get the listener and clientHandler sockets from the IAsyncResult parameter
            Socket listener = (Socket)ar.AsyncState;
            Socket clientHandler = listener.EndAccept(ar);

            /* Create a new state object, assign the clientHandler socket to the state's workSocket for later 
               continuity, and call the ReadCallback callback method to receive any new data from the client. */ 
            StateObject state = new StateObject();
            state.workSocket = clientHandler;
            clientHandler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        /*
        * FUNCTION : ReadCallback
        * DESCRIPTION :
        *           This method listens to the server and waits for any new connection attempts.
        * PARAMETERS :
        *   IAsyncResult ar :   The result of the async callback method call. Contains the StateObject, the previous callback method's StateObject in this case. 
        * RETURNS :
        *   N/A
        */
        public static void ReadCallback(IAsyncResult ar)
        {
            // Variables
            String content = String.Empty;                                                                  // Variable to hold the received client data in string format

            // Get the listener and clientHandler sockets from the IAsyncResult parameter  
            StateObject state = (StateObject)ar.AsyncState;
            Socket clientHandler = state.workSocket;

            // Read data from the client socket.
            int bytesRead = clientHandler.EndReceive(ar);

            // Check if there is any data to be worked with
            if (bytesRead > 0)
            {

                // Run task for parsing, confirming checksum, and sending to main thread for usage
                Task t = Task.Run(() =>
                {

                    state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                    content = state.sb.ToString();
                    state.sb.Clear();
                    TelemData tData;
                    string[] tempDataArray = content.Split(';');                                // Split to tail#, seq#, telemData, checksum
                    string[] telemDataTokens = tempDataArray[3].Split(',');                        // Split telemData into tokens
                    object[] tempTelemData = new object[telemDataTokens.Length];                // object array to hold cleaned and prepared telemetry data to enter into TelemData object
                    //float tempFloat = 0.0f;                                                     // Temp float var for conversion to floating point values


                    //// Clean telemDataTokens
                    //for (int i = 0; i < telemDataTokens.Length; i++)
                    //{
                    //    telemDataTokens[i].Replace(',', (char)0);                               // Remove extra commas
                    //    telemDataTokens[i].Trim();                                              // Trim whitespaces

                    //    // Check if there is a space in the beginning and remove it; else save to additional data array
                    //    if (telemDataTokens[i][0] == ' ')
                    //    {
                    //        tempTelemData[i] = telemDataTokens[i].Substring(1, telemDataTokens[i].Length - 1);
                    //    }
                    //    else
                    //    {
                    //        tempTelemData[i] = telemDataTokens[i];
                    //    }
                    //}

                    //// Transfer cleaned data to object array
                    //tempTelemData = telemDataTokens;

                    //// Temp float array for assigning floating points to TelemData object
                    //float[] copyFloatData = new float[tempTelemData.Length];

                    //// Check if can convert to float; if not do nothing
                    //for (int i = 0; i < tempTelemData.Length - 1; i++)
                    //{
                    //    // Check if conversion to float is possible; if yes copy converted float to temp float array
                    //    if (float.TryParse((string)tempTelemData[i], out tempFloat))
                    //    {
                    //        copyFloatData[i - 1] = tempFloat;
                    //    }
                    //}

                    // Create new TelemData, fill with appropriate data, and send out
                    tData = new TelemData(tempDataArray[0], (string)tempDataArray[2], float.Parse(tempDataArray[3]), float.Parse(tempDataArray[4]), float.Parse(tempDataArray[5]), float.Parse(tempDataArray[6]), float.Parse(tempDataArray[7]), float.Parse(tempDataArray[8]), float.Parse(tempDataArray[9]));

                    // Confirm Checksum is valid
                    double calcCheckSum = ((double)((tData.Altitude + tData.Pitch + tData.Bank) / 3));
                    int checkValue = (int)Math.Ceiling(calcCheckSum);
                    double sentcheckSum = double.Parse(tempDataArray[tempDataArray.Length - 2]);
                    int checkSum = (int)Math.Ceiling(sentcheckSum);
                    //Set as always true for the moment ot allow the connection to always wokr
                    if (true)
                    {
                        // Trigger event and send TelemData through invoking of the event
                        NewDataReceived?.Invoke(tData, EventArgs.Empty);
                    }
                    // Need to call these functiosn at some point to update 
                    DatabaseController databaseController = new DatabaseController();

                    databaseController.InsertConnection(tData.ConvertToDictionary());

                    //
                    if (!(content.IndexOf("<EOF>") > -1))
                    {
                        // Recall ReadCallback callback method for more data to receive 
                        clientHandler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                    }
                });
            }

        }
    }
}
