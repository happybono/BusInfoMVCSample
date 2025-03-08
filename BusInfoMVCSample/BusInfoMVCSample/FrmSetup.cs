using System;
using System.Collections.Generic;
using System.Windows.Forms;
using BusInfoMVCSample.Controllers;
using BusInfoMVCSample.Models;
using BusInfoMVCSample.Views;

namespace BusInfoMVCSample
{
    public partial class FrmSetup : Form
    {
        public event EventHandler SettingsSaved;
        public List<string> RouteIds { get; private set; } = new List<string>();
        public List<string> BstopIds { get; private set; } = new List<string>();

        public FrmSetup()
        {
            InitializeComponent();
            var controller = new SetupController(this);
            buttonSave.Click += btnSave_Click;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Trigger the save process and close
            this.DialogResult = DialogResult.OK;
            this.Close();

            SettingsSaved?.Invoke(this, EventArgs.Empty);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            // Close the setup form
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBoxBusStop1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button1.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxBusStop2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button2.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxBusStop3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button3.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxBusStop4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button4.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxBusStop5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button5.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxBusStop6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6.PerformClick();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
