using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using BusInfoMVCSample.Models;
using BusInfoMVCSample.Properties;

namespace BusInfoMVCSample.Controllers
{
    public class BusController
    {
        private readonly string serviceKey = Resources.ServiceKey.ToString();
        private readonly List<string> routeIds = new List<string> { "224000014", "216000011", "216000004" };
        private readonly List<string> bstopIds = new List<string> { "224000719", "224000719", "224000719" };

        public async Task<List<BusInfo>> GetBusInfoAsync()
        {
            if (routeIds.Count != bstopIds.Count)
            {
                throw new Exception("Route IDs and Stop IDs lists must have the same length.");
            }

            List<BusInfo> busInfoList = new List<BusInfo>();

            for (int i = 0; i < routeIds.Count; i++)
            {
                string routeId = routeIds[i];
                string bstopId = bstopIds[i];
                string apiUrl = $"https://apis.data.go.kr/6410000/busarrivalservice/v2/getBusArrivalItemv2?serviceKey={serviceKey}&routeId={routeId}&stationId={bstopId}&format=xml";

                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode();

                        string responseBody = await response.Content.ReadAsStringAsync();
                        var busInfo = ParseBusInfo(responseBody);
                        busInfoList.Add(busInfo);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error fetching bus information: " + ex.Message);
                }
            }

            return busInfoList;
        }

        private BusInfo ParseBusInfo(string xmlData)
        {
            XElement xml = XElement.Parse(xmlData);

            // Check for error code
            var returnReasonCode = xml.Descendants("returnReasonCode").FirstOrDefault()?.Value;
            if (!string.IsNullOrEmpty(returnReasonCode) && returnReasonCode != "0")
            {
                return new BusInfo { ErrorMessage = "Error Code: " + returnReasonCode };
            }

            var arrivalTime1 = xml.Descendants("predictTime1").FirstOrDefault()?.Value;
            var stopCount1 = xml.Descendants("locationNo1").FirstOrDefault()?.Value;
            var crowded1 = xml.Descendants("crowded1").FirstOrDefault()?.Value;
            var arrivalTime2 = xml.Descendants("predictTime2").FirstOrDefault()?.Value;
            var stopCount2 = xml.Descendants("locationNo2").FirstOrDefault()?.Value;
            var crowded2 = xml.Descendants("crowded2").FirstOrDefault()?.Value;
            var routeName = xml.Descendants("routeName").FirstOrDefault()?.Value;
            var destName = xml.Descendants("routeDestName").FirstOrDefault()?.Value;
            var LowPlate1 = xml.Descendants("lowPlate1").FirstOrDefault()?.Value;
            var LowPlate2 = xml.Descendants("lowPlate2").FirstOrDefault()?.Value;

            return new BusInfo
            {
                RouteName = routeName,
                DestName = destName,
                ArrivalTime1 = string.IsNullOrEmpty(arrivalTime1) ? (int?)null : int.Parse(arrivalTime1),
                ArrivalTime2 = string.IsNullOrEmpty(arrivalTime2) ? (int?)null : int.Parse(arrivalTime2),
                StopCount1 = stopCount1,
                StopCount2 = stopCount2,
                Crowded1 = crowded1,
                Crowded2 = crowded2,
                LowPlate1 = LowPlate1,
                LowPlate2 = LowPlate2
            };
        }


        public List<BusInfo> GetUrgentBusInfo(List<BusInfo> busInfoList)
        {
            var urgentBusInfoList = busInfoList
                .Where(busInfo => busInfo.ArrivalTime1.HasValue && busInfo.ArrivalTime1.Value < 2)
                .OrderBy(busInfo => busInfo.ArrivalTime1.Value)
                .ToList();

            return urgentBusInfoList;
        }

        public string GetCongestionText(string crowded)
        {
            return crowded == "2" ? "혼잡" : crowded == "1" ? "여유" : "보통";
        }

        public string GetLowPlateText(string lowPlate)
        {
            return lowPlate == "0" ? "일반" : lowPlate == "1" ? "저상" : "";
        }
    }
}
