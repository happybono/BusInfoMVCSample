using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using BusInfoMVCSample.Controllers; // Ensure this namespace is correct
using BusInfoMVCSample.Models;
using BusInfoMVCSample.Properties;

namespace BusInfoMVCSample.Views
{
    public partial class MainForm : Form
    {
        private FrmSetup _setupForm;

        private readonly string serviceKey = Resources.ServiceKey.ToString();

        private List<System.Windows.Forms.Label> routeLabels;
        private List<System.Windows.Forms.Label> destLabels;
        private List<System.Windows.Forms.Label> arrivalTimeLabels;
        private List<System.Windows.Forms.Label> stopCountLabels;
        private List<System.Windows.Forms.Label> secondArrivalTimeLabels;
        private List<System.Windows.Forms.Label> secondStopCountLabels;
        private List<System.Windows.Forms.Label> congestionLabels;
        private List<System.Windows.Forms.Label> urgentCongestionLabels;
        private List<System.Windows.Forms.Label> secondCongestionLabels;
        private List<System.Windows.Forms.Label> lowPlateLabels;
        private List<System.Windows.Forms.Label> secondLowPlateLabels;
        private List<System.Windows.Forms.Label> urgentRouteLabels;
        private List<System.Windows.Forms.Label> timeDiffLabels;
        private List<System.Windows.Forms.Label> errorLabels;
        private List<System.Windows.Forms.Label> urgentRouteLabels2;
        private List<System.Windows.Forms.Label> urgentCongestionLabels2;
        private List<System.Windows.Forms.Label> timeDiffLabels2;

        private List<Panel> errorPanels;
        private List<Panel> secondBusPanels;

        private BusController _busController;
        private List<string> _routeIds;
        private List<string> _bstopIds;

        private Timer _currentTimeTimer;

