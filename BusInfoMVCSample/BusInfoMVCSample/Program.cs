using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BusInfoMVCSample.Properties;
using BusInfoMVCSample.Views;

namespace BusInfoMVCSample
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!AreSettingsSaved())
            {
                ShowSetupForm();
            }

            if (AreSettingsSaved())
            {
                OpenMainForm();
            }
            else
            {
                MessageBox.Show("Setup was not completed correctly.", "Setup Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }

        static bool AreSettingsSaved()
        {
            return !string.IsNullOrEmpty(Settings.Default.BusStop1) &&
                   !string.IsNullOrEmpty(Settings.Default.BusStop2) &&
                   !string.IsNullOrEmpty(Settings.Default.BusStop3) &&
                   !string.IsNullOrEmpty(Settings.Default.BusStop4) &&
                   !string.IsNullOrEmpty(Settings.Default.BusStop5) &&
                   !string.IsNullOrEmpty(Settings.Default.BusStop6) &&
                   !string.IsNullOrEmpty(Settings.Default.BusRouteNo1) &&
                   !string.IsNullOrEmpty(Settings.Default.BusRouteNo2) &&
                   !string.IsNullOrEmpty(Settings.Default.BusRouteNo3) &&
                   !string.IsNullOrEmpty(Settings.Default.BusRouteNo4) &&
                   !string.IsNullOrEmpty(Settings.Default.BusRouteNo5) &&
                   !string.IsNullOrEmpty(Settings.Default.BusRouteNo6);
        }

        static void ShowSetupForm()
        {
            using (var setupForm = new FrmSetup())
            {
                if (setupForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

        static void OpenMainForm()
        {
            var routeIds = new List<string>
            {
                Settings.Default.BusRouteNo1,
                Settings.Default.BusRouteNo2,
                Settings.Default.BusRouteNo3,
                Settings.Default.BusRouteNo4,
                Settings.Default.BusRouteNo5,
                Settings.Default.BusRouteNo6
            };

            var bstopIds = new List<string>
            {
                Settings.Default.BusStop1,
                Settings.Default.BusStop2,
                Settings.Default.BusStop3,
                Settings.Default.BusStop4,
                Settings.Default.BusStop5,
                Settings.Default.BusStop6
            };

            Application.Run(new MainForm(routeIds, bstopIds));
        }
    }
}
