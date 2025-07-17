using Guna.UI2.WinForms;
using Guna.UI2.WinForms.Suite;
using Stix.Design.User_Controls;
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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix.Design
{
    public partial class Main : Form
    {

        public Main()
        {
            InitializeComponent();
        }

        private async void Main_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            var home = new HomePanel();
            guna2Transition1.ShowSync(HomeBG);
            await DisplayFormInPanelWithoutAnimation(home, HomeBG);

            //Download Resources
            var client = new HttpClient();
            var url = "https://www.dropbox.com/scl/fi/qdgw7wcn7oesd3rbfu883/Stix-Free.zip?rlkey=6ed88ulyityakfpyv7f0que9d&st=6eeunx33&dl=1";
            var tempFile = Path.Combine(Path.GetTempPath(), "temp.zip");
            var response = await client.GetAsync(url);
            using (var fs = File.Create(tempFile))
            {
                await response.Content.CopyToAsync(fs);
            }

            var stixPath = @"C:\Stix Free";
            if (Directory.Exists(stixPath))
            {
                Directory.Delete(stixPath, true);
            }

            ZipFile.ExtractToDirectory(tempFile, @"C:\");
            File.Delete(tempFile);
        }

        private void guna2ControlBox1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        public async Task DisplayFormInPanelWithoutAnimation(UserControl formToDisplay, Panel targetPanel)
        {
            await Task.Run(() =>
            {
                targetPanel.Invoke((Action)(() =>
                {
                    targetPanel.Controls.Clear();
                    formToDisplay.Dock = DockStyle.Fill;
                    targetPanel.Controls.Add(formToDisplay);
                    formToDisplay.Show();
                }));
            });
        }

        public async Task DisplayFormInPanel(UserControl formToDisplay, Panel targetPanel, Guna2Transition transition)
        {
            transition.HideSync(targetPanel);
            await Task.Run(() =>
            {
                targetPanel.Invoke((Action)(() =>
                {
                    targetPanel.Controls.Clear();
                    formToDisplay.Dock = DockStyle.Fill;
                    targetPanel.Controls.Add(formToDisplay);
                    formToDisplay.Show();
                    transition.ShowSync(targetPanel);
                }));
            });
        }
        private async void guna2Button2_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            var home = new HomePanel();
            await DisplayFormInPanel(home, HomeBG, guna2Transition1);
        }

        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            var home = new BackupPanel();
            await DisplayFormInPanel(home, HomeBG, guna2Transition1);
        }

        private async void guna2Button4_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            var home = new TweaksPanel();
            await DisplayFormInPanel(home, HomeBG, guna2Transition1);
        }

        private async void guna2Button3_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            var home = new NetworkPanel();
            await DisplayFormInPanel(home, HomeBG, guna2Transition1);
        }

        private async void guna2Button5_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            var home = new DebloatPanel();
            await DisplayFormInPanel(home, HomeBG, guna2Transition1);
        }

        private async void guna2Button7_Click(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;

            var home = new SettingsPanel();
            await DisplayFormInPanel(home, HomeBG, guna2Transition1);
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {

        }
    }
}
