using System;

namespace VialLogParsing
{
    public class Presence
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Duration { get => End.Subtract(Start); }

        public Presence(DataEntry entry)
        {
            Start = entry.DT;
        }
    }
}
