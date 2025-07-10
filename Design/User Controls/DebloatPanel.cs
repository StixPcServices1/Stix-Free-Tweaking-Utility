using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix.Design
{
    public partial class DebloatPanel : UserControl
    {
        public DebloatPanel()
        {
            InitializeComponent();
        }

        private void DebloatPanel_Load(object sender, EventArgs e)
        {

        }
        public static string Execute(string command)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = "/c " + command;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    return output;
                }
            }
            catch
            {
                return "";
            }
        }
        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            string url = "https:download.sysinternals.com/files/Autoruns.zip";
            string tempDir = Path.GetTempPath();
            string zipPath = Path.Combine(tempDir, "Autoruns.zip");
            string extractPath = Path.Combine(tempDir, "Autoruns");

            if (!Directory.Exists(extractPath))
                Directory.CreateDirectory(extractPath);

            using (WebClient client = new WebClient())
                client.DownloadFile(url, zipPath);

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    string fullPath = Path.Combine(extractPath, entry.FullName);
                    if (string.IsNullOrEmpty(entry.Name))
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                    else
                        entry.ExtractToFile(fullPath, true);
                }
            }

            string autorunsPath = Directory.GetFiles(extractPath, "Autoruns.exe", SearchOption.AllDirectories)[0];

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = autorunsPath,
                Verb = "runas",
                UseShellExecute = true
            };

            Process.Start(psi);
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            Execute("sc config vmicrdv start= disabled >nul 2>&1");
            Execute("sc config vmicvss start= disabled >nul 2>&1");
            Execute("sc config vmicshutdown start= disabled >nul 2>&1");
            Execute("sc config vmicheartbeat start= disabled >nul 2>&1");
            Execute("sc config vmicvmsession start= disabled >nul 2>&1");
            Execute("sc config vmictimesync start= disabled >nul 2>&1");
            Execute("sc config vmicguestinterface start= disabled >nul 2>&1");
            Execute("sc config vmickvpexchange start= disabled >nul 2>&1");
            Execute("dism /online /disable-feature /featurename:Microsoft-Hyper-V-All /quiet /norestart >nul 2>&1");
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            Execute("sc config BTAGService start= disabled >nul 2>&1");
            Execute("sc config bthserv start= disabled >nul 2>&1");
        }

        private void guna2ToggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            Execute("PowerShell -NoProfile -Command \"Get-WindowsPackage -Online | Where-Object PackageName -like '*Hello-Face*' | Remove-WindowsPackage -Online -NoRestart\"");
            Execute("PowerShell -NoProfile -Command \"Get-WindowsPackage -Online | Where-Object PackageName -like '*QuickAssist*' | Remove-WindowsPackage -Online -NoRestart\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Disney.37853FC22B2CE' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.549981C3F5F10' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.BingNews' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.BingWeather' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.GetHelp' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Getstarted' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MSPaint' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Microsoft3DViewer' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MicrosoftOfficeHub' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MicrosoftSolitaireCollection' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MicrosoftStickyNotes' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Office.OneNote' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.OneDriveSync' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.People' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.PowerAutomateDesktop' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.SkypeApp' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Todos' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Wallet' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsAlarms' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsCamera' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsFeedbackHub' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsMaps' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsSoundRecorder' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.YourPhone' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.ZuneMusic' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.ZuneVideo' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'MicrosoftCorporationII.QuickAssist' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'MicrosoftTeams' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'MicrosoftWindows.Client.WebExperience' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'SpotifyAB.SpotifyMusic' | Remove-AppxPackage\"");
            Execute("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsCommunicationsApps' | Remove-AppxPackage\"");
        }

        private void guna2ToggleSwitch5_CheckedChanged(object sender, EventArgs e)
        {
            Execute("sc config RemoteRegistry start= disabled >nul 2>&1");
            Execute("sc config RemoteAccess start= disabled >nul 2>&1");
            Execute("sc config WinRM start= disabled >nul 2>&1");
            Execute("sc config RmSvc start= disabled >nul 2>&1");
            Execute("sc config PrintNotify start= disabled >nul 2>&1");
            Execute("sc config SysMain start= disabled >nul 2>&1");
            Execute("sc config WSearch start= disabled >nul 2>&1");
            Execute("sc config Spooler start= disabled >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\Printing\\EduPrintProv\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\Printing\\PrinterCleanupTask\" /Disable >nul 2>&1");
        }

        private void guna2ToggleSwitch6_CheckedChanged(object sender, EventArgs e)
        {
            Execute("sc config ClipSVC start= disabled >nul 2>&1");
            Execute("sc config BITS start= disabled >nul 2>&1");
            Execute("sc config InstallService start= disabled >nul 2>&1");
            Execute("sc config uhssvc start= disabled >nul 2>&1");
            Execute("sc config UsoSvc start= disabled >nul 2>&1");
            Execute("sc config wuauserv start= disabled >nul 2>&1");
            Execute("sc config LanmanServer start= disabled >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\DoSvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\InstallService\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\UsoSvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\wuauserv\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WaaSMedicSvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\BITS\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\upfc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\uhssvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\ossrs\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DeferUpdatePeriod\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DeferUpgrade\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DeferUpgradePeriod\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DisableWindowsUpdateAccess\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\ScanForUpdates\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\ScanForUpdatesAsUser\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\SmartRetry\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\WakeUpAndContinueUpdates\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\WakeUpAndScanForUpdates\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\Report policies\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\Schedule Scan\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\Schedule Scan Static Task\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\UpdateModelTask\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\USO_UxBroker\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\WaaSMedic\\PerformRemediation\" /Disable >nul 2>&1");
            Execute("schtasks /Change /TN \"Microsoft\\Windows\\WindowsUpdate\\Scheduled Start\" /Disable >nul 2>&1");
        }
    }
}