        public MainForm(List<string> routeIds, List<string> bstopIds)
        {
            InitializeComponent();
            InitializeCurrentTimeTimer();

            _routeIds = routeIds;
            _bstopIds = bstopIds;

            _busController = new BusController(routeIds, bstopIds, this);

            // Initialize SetupController and subscribe to the SettingsSaved event
            _setupForm = new FrmSetup();
            _setupForm.SettingsSaved += SetupForm_SettingsSaved;

            // Use asynchronous method to update labels initially
            LoadInitialDataAsync();

            // Initialize labels for displaying bus route, arrival time, remaining stops, and congestion level
            routeLabels = new List<System.Windows.Forms.Label> { labelBusRoute1, labelBusRoute2, labelBusRoute3, labelBusRoute4, labelBusRoute5, labelBusRoute6 };
            arrivalTimeLabels = new List<System.Windows.Forms.Label> { labelArrivalTime1, labelArrivalTime2, labelArrivalTime3, labelArrivalTime4, labelArrivalTime5, labelArrivalTime6 };
            stopCountLabels = new List<System.Windows.Forms.Label> { labelStopCount1, labelStopCount2, labelStopCount3, labelStopCount4, labelStopCount5, labelStopCount6 };
            secondArrivalTimeLabels = new List<System.Windows.Forms.Label> { labelSecondArrivalTime1, labelSecondArrivalTime2, labelSecondArrivalTime3, labelSecondArrivalTime4, labelSecondArrivalTime5, labelSecondArrivalTime6 };
            secondStopCountLabels = new List<System.Windows.Forms.Label> { labelSecondStopCount1, labelSecondStopCount2, labelSecondStopCount3, labelSecondStopCount4, labelSecondStopCount5, labelSecondStopCount6 };
            congestionLabels = new List<System.Windows.Forms.Label> { labelCongestion1, labelCongestion2, labelCongestion3, labelCongestion4, labelCongestion5, labelCongestion6 };
            secondCongestionLabels = new List<System.Windows.Forms.Label> { labelSecondCongestion1, labelSecondCongestion2, labelSecondCongestion3, labelSecondCongestion4, labelSecondCongestion5, labelSecondCongestion6 };
            destLabels = new List<System.Windows.Forms.Label> { labelRouteDest1, labelRouteDest2, labelRouteDest3, labelRouteDest4, labelRouteDest5, labelRouteDest6 };
            lowPlateLabels = new List<System.Windows.Forms.Label> { labelLowPlate1, labelLowPlate2, labelLowPlate3, labelLowPlate4, labelLowPlate5, labelLowPlate6 };
            secondLowPlateLabels = new List<System.Windows.Forms.Label> { labelSecondLowPlate1, labelSecondLowPlate2, labelSecondLowPlate3, labelSecondLowPlate4, labelSecondLowPlate5, labelSecondLowPlate6 };
            secondBusPanels = new List<Panel> { panelSecondBus1, panelSecondBus2, panelSecondBus3, panelSecondBus4, panelSecondBus5, panelSecondBus6 };

            urgentRouteLabels = new List<System.Windows.Forms.Label> { labelUrgentRoute1, labelUrgentRoute2, labelUrgentRoute3 };
            urgentCongestionLabels = new List<System.Windows.Forms.Label> { labelUrgentCongestion1, labelUrgentCongestion2, labelUrgentCongestion3 };
            timeDiffLabels = new List<System.Windows.Forms.Label> { labelTimeDiff1, labelTimeDiff2, labelTimeDiff3 };


            urgentRouteLabels2 = new List<System.Windows.Forms.Label> { labelUrgentRoute4, labelUrgentRoute5, labelUrgentRoute6 };
            urgentCongestionLabels2 = new List<System.Windows.Forms.Label> { labelUrgentCongestion4, labelUrgentCongestion5, labelUrgentCongestion6 };
            timeDiffLabels2 = new List<System.Windows.Forms.Label> { labelTimeDiff4, labelTimeDiff5, labelTimeDiff6 };

            errorLabels = new List<System.Windows.Forms.Label> { labelError1, labelError2, labelError3, labelError4, labelError5, labelError6 };
            errorPanels = new List<Panel> { panelError1, panelError2, panelError3, panelError4, panelError5, panelError6 };

            _ = StartBackgroundTask();
        }

        private async Task StartBackgroundTask()
        {
            while (true)
            {
                await UpdateBusInfo();
                await Task.Delay(20000);
            }
        }

        private async Task UpdateBusStopLabels()
        {
            string busStop1 = Settings.Default.BusStop1;
            string busStop4 = Settings.Default.BusStop4;

            string labelText1 = await GetBusStopDisplayNameByStationIdAsync(busStop1);
            string labelText4 = await GetBusStopDisplayNameByStationIdAsync(busStop4);

            label5.Text = labelText1;
            label61.Text = labelText4;
        }

        private async Task<string> GetBusStopDisplayNameByStationIdAsync(string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
            {
                return string.Empty;
            }

            string stationInfoUrl = $"https://apis.data.go.kr/6410000/busstationservice/v2/busStationInfov2?serviceKey={serviceKey}&stationId={stationId}&format=xml";
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(stationInfoUrl);

                try
                {
                    // Log the response for debugging purposes
                    Console.WriteLine("API Response: " + response);

                    XElement xml = XElement.Parse(response);
                    var stationElement = xml.Descendants("busStationInfo").FirstOrDefault();

                    if (stationElement != null)
                    {
                        string stationName = stationElement.Element("stationName")?.Value.Trim();
                        string mobileNo = stationElement.Element("mobileNo")?.Value.Trim();
                        return $"{stationName} ({mobileNo})";
                    }
                    else
                    {
                        // Log if the stationElement is null
                        Console.WriteLine("stationElement is null. XML structure might be different than expected.");
                        return "Unknown Bus Stop";
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception for debugging purposes
                    Console.WriteLine("Exception: " + ex.Message);
                    return "Error fetching bus stop data";
                }
            }
        }

