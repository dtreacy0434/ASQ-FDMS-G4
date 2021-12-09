//* FILE : Server.cs
//* PROJECT : SENG3020 - FDMS Ground Terminal System
//* PROGRAMMER : Stephen Perrin, Faith Madore, Alex Palmer, Daniel Treacy
//* FIRST VERSION : 2021-11-11
//* DESCRIPTION :
//* The functions in this file are used to create a asyncronous TCP/IP Socket server. The server
//* handles getting data from the client, parses it into readable tokens, and returns the parsed
//* data through an event handler.
//*/



using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AircraftTelemetry
{

    class Server
    {
        TcpListener server = null;
        public Server(string ip, int port)
        {
            //start server cand begin to listen
            IPAddress localAddr = IPAddress.Parse(ip);
            server = new TcpListener(localAddr, port);
            server.Start();
            //Spawns a thread to allow the system to continue running
            Thread t = new Thread(new ParameterizedThreadStart(StartListener));
            t.Start();
        }

        public void StartListener(Object obj)
        {
            try
            {
                // connect to server
                Console.WriteLine("Waiting for a connection...");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");
                //create thread for client
                Thread t = new Thread(new ParameterizedThreadStart(HandleDeivce));
                t.Start(client);

            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
                server.Stop();
            }
        }

        public void HandleDeivce(Object obj)
        {
            //grab stream
            TcpClient client = (TcpClient)obj;
            var stream = client.GetStream();

            string data;
            Byte[] bytes = new Byte[150];
            int i;
            try
            {
                // read split and organize data
                while ((i = stream.Read(bytes, 0, 150)) != 0)
                {
                    
                    data = Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine("{1}: Received: {0}", data, Thread.CurrentThread.ManagedThreadId);
                    data = data.Trim('@');
                    string[] array = data.Split(';');
                    // add function to add to database here
                    TelemData tData = new TelemData(array[0], float.Parse(array[3]), float.Parse(array[4]), float.Parse(array[5]), float.Parse(array[6]), float.Parse(array[7]), float.Parse(array[8]), float.Parse(array[9]));
                    //Add checksum
                    int checksumSent = int.Parse(array[10]);
                    int checksumCalc = (int)Math.Ceiling((float.Parse(array[7]) + float.Parse(array[8]) + float.Parse(array[9])) / 3);
                    if (checksumCalc == checksumSent)
                    {
                        DatabaseController.InsertConnection(tData.ConvertToDictionary());
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.ToString());
                client.Close();
            }
        }
    }
}