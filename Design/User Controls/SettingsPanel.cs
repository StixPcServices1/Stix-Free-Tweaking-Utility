using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix.Design.User_Controls
{
    public partial class SettingsPanel : UserControl
    {
        public SettingsPanel()
        {
            InitializeComponent();
        }
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception)
            {

            }
        }
        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            OpenUrl("https://stixtweaks.com/");
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/StixPcServices1");
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            OpenUrl("https://discord.gg/stix");
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            OpenUrl("https://x.com/StixTweaks");
        }
    }
}
