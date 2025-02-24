using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Linq;
using BusInfoMVCSample.Models;
using BusInfoMVCSample.Properties;
using BusInfoMVCSample.Views;

namespace BusInfoMVCSample.Controllers
{
    public class SetupController
    {
        private readonly FrmSetup _view;
        private readonly string serviceKey = Resources.ServiceKey.ToString();
        // Define an event to notify when settings are saved
        public event EventHandler SettingsSaved;

        public SetupController(FrmSetup view)
        {
            _view = view;
            _view.Load += FrmSetup_Load;
            _view.buttonSave.Click += ButtonSave_Click;
            _view.button1.Click += async (s, e) => await SearchBusStopsAsync(_view.textBoxBusStop1.Text, _view.comboBoxBusStops1);
            _view.button2.Click += async (s, e) => await SearchBusStopsAsync(_view.textBoxBusStop2.Text, _view.comboBoxBusStops2);
            _view.button3.Click += async (s, e) => await SearchBusStopsAsync(_view.textBoxBusStop3.Text, _view.comboBoxBusStops3);
            _view.button4.Click += async (s, e) => await SearchBusStopsAsync(_view.textBoxBusStop4.Text, _view.comboBoxBusStops4);
            _view.button5.Click += async (s, e) => await SearchBusStopsAsync(_view.textBoxBusStop5.Text, _view.comboBoxBusStops5);
            _view.button6.Click += async (s, e) => await SearchBusStopsAsync(_view.textBoxBusStop6.Text, _view.comboBoxBusStops6);
            _view.comboBoxBusStops1.SelectedIndexChanged += async (s, e) => await UpdateRoutesAsync(_view.comboBoxBusStops1, _view.comboBoxRoutes1);
            _view.comboBoxBusStops2.SelectedIndexChanged += async (s, e) => await UpdateRoutesAsync(_view.comboBoxBusStops2, _view.comboBoxRoutes2);
            _view.comboBoxBusStops3.SelectedIndexChanged += async (s, e) => await UpdateRoutesAsync(_view.comboBoxBusStops3, _view.comboBoxRoutes3);
            _view.comboBoxBusStops4.SelectedIndexChanged += async (s, e) => await UpdateRoutesAsync(_view.comboBoxBusStops4, _view.comboBoxRoutes4);
            _view.comboBoxBusStops5.SelectedIndexChanged += async (s, e) => await UpdateRoutesAsync(_view.comboBoxBusStops5, _view.comboBoxRoutes5);
            _view.comboBoxBusStops6.SelectedIndexChanged += async (s, e) => await UpdateRoutesAsync(_view.comboBoxBusStops6, _view.comboBoxRoutes6);
            _view.FormClosing += FrmSetup_FormClosing;
        }

        private async void FrmSetup_Load(object sender, EventArgs e)
        {
            await LoadSettingsIntoControlsAsync();
            await PopulateArrivingBusRoutesAsync();

            CountBusStopAndRouteIds();

            // Log the items filled during form load
            LogComboBoxItems(_view.comboBoxBusStops1, "comboBoxBusStops1");
            LogComboBoxItems(_view.comboBoxBusStops2, "comboBoxBusStops2");
            LogComboBoxItems(_view.comboBoxBusStops3, "comboBoxBusStops3");
            LogComboBoxItems(_view.comboBoxBusStops4, "comboBoxBusStops4");
            LogComboBoxItems(_view.comboBoxBusStops5, "comboBoxBusStops5");
            LogComboBoxItems(_view.comboBoxBusStops6, "comboBoxBusStops6");
            LogComboBoxItems(_view.comboBoxRoutes1, "comboBoxRoutes1");
            LogComboBoxItems(_view.comboBoxRoutes2, "comboBoxRoutes2");
            LogComboBoxItems(_view.comboBoxRoutes3, "comboBoxRoutes3");
            LogComboBoxItems(_view.comboBoxRoutes4, "comboBoxRoutes4");
            LogComboBoxItems(_view.comboBoxRoutes5, "comboBoxRoutes5");
            LogComboBoxItems(_view.comboBoxRoutes6, "comboBoxRoutes6");
        }

