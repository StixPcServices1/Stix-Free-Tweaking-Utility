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

namespace Stix.Design
{
    public partial class HomePanel : UserControl
    {
        private SystemInfo systemInfo;

        public HomePanel()
        {
            InitializeComponent();
            InitializeSystemInfo();
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

        private void InitializeSystemInfo()
        {
            systemInfo = new SystemInfo(
                cpulabel, label4, label5,
                gpulabel, label6, label7,
                label12, label10, label11);
        }

        private void HomePanel_Load(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox5_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/StixPcServices1/Stix-Free-Tweaking-Utility");
        }
    }
}
