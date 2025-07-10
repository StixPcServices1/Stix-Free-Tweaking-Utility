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
        private ManagementBaseObject[] _restorePoints = Array.Empty<ManagementBaseObject>();
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
                ClearLabels();

                using (var searcher = new ManagementObjectSearcher(
                    "root\\default",
                    "SELECT Description, CreationTime FROM SystemRestore"))
                using (var results = searcher.Get())
                {
                    _restorePoints = new ManagementBaseObject[results.Count];
                    results.CopyTo(_restorePoints, 0);

                    Array.Sort(_restorePoints, (x, y) =>
                        DateTime.Compare(
                            ManagementDateTimeConverter.ToDateTime(y["CreationTime"]?.ToString() ?? ""),
                            ManagementDateTimeConverter.ToDateTime(x["CreationTime"]?.ToString() ?? "")));

                    Label[] nameLabels = { label2, label3, label4, label5 };
                    Label[] dateLabels = { label6, label7, label8, label9 };

                    for (int i = 0; i < Math.Min(_restorePoints.Length, 4); i++)
                    {
                        nameLabels[i].Text = _restorePoints[i]["Description"]?.ToString() ?? "N/A";
                        dateLabels[i].Text = ManagementDateTimeConverter.ToDateTime(
                            _restorePoints[i]["CreationTime"]?.ToString() ?? "").ToString("g");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading restore points: {ex.Message}");
            }
        }

        private void ClearLabels()
        {
            foreach (var lbl in new[] { label2, label3, label4, label5, label6, label7, label8, label9 })
            {
                lbl.Text = string.Empty;
            }
        }
        private void UseRestorePoint(int index)
        {
            if (_restorePoints.Length == 0 || index >= _restorePoints.Length)
            {
                MessageBox.Show("No restore points loaded", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var point = _restorePoints[index];
            string name = point["Description"]?.ToString() ?? "Unnamed";
            string time = ManagementDateTimeConverter.ToDateTime(point["CreationTime"]?.ToString() ?? "").ToString("g");

            using (var confirm = new Form
            {
                Text = "Confirm Restore",
                ClientSize = new Size(300, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(22, 22, 22), 
                ForeColor = Color.White, 
                MinimizeBox = false,
                MaximizeBox = false
            })
            {
                var lblMessage = new Label
                {
                    Text = $"Restore to:\n{name}\n{time}",
                    Font = new Font("Poppins", 11), 
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                var panel = new Panel { Dock = DockStyle.Bottom, Height = 45 };

                var btnYes = new Button
                {
                    Text = "Restore",
                    DialogResult = DialogResult.Yes,
                    Size = new Size(80, 30),
                    Location = new Point(60, 5),
                    //BackColor = Color.FromArgb(70, 70, 70),
                    ForeColor = Color.White
                };

                var btnNo = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.No,
                    Size = new Size(80, 30),
                    Location = new Point(160, 5),
                    //BackColor = Color.FromArgb(70, 70, 70),
                    ForeColor = Color.White
                };

                panel.Controls.Add(btnYes);
                panel.Controls.Add(btnNo);
                confirm.Controls.Add(lblMessage);
                confirm.Controls.Add(panel);

                confirm.AcceptButton = btnYes;
                confirm.CancelButton = btnNo;

                if (confirm.ShowDialog() == DialogResult.Yes)
                {
                    try
                    {
                        Process.Start("rstrui.exe", "/restore");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}", "Failed",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void BackupPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