        private async void SetupForm_SettingsSaved(object sender, EventArgs e)
        {
            // Update the bus info and labels immediately after settings are saved
            await UpdateBusInfo();
            await UpdateBusStopLabels();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Update the bus arrival information when the form loads
            await UpdateBusInfo();
        }

        private async Task Timer_Tick(object sender, EventArgs e)
        {
            await UpdateBusInfo();
        }

        private async void LoadInitialDataAsync()
        {
            await UpdateBusStopLabels();
        }

        public async Task UpdateBusInfo()
        {
            _routeIds = new List<string> { Settings.Default.BusRouteNo1, Settings.Default.BusRouteNo2, Settings.Default.BusRouteNo3, Settings.Default.BusRouteNo4, Settings.Default.BusRouteNo5, Settings.Default.BusRouteNo6 };
            _bstopIds = new List<string> { Settings.Default.BusStop1, Settings.Default.BusStop2, Settings.Default.BusStop3, Settings.Default.BusStop4, Settings.Default.BusStop5, Settings.Default.BusStop6 };
            _busController = new BusController(_routeIds, _bstopIds, this);

            var busInfoList = await _busController.GetBusInfoAsync();
            DisplayBusInfo(busInfoList);
        }

        private async Task UpdateBusStopLabelsAsync()
        {
            string busStop1 = Settings.Default.BusStop1;
            string busStop4 = Settings.Default.BusStop4;

            string labelText1 = await GetBusStopDisplayNameByStationIdAsync(busStop1);
            string labelText4 = await GetBusStopDisplayNameByStationIdAsync(busStop4);

            label5.Text = labelText1;
            label61.Text = labelText4;
        }

