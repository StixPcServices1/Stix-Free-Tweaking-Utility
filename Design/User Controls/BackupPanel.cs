using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix.Design
{
    public partial class BackupPanel : UserControl
    {
        private ManagementBaseObject[] restorePoints = new ManagementBaseObject[0];

        public BackupPanel()
        {
            InitializeComponent();
            LoadRestorePoints();

            guna2PictureBox1.Click += (s, e) => UseRestorePoint(0);
            guna2PictureBox2.Click += (s, e) => UseRestorePoint(1);
            guna2PictureBox3.Click += (s, e) => UseRestorePoint(2);
            guna2PictureBox4.Click += (s, e) => UseRestorePoint(3);
        }

        private void LoadRestorePoints()
        {
            try
            {
                label2.Text = label3.Text = label4.Text = label5.Text = "";
                label6.Text = label7.Text = label8.Text = label9.Text = "";

                var searcher = new ManagementObjectSearcher("root\\default", "SELECT Description, CreationTime FROM SystemRestore");
                var results = searcher.Get();

                restorePoints = new ManagementBaseObject[results.Count];
                results.CopyTo(restorePoints, 0);

                Array.Sort(restorePoints, (x, y) => DateTime.Compare(
                    ManagementDateTimeConverter.ToDateTime(y["CreationTime"].ToString()),
                    ManagementDateTimeConverter.ToDateTime(x["CreationTime"].ToString())));

                Label[] names = { label2, label3, label4, label5 };
                Label[] dates = { label6, label7, label8, label9 };

                for (int i = 0; i < Math.Min(restorePoints.Length, 4); i++)
                {
                    names[i].Text = restorePoints[i]["Description"].ToString();
                    dates[i].Text = ManagementDateTimeConverter.ToDateTime(restorePoints[i]["CreationTime"].ToString()).ToString("g");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading restore points: " + ex.Message);
            }
        }

        private void UseRestorePoint(int index)
        {
            if (restorePoints.Length == 0 || index >= restorePoints.Length)
            {
                MessageBox.Show("No restore points available");
                return;
            }

            var point = restorePoints[index];
            string name = point["Description"].ToString();
            string time = ManagementDateTimeConverter.ToDateTime(point["CreationTime"].ToString()).ToString("g");

            var confirm = new Form();
            confirm.Text = "Confirm Restore";
            confirm.Size = new Size(300, 150);
            confirm.FormBorderStyle = FormBorderStyle.FixedDialog;
            confirm.StartPosition = FormStartPosition.CenterParent;
            confirm.BackColor = Color.FromArgb(22, 22, 22);
            confirm.ForeColor = Color.White;
            confirm.MaximizeBox = false;
            confirm.MinimizeBox = false;

            var message = new Label();
            message.Text = $"Restore to:\n{name}\n{time}";
            message.Font = new Font("Poppins", 11);
            message.Dock = DockStyle.Fill;
            message.TextAlign = ContentAlignment.MiddleCenter;

            var buttonPanel = new Panel();
            buttonPanel.Dock = DockStyle.Bottom;
            buttonPanel.Height = 45;

            var restoreBtn = new Button();
            restoreBtn.Text = "Restore";
            restoreBtn.Size = new Size(80, 30);
            restoreBtn.Location = new Point(60, 5);
            restoreBtn.ForeColor = Color.White;
            restoreBtn.DialogResult = DialogResult.Yes;

            var cancelBtn = new Button();
            cancelBtn.Text = "Cancel";
            cancelBtn.Size = new Size(80, 30);
            cancelBtn.Location = new Point(160, 5);
            cancelBtn.ForeColor = Color.White;
            cancelBtn.DialogResult = DialogResult.No;

            buttonPanel.Controls.Add(restoreBtn);
            buttonPanel.Controls.Add(cancelBtn);
            confirm.Controls.Add(message);
            confirm.Controls.Add(buttonPanel);

            if (confirm.ShowDialog() == DialogResult.Yes)
            {
                try
                {
                    Process.Start("rstrui.exe", "/restore");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }

            confirm.Dispose();
        }

        private void BackupPanel_Load(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
