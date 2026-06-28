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

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://stixtweaks.com/");
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/StixPcServices1");
        }

        private void guna2PictureBox4_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/stix");
        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            Process.Start("https://x.com/StixTweaks");
        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {
            Process.Start("https://x.com/StixTweaks");
        }
    }
}