        private void LogComboBoxItems(ComboBox comboBox, string comboBoxName)
        {
            Console.WriteLine($"{comboBoxName} item count: {comboBox.Items.Count}");
            foreach (var item in comboBox.Items)
            {
                Console.WriteLine($"{comboBoxName} item: {item}");
            }
        }


        private async Task PopulateArrivingBusRoutesAsync()
        {
            await PopulateBusRoutesAsync(_view.comboBoxRoutes1, Properties.Settings.Default.BusStop1);
            await PopulateBusRoutesAsync(_view.comboBoxRoutes2, Properties.Settings.Default.BusStop2);
            await PopulateBusRoutesAsync(_view.comboBoxRoutes3, Properties.Settings.Default.BusStop3);
            await PopulateBusRoutesAsync(_view.comboBoxRoutes4, Properties.Settings.Default.BusStop4);
            await PopulateBusRoutesAsync(_view.comboBoxRoutes5, Properties.Settings.Default.BusStop5);
            await PopulateBusRoutesAsync(_view.comboBoxRoutes6, Properties.Settings.Default.BusStop6);
        }


        private async Task LoadSettingsIntoControlsAsync()
        {
            // Fetch and load mobile numbers into TextBoxes using station IDs
            _view.comboBoxBusStops1.Text = await GetMobileNumberAsync(Properties.Settings.Default.BusStop1);
            _view.comboBoxBusStops2.Text = await GetMobileNumberAsync(Properties.Settings.Default.BusStop2);
            _view.comboBoxBusStops3.Text = await GetMobileNumberAsync(Properties.Settings.Default.BusStop3);
            _view.comboBoxBusStops4.Text = await GetMobileNumberAsync(Properties.Settings.Default.BusStop4);
            _view.comboBoxBusStops5.Text = await GetMobileNumberAsync(Properties.Settings.Default.BusStop5);
            _view.comboBoxBusStops6.Text = await GetMobileNumberAsync(Properties.Settings.Default.BusStop6);

            // Fetch and load bus route names into ComboBoxes using route IDs
            _view.comboBoxRoutes1.Text = await GetBusRouteDisplayNameAsync(Properties.Settings.Default.BusRouteNo1);
            _view.comboBoxRoutes2.Text = await GetBusRouteDisplayNameAsync(Properties.Settings.Default.BusRouteNo2);
            _view.comboBoxRoutes3.Text = await GetBusRouteDisplayNameAsync(Properties.Settings.Default.BusRouteNo3);
            _view.comboBoxRoutes4.Text = await GetBusRouteDisplayNameAsync(Properties.Settings.Default.BusRouteNo4);
            _view.comboBoxRoutes5.Text = await GetBusRouteDisplayNameAsync(Properties.Settings.Default.BusRouteNo5);
            _view.comboBoxRoutes6.Text = await GetBusRouteDisplayNameAsync(Properties.Settings.Default.BusRouteNo6);

            // Populate bus routes based on the initial bus stop selections
            await PopulateArrivingBusRoutesAsync();
        }



        private async Task<string> GetMobileNumberAsync(string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
            {
                return string.Empty;
            }

            string stationInfoUrl = $"https://apis.data.go.kr/6410000/busstationservice/v2/busStationInfov2?serviceKey={serviceKey}&stationId={stationId}&format=xml";
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(stationInfoUrl);
                var busStops = new List<BusStopDisplay>();

                try
                {
                    XElement xml = XElement.Parse(response);
                    var stationElement = xml.Descendants("busStationInfo").FirstOrDefault();

                    if (stationElement != null)
                    {
                        string StationName = stationElement.Element("stationName").Value + " (" + stationElement.Element("mobileNo").Value.Trim() + ")";

                        return $"{StationName}";
                    }
                    else
                    {
                        // Log or handle the case where the element is missing
                        Console.WriteLine($"No busStationInfoItem found in response: {response}");
                        return string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    // Log or handle the exception as needed
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    return string.Empty;
                }
            }
        }


