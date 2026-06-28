using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix.Design.User_Controls
{
    public partial class SetupPanel : UserControl
    {
        public SetupPanel()
        {
            InitializeComponent();
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                var file = Path.Combine(Path.GetTempPath(), "vc_redist.x64.exe");
                client.DownloadFile("https://aka.ms/vs/17/release/vc_redist.x64.exe", file);
                Process.Start(file);
            }
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                var scope = new ManagementScope("\\\\localhost\\root\\default");
                var path = new ManagementPath("SystemRestore");
                var options = new ObjectGetOptions();
                var restoreClass = new ManagementClass(scope, path, options);

                var parameters = restoreClass.GetMethodParameters("CreateRestorePoint");
                parameters["Description"] = "Stix Free Tweaking Utility";
                parameters["RestorePointType"] = 12;
                parameters["EventType"] = 100;

                restoreClass.InvokeMethod("CreateRestorePoint", parameters, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}