using System;
using System.Collections.Generic;
using System.Text;

namespace Ground_Terminal_System
{
    public class TelemData
    {
        public string AircraftTailNumber { get; set; }

        public string StorageTime { get; set; }

        public float X { get; set; }

        public float Y { get; set; }

        public float Z { get; set; }

        public float Weight { get; set; }

        public float Altitude { get; set; }

        public float Pitch { get; set; }

        public float Bank { get; set; }

        /*
        * FUNCTION : TelemData
        * DESCRIPTION :
        *           This constructor initializes the object's data members
        * PARAMETERS :
        *   string tn   :   AircraftTailNumber
        *   string st   :   StorageTime
        *   float _x    :   X
        *   float _y    :   Y
        *   float _z    :   Z
        *   float w     :   Weight
        *   float alt   :   Altitude
        *   float p     :   Pitch
        *   float b     :   Bank
        * RETURNS :
        *   N/A
        */
        public TelemData(string tn, string st, float _x, float _y, float _z, float w, float alt, float p, float b)
        {
            AircraftTailNumber = tn;
            StorageTime = st;
            X = _x;
            Y = _y;
            Z = _z;
            Weight = w;
            Altitude = alt;
            Pitch = p;
            Bank = b;
        }

        /*
        * FUNCTION : ConvertToDictionary
        * DESCRIPTION :
        *           This method converts the object's data members into Key/Value strings
        *           and adds it to the return dictionary
        * PARAMETERS :
        *   N/A
        * RETURNS :
        *   Dictionary<string, string> : Returns the dictionary of the converted object's data members
        */
        public Dictionary<string, string> ConvertToDictionary()
        {
            // Variable
            Dictionary<string, string> retDict = new Dictionary<string, string>();

            // Converts data members to strings and adds them to the return dictionary
            retDict.Add("AircraftTailNumber", AircraftTailNumber.ToString());
            retDict.Add("StorageTime", StorageTime.ToString());
            retDict.Add("X", X.ToString());
            retDict.Add("Y", Y.ToString());
            retDict.Add("Z", Z.ToString());
            retDict.Add("Weight", Weight.ToString());
            retDict.Add("Altitude", Altitude.ToString());
            retDict.Add("Pitch", Pitch.ToString());
            retDict.Add("Bank", Bank.ToString());

            return retDict;
        }

        public override string ToString()
        {
            return ("Tail Number: " + AircraftTailNumber + "; " +
                "Date/time: " + StorageTime.ToString() + "; " +
                "Accel-X: " + X.ToString() + "; " +
                "Accel-Y: " + Y.ToString() + "; " +
                "Accel-Z: " + Z.ToString() + "; " +
                "Weight: " + Weight.ToString() + "; " +
                "Altutide: " + Altitude.ToString() + "; " +
                "Pitch: " + Pitch.ToString() + "; " +
                "Bank: " + Bank.ToString());
        }
    }
}
