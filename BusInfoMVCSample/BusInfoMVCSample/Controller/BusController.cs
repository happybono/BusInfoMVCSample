using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using BusInfoMVCSample.Models;
using BusInfoMVCSample.Properties;

namespace BusInfoMVCSample.Controllers
{
    public class BusController
    {
        private readonly string serviceKey = Resources.ServiceKey.ToString();
        private readonly List<string> routeIds;
        private readonly List<string> bstopIds;
        private readonly Form parentForm;

        public BusController(List<string> routeIds, List<string> bstopIds, Form parentForm)
        {
            if (routeIds.Count != bstopIds.Count)
            {
                throw new Exception("Route IDs and Stop IDs lists must have the same length.");
            }

            this.routeIds = routeIds;
            this.bstopIds = bstopIds;
            this.parentForm = parentForm;
        }

        public void OpenSettingsDialog()
        {
            using (FrmSetup setupForm = new FrmSetup())
            {
                setupForm.ShowDialog(parentForm); // Show FrmSetup as a dialog
            }
        }

        public async Task<List<BusInfo>> GetBusInfoAsync()
        {
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

                        if (IsValidXml(responseBody))
                        {
                            var busInfo = ParseBusInfo(responseBody);
                            busInfoList.Add(busInfo);
                        }
                        else
                        {
                            throw new Exception("Invalid XML response received.");
                        }
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

        private bool IsValidXml(string xml)
        {
            try
            {
                XElement.Parse(xml);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<BusInfo> GetUrgentBusInfo(List<BusInfo> busInfoList)
        {
            var urgentBusInfoList = busInfoList
                .Where(busInfo => busInfo.ArrivalTime1.HasValue && busInfo.ArrivalTime1.Value < 2)
                .OrderBy(busInfo => busInfo.ArrivalTime1.Value)
                .ToList();

            return urgentBusInfoList;
        }

        // Add a method to get urgent bus info for the second group
        public List<BusInfo> GetUrgentBusInfoGroup2(List<BusInfo> busInfoList)
        {
            var urgentBusInfoList2 = busInfoList
                .Where(busInfo => busInfo.ArrivalTime1.HasValue && busInfo.ArrivalTime1.Value < 2)
                .OrderBy(busInfo => busInfo.ArrivalTime1.Value)
                .ToList();

            return urgentBusInfoList2;
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