        private void DisplayBusInfo(List<BusInfo> busInfoList)
        {
            var urgentBusInfoList = _busController.GetUrgentBusInfo(busInfoList.GetRange(0, 3));
            var urgentBusInfoList2 = _busController.GetUrgentBusInfo(busInfoList.GetRange(3, 3));

            _ = UpdateBusStopLabelsAsync();

            // Update the original group
            for (int i = 0; i < urgentRouteLabels.Count; i++)
            {
                if (i < urgentBusInfoList.Count)
                {
                    var busInfo = urgentBusInfoList[i];
                    urgentRouteLabels[i].Text = busInfo.RouteName;
                    urgentCongestionLabels[i].Text = _busController.GetCongestionText(busInfo.Crowded1);

                    if (i < timeDiffLabels.Count && busInfo.ArrivalTime2.HasValue)
                    {
                        int timeDiff = busInfo.ArrivalTime2.Value - busInfo.ArrivalTime1.Value;
                        timeDiffLabels[i].Text = "다음 " + timeDiff + " 분 후 도착";
                    }
                    else
                    {
                        timeDiffLabels[i].Text = "";
                    }
                }
                else
                {
                    urgentRouteLabels[i].Text = "";
                    if (i < timeDiffLabels.Count)
                    {
                        timeDiffLabels[i].Text = "";
                        urgentCongestionLabels[i].Text = "";
                    }
                }
            }

            // Update the new group
            for (int i = 0; i < urgentRouteLabels2.Count; i++)
            {
                if (i < urgentBusInfoList2.Count)
                {
                    var busInfo = urgentBusInfoList2[i];
                    urgentRouteLabels2[i].Text = busInfo.RouteName;
                    urgentCongestionLabels2[i].Text = _busController.GetCongestionText(busInfo.Crowded1);

                    if (i < timeDiffLabels2.Count && busInfo.ArrivalTime2.HasValue)
                    {
                        int timeDiff = busInfo.ArrivalTime2.Value - busInfo.ArrivalTime1.Value;
                        timeDiffLabels2[i].Text = "다음 " + timeDiff + " 분 후 도착";
                    }
                    else
                    {
                        timeDiffLabels2[i].Text = "";
                    }
                }
                else
                {
                    urgentRouteLabels2[i].Text = "";
                    if (i < timeDiffLabels2.Count)
                    {
                        timeDiffLabels2[i].Text = "";
                        urgentCongestionLabels2[i].Text = "";
                    }
                }
            }

            for (int i = 0; i < busInfoList.Count; i++)
            {
                var busInfo = busInfoList[i];

                if (i < errorPanels.Count)
                {
                    if (!string.IsNullOrEmpty(busInfo.ErrorMessage))
                    {
                        errorPanels[i].Visible = true;
                        errorLabels[i].Text = busInfo.ErrorMessage;
                    }
                    else
                    {
                        errorPanels[i].Visible = false;
                        errorLabels[i].Text = "";
                    }
                }

                if (busInfo.ArrivalTime1.HasValue)
                {
                    if (i < routeLabels.Count) routeLabels[i].Text = busInfo.RouteName;
                    if (i < destLabels.Count) destLabels[i].Text = busInfo.DestName + "행";
                    if (i < arrivalTimeLabels.Count) arrivalTimeLabels[i].Text = busInfo.ArrivalTime1.HasValue ? busInfo.ArrivalTime1.Value + "" : string.Empty;
                    if (i < stopCountLabels.Count) stopCountLabels[i].Text = busInfo.StopCount1 + "";
                    if (i < congestionLabels.Count) congestionLabels[i].Text = _busController.GetCongestionText(busInfo.Crowded1);
                    if (i < lowPlateLabels.Count) lowPlateLabels[i].Text = _busController.GetLowPlateText(busInfo.LowPlate1);
                }
                else
                {
                    errorPanels[i].Visible = true;
                    errorLabels[i].Text = "";
                    if (i < arrivalTimeLabels.Count) arrivalTimeLabels[i].Text = string.Empty;
                    if (i < stopCountLabels.Count) stopCountLabels[i].Text = string.Empty;
                    if (i < congestionLabels.Count) congestionLabels[i].Text = string.Empty;
                    if (i < lowPlateLabels.Count) lowPlateLabels[i].Text = string.Empty;
                }

                if (busInfo.ArrivalTime2.HasValue)
                {
                    secondBusPanels[i].Visible = false;
                    if (i < secondArrivalTimeLabels.Count) secondArrivalTimeLabels[i].Text = busInfo.ArrivalTime2.Value + "";
                    if (i < secondStopCountLabels.Count) secondStopCountLabels[i].Text = busInfo.StopCount2 + "";
                    if (i < secondCongestionLabels.Count) secondCongestionLabels[i].Text = _busController.GetCongestionText(busInfo.Crowded2);
                    if (i < secondLowPlateLabels.Count) secondLowPlateLabels[i].Text = _busController.GetLowPlateText(busInfo.LowPlate2);
                }
                else
                {
                    secondBusPanels[i].Visible = true;
                    if (i < secondArrivalTimeLabels.Count) secondArrivalTimeLabels[i].Text = string.Empty;
                    if (i < secondStopCountLabels.Count) secondStopCountLabels[i].Text = string.Empty;
                    if (i < secondCongestionLabels.Count) secondCongestionLabels[i].Text = string.Empty;
                    if (i < secondLowPlateLabels.Count) secondLowPlateLabels[i].Text = string.Empty;
                }
            }
        }

        private void CurrentTimeTimer_Tick(object sender, EventArgs e)
        {
            labelCurTime.Text = DateTime.Now.ToShortTimeString();
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            OpenSettingsDialog();
        }

        private void OpenSettingsDialog()
        {
            using (FrmSetup setupForm = new FrmSetup())
            {
                setupForm.ShowDialog(this); // Show FrmSetup as a dialog
            }
        }

        private void InitializeCurrentTimeTimer()
        {
            _currentTimeTimer = new Timer();
            _currentTimeTimer.Interval = 1000; // 1 second
            _currentTimeTimer.Tick += CurrentTimeTimer_Tick;
            _currentTimeTimer.Start();
        }
    }
}