        private async Task<string> GetBusRouteDisplayNameAsync(string routeID)
        {
            if (string.IsNullOrEmpty(routeID))
            {
                return string.Empty;
            }

            string routeName = await GetRouteNameFromFirstAPI(routeID);
            if (routeName == "Unknown Route")
            {
                routeName = await GetRouteNameFromSecondAPI(routeID);
            }

            return routeName;
        }


        private async Task<string> GetRouteNameFromFirstAPI(string routeID)
        {
            string routeInfoUrl = $"https://apis.data.go.kr/6410000/busrouteservice/v2/getBusRouteInfoItemv2?serviceKey={serviceKey}&routeId={routeID}&format=xml";

            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(routeInfoUrl);

                try
                {
                    XElement xml = XElement.Parse(response);
                    var routeElement = xml.Descendants("busRouteInfoItem").FirstOrDefault();

                    if (routeElement != null)
                    {
                        string routeName = routeElement.Element("routeName")?.Value;
                        return routeName ?? "Unknown Route";
                    }
                    else
                    {
                        Console.WriteLine($"No busRouteInfoItem found in response: {response}");
                        return "Unknown Route";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    return "Unknown Route";
                }
            }
        }

        private async Task<string> GetRouteNameFromSecondAPI(string routeID)
        {
            string routeInfoUrl = $"https://apis.data.go.kr/6280000/busRouteService/getBusRouteId?serviceKey={serviceKey}&pageNo=1&numOfRows=10&routeId={routeID}";

            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(routeInfoUrl);

                try
                {
                    XElement xml = XElement.Parse(response);
                    var routeElement = xml.Descendants("itemList").FirstOrDefault();

                    if (routeElement != null)
                    {
                        string routeNo = routeElement.Element("ROUTENO")?.Value;
                        return routeNo ?? "Unknown Route";
                    }
                    else
                    {
                        Console.WriteLine($"No itemList found in response: {response}");
                        return "Unknown Route";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception occurred: {ex.Message}");
                    return "Error fetching route data";
                }
            }
        }



private async Task SearchBusStopsAsync(string keyword, ComboBox comboBox)
        {
            string url = $"https://apis.data.go.kr/6410000/busstationservice/v2/getBusStationListv2?serviceKey={serviceKey}&keyword={keyword}&format=xml";

            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);
            XElement xml = XElement.Parse(response);

            var busStops = new List<BusStopDisplay>();

            foreach (var element in xml.Descendants("busStationList"))
            {
                busStops.Add(new BusStopDisplay
                {
                    StationId = element.Element("stationId").Value,
                    StationName = element.Element("stationName").Value + " (" + element.Element("mobileNo").Value.Trim() + ")"
                });
            }

            comboBox.DataSource = busStops;
            comboBox.DisplayMember = "StationName";
            comboBox.ValueMember = "StationId";
        }

        private async Task UpdateRoutesAsync(ComboBox busStopComboBox, ComboBox routeComboBox)
        {
            if (busStopComboBox.SelectedItem is BusStopDisplay selectedBusStop)
            {
                await PopulateBusRoutesAsync(routeComboBox, selectedBusStop.StationId);
            }
        }

