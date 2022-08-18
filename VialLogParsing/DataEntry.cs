using System;

namespace VialLogParsing
{
    public class DataEntry
    {
        public string Name { get; set; }
        public DateTime DT { get; set; }
        public double Reading { get; set; }
        public bool State { get; set; }
        public bool Valid { get; set; } = false;

        public DataEntry(string name, string data)
        {
            Name = name;
            string[] cols = data.Split('\t');
            try
            {
                DT = DateTime.Parse(cols[0]);
                Reading = double.Parse(cols[5]);
                State = bool.Parse(cols[7]);
            }
            catch (Exception) { }
            Valid = true;
        }
    }
}
