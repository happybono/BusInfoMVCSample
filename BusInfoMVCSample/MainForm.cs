using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using System.Windows.Forms;
using BusInfoMVCSample.Controllers;
using BusInfoMVCSample.Models;

namespace BusInfoMVCSample.Views
{
    public partial class MainForm : Form
    {
        private Timer _timer;
        private Timer _currentTimeTimer;

        private List<Label> routeLabels;
        private List<Label> destLabels;
        private List<Label> arrivalTimeLabels;
        private List<Label> stopCountLabels;
        private List<Label> secondArrivalTimeLabels;
        private List<Label> secondStopCountLabels;
        private List<Label> congestionLabels;
        private List<Label> urgentCongestionLabels;
        private List<Label> secondCongestionLabels;
        private List<Label> lowPlateLabels;
        private List<Label> secondLowPlateLabels;
        private List<Label> urgentRouteLabels;
        private List<Label> timeDiffLabels;
        private List<Label> errorLabels;
        private List<Panel> errorPanels;
        private List<Panel> secondBusPanels;

        private BusController _busController;

        public MainForm()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeCurrentTimeTimer();

            _busController = new BusController();

            // Initialize labels for displaying bus route, arrival time, remaining stops, and congestion level
            routeLabels = new List<Label> { labelBusRoute1, labelBusRoute2, labelBusRoute3 };
            arrivalTimeLabels = new List<Label> { labelArrivalTime1, labelArrivalTime2, labelArrivalTime3 };
            stopCountLabels = new List<Label> { labelStopCount1, labelStopCount2, labelStopCount3 };
            secondArrivalTimeLabels = new List<Label> { labelSecondArrivalTime1, labelSecondArrivalTime2, labelSecondArrivalTime3 };
            secondStopCountLabels = new List<Label> { labelSecondStopCount1, labelSecondStopCount2, labelSecondStopCount3 };
            congestionLabels = new List<Label> { labelCongestion1, labelCongestion2, labelCongestion3 };
            secondCongestionLabels = new List<Label> { labelSecondCongestion1, labelSecondCongestion2, labelSecondCongestion3 };
            destLabels = new List<Label> { labelRouteDest1, labelRouteDest2, labelRouteDest3 };
            lowPlateLabels = new List<Label> {labelLowPlate1, labelLowPlate2,  labelLowPlate3};
            secondLowPlateLabels = new List<Label> { labelSecondLowPlate1, labelSecondLowPlate2, labelSecondLowPlate3 };
            secondBusPanels = new List <Panel> { panelSecondBus1, panelSecondBus2, panelSecondBus3 };

            urgentRouteLabels = new List<Label> { labelUrgentRoute1, labelUrgentRoute2, labelUrgentRoute3};
            urgentCongestionLabels = new List<Label> { labelUrgentCongestion1, labelUrgentCongestion2, labelUrgentCongestion3 };
            timeDiffLabels = new List<Label> { labelTimeDiff1, labelTimeDiff2, labelTimeDiff3 };

            errorLabels = new List<Label> { labelError1, labelError2, labelError3 };
            errorPanels = new List<Panel> { panelError1, panelError2,  panelError3 };
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            // Update the bus arrival information when the form loads
            await UpdateBusInfo();
        }

        private void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Interval = 20000; // 20 seconds in milliseconds
            _timer.Tick += async (s, e) => await Timer_Tick(s, e);
            _timer.Start();
        }


        private async Task Timer_Tick(object sender, EventArgs e)
        {
            await UpdateBusInfo();
        }

        private async Task UpdateBusInfo()
        {
            var busInfoList = await _busController.GetBusInfoAsync();
            DisplayBusInfo(busInfoList);
        }

        private void DisplayBusInfo(List<BusInfo> busInfoList)
        {
            var urgentBusInfoList = _busController.GetUrgentBusInfo(busInfoList);

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
                        timeDiffLabels[i].Text = "다음 " + timeDiff + "분 후 도착";
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
                    if (i < arrivalTimeLabels.Count) arrivalTimeLabels[i].Text = busInfo.ArrivalTime1.HasValue ? busInfo.ArrivalTime1.Value + " 분" : string.Empty;
                    if (i < stopCountLabels.Count) stopCountLabels[i].Text = busInfo.StopCount1 + " 남음";
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
                    if (i < secondArrivalTimeLabels.Count) secondArrivalTimeLabels[i].Text = busInfo.ArrivalTime2.Value + " 분";
                    if (i < secondStopCountLabels.Count) secondStopCountLabels[i].Text = busInfo.StopCount2 + " 남음";
                    if (i < secondCongestionLabels.Count) secondCongestionLabels[i].Text = _busController.GetCongestionText(busInfo.Crowded2);
                    if (i < secondLowPlateLabels.Count) secondLowPlateLabels[i].Text = _busController.GetLowPlateText(busInfo.LowPlate2);
                }
                else
                {
                    secondBusPanels[i].Visible = true;
                    if (i < secondArrivalTimeLabels.Count) secondArrivalTimeLabels[i].Text = string.Empty;
                    if (i < secondStopCountLabels.Count) secondStopCountLabels[i].Text = string.Empty;
                    if (i < secondCongestionLabels.Count) secondCongestionLabels[i].Text = string.Empty;
                    if (i < lowPlateLabels.Count) secondLowPlateLabels[i].Text = string.Empty;
                }
            }
        }
        
        private void InitializeCurrentTimeTimer()
        {
            _currentTimeTimer = new Timer();
            _currentTimeTimer.Interval = 1000; // 1 second
            _currentTimeTimer.Tick += CurrentTimeTimer_Tick;
            _currentTimeTimer.Start();
        }

        private void CurrentTimeTimer_Tick(object sender, EventArgs e)
        {
            labelCurTime.Text = DateTime.Now.ToShortTimeString();
        }
    }
}