        private async Task PopulateBusRoutesAsync(ComboBox comboBox, string stationId)
        {
            if (string.IsNullOrEmpty(stationId))
            {
                return;
            }

            string url = $"https://apis.data.go.kr/6410000/busarrivalservice/v2/getBusArrivalListv2?serviceKey={serviceKey}&stationId={stationId}&format=xml";
            HttpClient client = new HttpClient();
            string response = await client.GetStringAsync(url);
            XElement xml = XElement.Parse(response);

            var busRoutes = new List<BusRoute>();

            foreach (var element in xml.Descendants("busArrivalList"))
            {
                busRoutes.Add(new BusRoute
                {
                    RouteId = element.Element("routeId")?.Value,
                    RouteName = element.Element("routeName")?.Value
                });
            }

            // Clear existing items and add new items without changing the selected value
            var selectedValue = comboBox.SelectedValue;
            comboBox.Items.Clear();
            foreach (var busRoute in busRoutes)
            {
                comboBox.Items.Add(busRoute);
            }
            comboBox.DisplayMember = "RouteName";
            comboBox.ValueMember = "RouteId";

            // Restore the previously selected value if it exists in the new items
            comboBox.SelectedValue = selectedValue;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            // Ensure lists are cleared before counting and adding new items
            _view.BstopIds.Clear();
            _view.RouteIds.Clear();

            // Count the items when the form loads and any user-made changes
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops1, _view.comboBoxRoutes1, "BusStop1", "BusRouteNo1");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops2, _view.comboBoxRoutes2, "BusStop2", "BusRouteNo2");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops3, _view.comboBoxRoutes3, "BusStop3", "BusRouteNo3");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops4, _view.comboBoxRoutes4, "BusStop4", "BusRouteNo4");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops5, _view.comboBoxRoutes5, "BusStop5", "BusRouteNo5");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops6, _view.comboBoxRoutes6, "BusStop6", "BusRouteNo6");

            // Log lengths and values of lists
            Console.WriteLine($"BstopIds count: {_view.BstopIds.Count}");
            Console.WriteLine($"RouteIds count: {_view.RouteIds.Count}");
            Console.WriteLine($"BstopIds values: {string.Join(", ", _view.BstopIds)}");
            Console.WriteLine($"RouteIds values: {string.Join(", ", _view.RouteIds)}");

            // Save settings
            Properties.Settings.Default.Save();
            
            // Raise the event to notify subscribers (e.g., MainForm)
            SettingsSaved?.Invoke(this, EventArgs.Empty);

            // Close FrmSetup after saving
            _view.DialogResult = DialogResult.OK;
            _view.Close();
        }


        private void FrmSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Raise the event to notify subscribers (e.g., MainForm) when the form is closing
            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }



        private void AddSelectedBusStopAndRoute(ComboBox busStopComboBox, ComboBox routeComboBox, string busStopSettingName, string busRouteSettingName)
{
    if (busStopComboBox.SelectedItem is BusStopDisplay selectedBusStop)
    {
        _view.BstopIds.Add(selectedBusStop.StationId);
        Properties.Settings.Default[busStopSettingName] = selectedBusStop.StationId;
    }
    else
    {
        Console.WriteLine($"No selection in {busStopComboBox.Name}");
    }

    if (routeComboBox.SelectedItem is BusRoute selectedBusRoute)
    {
        _view.RouteIds.Add(selectedBusRoute.RouteId);
        Properties.Settings.Default[busRouteSettingName] = selectedBusRoute.RouteId;
    }
    else
    {
        // Log which route ComboBox didn't have a selection
        Console.WriteLine($"No selection in {routeComboBox.Name}");
    }
}


        private void CountBusStopAndRouteIds()
        {
            _view.BstopIds.Clear();
            _view.RouteIds.Clear();

            AddSelectedBusStopAndRoute(_view.comboBoxBusStops1, _view.comboBoxRoutes1, "BusStop1", "BusRouteNo1");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops2, _view.comboBoxRoutes2, "BusStop2", "BusRouteNo2");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops3, _view.comboBoxRoutes3, "BusStop3", "BusRouteNo3");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops4, _view.comboBoxRoutes4, "BusStop4", "BusRouteNo4");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops5, _view.comboBoxRoutes5, "BusStop5", "BusRouteNo5");
            AddSelectedBusStopAndRoute(_view.comboBoxBusStops6, _view.comboBoxRoutes6, "BusStop6", "BusRouteNo6");

            // Log the counts and values for verification
            Console.WriteLine($"BstopIds count after form load: {_view.BstopIds.Count}");
            Console.WriteLine($"RouteIds count after form load: {_view.RouteIds.Count}");
            Console.WriteLine($"BstopIds values after form load: {string.Join(", ", _view.BstopIds)}");
            Console.WriteLine($"RouteIds values after form load: {string.Join(", ", _view.RouteIds)}");
        }
    }
}
