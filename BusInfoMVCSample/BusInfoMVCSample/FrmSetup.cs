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
    }
}