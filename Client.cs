/*
 * File : Client.cs
 * Project : Term Project - Flight Data management System V2
 * Programmer : Alex Palmer
 * First Version : 2021-11-09
 * Description :
 * This Program is the Client part of the FDMS V2 project
 * Its purpose is to read the data from the files one by one 
 * and send it to the server.
 * The Data is Organized by:
 * Aircraft tail #;Packet sequency #;aircraft data (with ; in between each set of data);Checksum
 */
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace Advanced_software_quality_milestone_2
{
    class Client
    {
        static void Main(string[] args)
        {
            //Create the tcp Client
            TcpClient tcpClient = new TcpClient();
            Console.WriteLine("Connecting......");
            //ip address, port number and packet sequence
            string ip = "170.0.0.1";
            int port = 7000;
            int ps = 1;
            //Connect to server
            tcpClient.Connect(ip, port);

            Console.WriteLine("Connected");
            //get data from files
            
            //file locations and tail names
            string file1 = "./Telemetry-Data/C-FGAX.txt";
            string tail1 = "C-FGAX";
            string file2 = "./Telemetry-Data/C-GEFC.txt";
            string tail2 = "C-GEFC";
            string file3 = "./Telemetry-Data/C-QWWT.txt";
            string tail3 = "C-QWWT";
            //read all the lines from the files and make a string array.
            string[] data1 = File.ReadAllLines(file1);
            string[] data2 = File.ReadAllLines(file2);
            string[] data3 = File.ReadAllLines(file3);
            //get stream
            Stream stream = tcpClient.GetStream();
            // convert data into bytes to be transmitted
            ASCIIEncoding asen = new ASCIIEncoding();
            //for each value in the array arrange the data properly inside the string and send it to server
            //program will sleep for 1/6 of a second every time it sends a message
            foreach (string line in data1)
            {
                string nline = line;
                nline = nline.Replace(',', ';');
                //couldn't find a way to remove space so i am bruteforcing it.
                //split all values into an array
                string[] remspace = nline.Split(' ');
                //add the values into a double and devide the value by 3, removing ; from the values
                double value = (Convert.ToDouble(remspace[6].Remove(11)) + Convert.ToDouble(remspace[7].Remove(8)) + Convert.ToDouble(remspace[8].Remove(8))) / 3;
                //round the value to the nearest value. ex, 2.6 becomes 3
                int cs = (int)Math.Ceiling(value);
                //add all the information into one string
                nline = string.Format("{0};{1};{2}{3}",tail1,ps++.ToString(),remspace[1] + ";" + remspace[2] + remspace[3] + remspace[4] + remspace[5] + remspace[6] + remspace[7] + remspace[8],cs.ToString());
                //turn the values into bytes and send it to the server
                byte[] bdata = asen.GetBytes(nline);
                Console.WriteLine("Transmitting - " + nline);
                stream.Write(bdata, 0, bdata.Length);
                //sleep for 1/6 of a second
                Thread.Sleep(166);
            }
            foreach (string line in data2)
            {
                string nline = line;
                nline = nline.Replace(',', ';');
                //couldn't find a way to remove space so i am bruteforcing it.
                //split all values into an array
                string[] remspace = nline.Split(' ');
                //add the values into a double and devide the value by 3, removing ; from the values
                double value = (Convert.ToDouble(remspace[6].Remove(11)) + Convert.ToDouble(remspace[7].Remove(8)) + Convert.ToDouble(remspace[8].Remove(8))) / 3;
                //round the value to the nearest value. ex, 2.6 becomes 3
                int cs = (int)Math.Ceiling(value);
                //add all the information into one string
                nline = string.Format("{0};{1};{2}{3}", tail2, ps++.ToString(), remspace[1] + ";" + remspace[2] + remspace[3] + remspace[4] + remspace[5] + remspace[6] + remspace[7] + remspace[8], cs.ToString());
                //turn the values into bytes and send it to the server
                byte[] bdata = asen.GetBytes(nline);
                Console.WriteLine("Transmitting - " + nline);
                stream.Write(bdata, 0, bdata.Length);
                //sleep for 1/6 of a second
                Thread.Sleep(166);
            }
            foreach (string line in data3)
            {
                string nline = line;
                nline = nline.Replace(',', ';');
                //couldn't find a way to remove space so i am bruteforcing it.
                //split all values into an array
                string[] remspace = nline.Split(' ');
                //add the values into a double and devide the value by 3, removing ; from the values
                double value = (Convert.ToDouble(remspace[6].Remove(11)) + Convert.ToDouble(remspace[7].Remove(8)) + Convert.ToDouble(remspace[8].Remove(8))) / 3;
                //round the value to the nearest value. ex, 2.6 becomes 3
                int cs = (int)Math.Ceiling(value);
                //add all the information into one string
                nline = string.Format("{0};{1};{2}{3}", tail3, ps++.ToString(), remspace[1] + ";" + remspace[2] + remspace[3] + remspace[4] + remspace[5] + remspace[6] + remspace[7] + remspace[8], cs.ToString());
                //turn the values into bytes and send it to the server
                byte[] bdata = asen.GetBytes(nline);
                Console.WriteLine("Transmitting - " + nline);
                stream.Write(bdata, 0, bdata.Length);
                //sleep for 1/6 of a second
                Thread.Sleep(166);
            }
            //once all the data is read the client closes
            tcpClient.Close();

        }
    }
}
