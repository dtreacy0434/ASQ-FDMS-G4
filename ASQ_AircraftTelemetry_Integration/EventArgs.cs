using System;

/*
* FILE : EventArgs.cs
* PROJECT : SENG3020 - Term Project Milestone 2
* PROGRAMMER : Faith Madore
* DESCRIPTION : Event Arguments - For switching between the two views
*/

namespace AircraftTelemetry
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}