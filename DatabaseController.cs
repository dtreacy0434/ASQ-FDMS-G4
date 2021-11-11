using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DT = System.Data;
using QC = Microsoft.Data.SqlClient;

namespace ASQ_DB
{
     class DatabaseController
    {

        
        
        /* Function - makes connection to DB for ALL flights
        *  Return all flight data as TelemData List
        */
        public List<TelemData> FlightDataTableConnection()
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

        /* Function - makes connection to DB for SPECIFIC flights
        *  Return all flight data as TelemData List
        */
        public List<TelemData> FlightDataTableConnection(string aircraftTail)
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
         * Function - used to pull the data for ALL flights.
         *
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
                        StorageTime = reader.GetSqlDateTime(0).ToString(),
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
        * Function - used to pull the data for SPECIFIC flights.
        *
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
                        StorageTime = reader.GetSqlDateTime(0).ToString(),
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
         * Function - This should be called with the parsed telemdata, it expectes the data to be in dictionary format.
         */
        public void InsertConnection(Dictionary<string,string> telemetryData)
        {

            switch (telemetryData["aircraft"])
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
                    return;
            }

            using (var connection = new QC.SqlConnection(ConnectionString))
            {
                connection.Open();
                Console.WriteLine("Opened connection");
                DatabaseController.InsertTelemetry(connection, telemetryData);
                Console.WriteLine("Closed connection");
                connection.Close();
            }
        }


        /*
         * Function : Used the created connection and telemerty data and inserts it into each of the databases.
         * 
         */
        public static void InsertTelemetry(QC.SqlConnection connection, Dictionary<string, string> telemetryData)
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
                INSERTED.ID
                VALUES
                (@X,
                 @Y,
                 @Z,
                 @Weight);";

                // Might change float to a real to use less storage
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

                int gforceId = (int)command.ExecuteScalar();

                command.CommandText = @"
                INSERT INTO attidue
                (Altitude,
                 Pitch,
                 Bank)
                OUPUT
                INSERTED.ID
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

                int attidueId = (int)command.ExecuteScalar();

                // smalldatatime 
                command.CommandText = @"
                INSERT INTO flight_data
                (AircraftID,
                 GforceID,
                 AttitdueID,
                 StorageTime)
                VALUES
                (@AircraftID,
                 @GforceID,
                 @AttitdueID,
                 GETDATE() AS smalldatetime);";
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
        }
        /*
         * This function takes the aircraft tail of the desire aircraft and writes the file
         * TODO right now for some reason the storagetime has an am/pm at the end will look to remove
         */
        public void writeFile(string aircraftTail)
        {
            List<TelemData> tData = this.FlightDataTableConnection(aircraftTail);
            string path = aircraftTail + ".txt";
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

        }
    }
}
