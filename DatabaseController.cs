/*
 * FILE            : DatabaseController.cs
 * PROJECT         : Flight Data Management System
 * PROGRAMMER      : Daniel Treacy, ASQ Group 4
 * FIRSTER VERSION : 2021-11-12
 * DESCRIPTION     :
 *  This file acts as the contoller between the FDMS and the Azure based SQL database.
 *  This includes select and insert statements.
 *  Also able to take data from database and write output file
 *  NOTE: Requires Microsoft.Data.SqlClient Nuget package.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DT = System.Data;
using QC = Microsoft.Data.SqlClient;

namespace AircraftTelemetry
{
    static class DatabaseController
    {
        private static readonly string ConnectionString = "Server=tcp:asq.database.windows.net,1433;Initial Catalog=asqDB;Persist Security Info=False;User ID=asqg4;Password=TsvVJDCMs2StwkF;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        /*
         * FUNCTION    : FlightDataTableConnection
         * DESCRIPTION : 
         *  This function is used to open the database connection to acquire all the rows from the database.
         * PARAMETERS  : None
         * RETURNS     : 
         *  List<TelemData> : List representation from all rows with the flight data.
         */
        public static List<TelemData> FlightDataTableConnection()
        {
            using (var connection = new QC.SqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Opened connection");
                List<TelemData> tData = SelectFlightData(connection);
                connection.Close();
                Console.WriteLine("Closed connection");
                return tData;
            }
        }

        /*
         * FUNCTION    : FlightDataTableConnection
         * DESCRIPTION : 
         *  This Overload function is used to open the database connection to acquire all the rows from a specific Aircraft.
         * PARAMETERS  : 
         *  string aircraftTail : The call sign of the desired aircraft.
         * RETURNS     : 
         *  List<TelemData> : List representation from all rows with the flight data of a specific aircraft.
         */
        public static List<TelemData> FlightDataTableConnection(string aircraftTail)
        {
            using (var connection = new QC.SqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Opened connection");
                List<TelemData> tData = SelectAircraftFlightData(connection, aircraftTail);
                connection.Close();
                Console.WriteLine("Closed connection");
                return tData;
            }
        }

        /*
         * FUNCTION    : FlightDataTableConnectionLive
         * DESCRIPTION : 
         *  This function writes the SQL query to retrive the telemetary data from the aircraft wnats the live data.
         * PARAMETERS  : 
         *  string aircraftTail : The call sign of the desired aircraft.
         * RETURNS     : 
         *  List<TelemData> : List representation from all rows with the flight data of a specific aircraft.
         */
        public static TelemData FlightDataTableConnectionLive(string aircraftTail)
        {
            using (var connection = new QC.SqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Opened connection");
                List<TelemData> tData = SelectAircraftFlightData(connection, aircraftTail);
                connection.Close();
                Console.WriteLine("Closed connection");
                if (tData.Count == 0)
                {
                    tData.Add(new TelemData());
                    return tData.Last();
                }
                return tData.Last();
            }
        }

        /*
         * FUNCTION    : SelectFlightData
         * DESCRIPTION :
         *  This function writes the SQL query to retrive the telemetary data from all aircraft
         * PARAMETERS  :
         *  SqlConnection : open connection object to the database
         * RETURNS     :
         *  List<TelemData> : List representation from all rows with the flight data.
         */
        public static List<TelemData> SelectFlightData(QC.SqlConnection connection)
        {
            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"
                SELECT StorageTime, Aircraft.Identifier, Attitude.Altitude, Attitude.Bank, Attitude.Pitch, Gforce.X, Gforce.Y, Gforce.Z, Gforce.Weight
                FROM ((([dbo].[FlightData]
                INNER JOIN Attitude on FlightData.AttitdueID = Attitude.AttitudeID)
                INNER JOIN Gforce on FlightData.GforceID = Gforce.GforceID)
                INNER JOIN Aircraft on FlightData.AircraftID = Aircraft.ID)";
                QC.SqlDataReader reader = command.ExecuteReader();
                List<TelemData> tData = new List<TelemData>();

                while (reader.Read())
                {
                    tData.Add(new TelemData()
                    {
                        //Data was returning time with whitespace and AM/PM, so it was removed
                        StorageTime = reader.GetDateTime(0).ToString().Remove(reader.GetDateTime(0).ToString().Length - 3, 3),
                        AircraftTailNumber = reader.GetString(1),
                        Altitude = (float)reader.GetSqlDouble(2),
                        Bank = (float)reader.GetSqlDouble(3),
                        Pitch = (float)reader.GetSqlDouble(4),
                        X = (float)reader.GetSqlDouble(5),
                        Y = (float)reader.GetSqlDouble(6),
                        Z = (float)reader.GetSqlDouble(7),
                        Weight = (float)reader.GetSqlDouble(8),
                    });
                }

                return tData;
            }
        }

        /*
         * FUNCTION    : SelectFlightData
         * DESCRIPTION :
         *  This function writes the SQL query to retrive the telemetary data from a specific aircraft
         * PARAMETERS  :
         *  SqlConnection connection: open connection object to the database
         *  string connection : Call sign of desired aircraft
         * RETURNS     :
         *  List<TelemData> : List representation from all rows with the flight data from a specitic aircraft.
         */
        public static List<TelemData> SelectAircraftFlightData(QC.SqlConnection connection, string aircraftTail)
        {
            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"
                SELECT StorageTime, Aircraft.Identifier, Attitude.Altitude, Attitude.Bank, Attitude.Pitch, Gforce.X, Gforce.Y, Gforce.Z, Gforce.Weight
                FROM ((([dbo].[FlightData]
                INNER JOIN Attitude on FlightData.AttitdueID = Attitude.AttitudeID)
                INNER JOIN Gforce on FlightData.GforceID = Gforce.GforceID)
                INNER JOIN Aircraft on FlightData.AircraftID = Aircraft.ID)
                WHERE Aircraft.Identifier = @AircraftTail";

                QC.SqlParameter parameter;
                parameter = new QC.SqlParameter("@AircraftTail", DT.SqlDbType.VarChar);
                parameter.Value = aircraftTail;
                command.Parameters.Add(parameter);

                QC.SqlDataReader reader = command.ExecuteReader();
                List<TelemData> tData = new List<TelemData>();

                while (reader.Read())
                {
                    tData.Add(new TelemData()
                    {
                        //Data was returning time with whitespace and AM/PM, so it was removed
                        StorageTime = reader.GetDateTime(0).ToString().Remove(reader.GetDateTime(0).ToString().Length - 3, 3),
                        AircraftTailNumber = reader.GetString(1),
                        Altitude = (float)reader.GetSqlDouble(2),
                        Bank = (float)reader.GetSqlDouble(3),
                        Pitch = (float)reader.GetSqlDouble(4),
                        X = (float)reader.GetSqlDouble(5),
                        Y = (float)reader.GetSqlDouble(6),
                        Z = (float)reader.GetSqlDouble(7),
                        Weight = (float)reader.GetSqlDouble(8),
                    });
                }

                return tData;
            }
        }

        /*
         * FUNCTION    : InsetConnection
         * DESCRIPTION :
         *  Takes the parsed telemetary data and opens the database connection for insert
         * PARAMETERS  :
         *  Dictionary<string,string> telemetryData : contains the elements of the parsed telemtary data as a dictionary
         *  X : string
         *  Y : string
         *  Z : string
         *  Weight : string
         *  Altitude : string
         *  Pitch : string
         *  Bank : string
         * RETURNS     :
         *  List<TelemData> : Holds the most recent update of the database after the insert.
         */
        public static TelemData InsertConnection(Dictionary<string, string> telemetryData)
        {
            

            // Convert Aircraft name to ID
            switch (telemetryData["AircraftTailNumber"])
            {
                case "C-FGAX":
                    telemetryData.Add("AircraftID", "1");
                    break;
                case "C-GEFC":
                    telemetryData.Add("AircraftID", "2");
                    break;
                case "C-QWWT":
                    telemetryData.Add("AircraftID", "3");
                    break;
                default:
                    Console.WriteLine("Unknown Aircraft");
                    return null;
            }
            TelemData tData;
            using (var connection = new QC.SqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Opened connection");
                tData = InsertTelemetry(connection, telemetryData);
                Console.WriteLine("Closed connection");
                connection.Close();
            }

            return tData;
        }

        /*
         * FUNCTION    : InsertTelemetry
         * DESCRIPTION :
         *  Uses the open connection and telemtry data and write to the SQL database
         * PARAMETERS  :
         *  SqlConnection connection : Open connection obejct to the databse
         *  Dictionary<string,string> : Parsed telemetry data to be inserted to the database.
         * RETURNS     :
         *  List<TelemData> : Returns the newly updated database rows of all aircraft
         */
        public static TelemData InsertTelemetry(QC.SqlConnection connection, Dictionary<string, string> telemetryData)
        {
            QC.SqlParameter parameter;
            using (var command = new QC.SqlCommand())
            {
                command.Connection = connection;
                command.CommandType = DT.CommandType.Text;
                command.CommandText = @"
                INSERT INTO gforce
                (X,
                 Y,
                 Z,
                 Weight)
                OUTPUT
                INSERTED.GforceID
                VALUES
                (@X,
                 @Y,
                 @Z,
                 @Weight);";

                parameter = new QC.SqlParameter("@X", DT.SqlDbType.Float);
                parameter.Value = telemetryData["X"];
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@Y", DT.SqlDbType.Float);
                parameter.Value = telemetryData["Y"];
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@Z", DT.SqlDbType.Float);
                parameter.Value = telemetryData["Z"];
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@Weight", DT.SqlDbType.Float);
                parameter.Value = telemetryData["Weight"];
                command.Parameters.Add(parameter);

                // Save newly created GforceID for FlightData table
                int gforceId = (int)command.ExecuteScalar();

                command.CommandText = @"
                INSERT INTO Attitude
                (Altitude,
                 Pitch,
                 Bank)
                OUTPUT
                INSERTED.AttitudeID
                VALUES
                (@Altitude,
                 @Pitch,
                 @Bank);";
                parameter = new QC.SqlParameter("@Altitude", DT.SqlDbType.Float);
                parameter.Value = telemetryData["Altitude"];
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@Pitch", DT.SqlDbType.Float);
                parameter.Value = telemetryData["Pitch"];
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@Bank", DT.SqlDbType.Float);
                parameter.Value = telemetryData["Bank"];
                command.Parameters.Add(parameter);

                // Save newly created AttitudeID for FlightData table
                int attidueId = (int)command.ExecuteScalar();

                // Insert the newly created IDs from the other tables into linking table 
                command.CommandText = @"
                INSERT INTO FlightData
                (AircraftID,
                 GforceID,
                 AttitdueID,
                 StorageTime)
                VALUES
                (@AircraftID,
                 @GforceID,
                 @AttitdueID,
                 GETDATE() AT TIME ZONE 'UTC' AT TIME ZONE 'Eastern Standard Time');";
                parameter = new QC.SqlParameter("@AircraftID", DT.SqlDbType.Int);
                parameter.Value = telemetryData["AircraftID"];
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@GforceID", DT.SqlDbType.Int);
                parameter.Value = gforceId;
                command.Parameters.Add(parameter);

                parameter = new QC.SqlParameter("@AttitdueID", DT.SqlDbType.Int);
                parameter.Value = attidueId;
                command.Parameters.Add(parameter);

                command.ExecuteScalar();
            }

            // For live update get newest data
            List <TelemData> tData = SelectFlightData(connection);
            return tData.LastOrDefault();


        }

        /*
         * FUNCTION    : OutputAircraftFile
         * DESCRIPTION :
         *  Writes an ASCII file for a selected aircraft
         * PARAMETERS  :
         * string aircraft : Name of the aircraft for desired file
         * RETURNS     : None
         */
        public static void OutputAircraftFile(string aircraftTail)
        {
            List<TelemData> tData = DatabaseController.FlightDataTableConnection(aircraftTail);
            string path = aircraftTail + ".txt";

            // Checks if a file already exists for the the aircraft if it doesn't create else append
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                    foreach (TelemData t in tData)
                    {
                        sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", t.StorageTime, t.X, t.Y, t.Z, t.Weight, t.Altitude, t.Pitch, t.Bank);
                    }
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(path, true))
                {
                    foreach (TelemData t in tData)
                    {
                        sw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", t.StorageTime, t.X, t.Y, t.Z, t.Weight, t.Altitude, t.Pitch, t.Bank);
                    }
                }
            }
        }
    }
}
