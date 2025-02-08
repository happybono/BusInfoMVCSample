using System;

namespace BusInfoMVCSample.Models
{
    public class BusInfo
    {
        public string RouteName { get; set; }
        public int? ArrivalTime1 { get; set; }
        public int? ArrivalTime2 { get; set; }
        public string DestName { get; set; }
        public string StopCount1 { get; set; }
        public string StopCount2 { get; set; }
        public string Crowded1 { get; set; }
        public string Crowded2 { get; set; }
        public string LowPlate1 { get; set; }
        public string LowPlate2 { get; set; }

        public string ErrorMessage {  get; set; }
    }
}
