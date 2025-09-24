using Guna.UI2.WinForms;
using Microsoft.Win32;
using Stix_Premium_Optimizer.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix_Premium_Optimizer.Forms
{
    public partial class Loader : Form
    {
        public Loader()
        {
            InitializeComponent();
            DisplaySystemInfo();
            DisplayMemory();
            DisplayStorageDriveInfo();
            DisplayTempFilesSize();
        }

        public static async Task<string> ExecuteAsync(string command)
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + command,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    },
                    EnableRaisingEvents = true
                };

                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();
                process.Dispose();
                return output;
            }
            catch
            {
                return string.Empty;
            }
        }

        private void DisplayTempFilesSize()
        {
            try
            {
                string windowsTempPath = @"C:\Windows\Temp";
                string prefetchPath = @"C:\Windows\Prefetch";

                long totalTempSizeBytes = GetDirectorySize(windowsTempPath)
                                        + GetDirectorySize(prefetchPath);

                double totalTempSizeMB = totalTempSizeBytes / (1024.0 * 1024.0);

                if (totalTempSizeMB < 1024)
                {
                    label27.Text = $"{totalTempSizeMB:F2} MB of Junk Files";
                }
                else
                {
                    label27.Text = $"{(totalTempSizeMB / 1024):F2} GB of Junk Files";
                }
            }
            catch (Exception ex)
            {
                label27.Text = $"Error: {ex.Message}";
            }
        }

        private long GetDirectorySize(string directoryPath)
        {
            long size = 0;
            try
            {
                DirectoryInfo dir = new DirectoryInfo(directoryPath);
                foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
                {
                    size += file.Length;
                }
            }
            catch
            {
            }
            return size;
        }

        private void DisplayStorageDriveInfo()
        {
            try
            {
                string systemDrive = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));

                using (ManagementObjectSearcher partitionSearcher = new ManagementObjectSearcher(
                    "ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + systemDrive.TrimEnd('\\') + "'} WHERE AssocClass=Win32_LogicalDiskToPartition"))
                {
                    foreach (ManagementObject partition in partitionSearcher.Get())
                    {
                        using (ManagementObjectSearcher diskSearcher = new ManagementObjectSearcher(
                            "ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass=Win32_DiskDriveToDiskPartition"))
                        {
                            foreach (ManagementObject disk in diskSearcher.Get())
                            {
                                string model = disk["Model"]?.ToString() ?? "Unknown Model";
                                string shortModel = GetShortModel(model);
                                string manufacturer = GetManufacturer(disk, model);
                                ulong sizeInBytes = (ulong)(disk["Size"] ?? 0);
                                string sizeInGB = (sizeInBytes / (1024 * 1024 * 1024)).ToString() + "GB";

                                label26.Text = $"{manufacturer} {shortModel} {sizeInGB}";
                                return;
                            }
                        }
                    }
                }

                label26.Text = "Windows drive not found";
            }
            catch (Exception ex)
            {
                label26.Text = "Error retrieving drive info";
            }
        }

        private string GetShortModel(string model)
        {
            if (model.Contains("P3")) return "P3";
            if (model.Contains("P5")) return "P5";
            if (model.Contains("EVO")) return "EVO";
            if (model.Contains("PRO")) return "PRO";
            if (model.Contains("BX500")) return "BX500";
            return model;
        }

        private string GetManufacturer(ManagementObject disk, string model)
        {
            string manufacturer = disk["Manufacturer"]?.ToString();

            if (string.IsNullOrEmpty(manufacturer) || manufacturer.Contains("Standard"))
            {
                if (model.Contains("Samsung", StringComparison.OrdinalIgnoreCase)) return "Samsung";
                if (model.Contains("Crucial", StringComparison.OrdinalIgnoreCase) || model.StartsWith("CT", StringComparison.OrdinalIgnoreCase)) return "Crucial";
                if (model.Contains("PNY", StringComparison.OrdinalIgnoreCase)) return "PNY";
                if (model.Contains("Kingston", StringComparison.OrdinalIgnoreCase)) return "Kingston";
                if (model.Contains("Western Digital", StringComparison.OrdinalIgnoreCase)) return "Western Digital";
                if (model.Contains("Seagate", StringComparison.OrdinalIgnoreCase)) return "Seagate";
                return "Unknown Manufacturer";
            }

            return manufacturer;
        }

        private void DisplaySystemInfo()
        {
            label20.Text = GetOSNameAndEdition();
            label21.Text = GetWmiValue("Win32_Processor", "Name");
            label23.Text = GetWmiValue("Win32_VideoController", "Name");
            label24.Text = GetMotherboardInfo();
            label25.Text = UpdateNetworkInfo();
        }

        private void DisplayMemory()
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    ulong totalMemoryBytes = (ulong)obj["TotalPhysicalMemory"];
                    double totalMemoryMB = totalMemoryBytes / (1024 * 1024);

                    if (totalMemoryMB < 1024)
                    {
                        label22.Text = $"{totalMemoryMB:F2} MB";
                    }
                    else
                    {
                        label22.Text = $"{(totalMemoryMB / 1024):F2} GB";
                    }
                }
            }
            catch (Exception ex)
            {
                label22.Text = $"Error: {ex.Message}";
            }
        }

        private string GetMotherboardInfo()
        {
            string manufacturer = GetWmiValue("Win32_BaseBoard", "Manufacturer");
            string product = GetWmiValue("Win32_BaseBoard", "Product");

            return $"{GetShortManufacturerName(manufacturer)} {product}";
        }

        private string GetShortManufacturerName(string manufacturer)
        {
            if (string.IsNullOrEmpty(manufacturer)) return "Unknown";

            return manufacturer.ToUpper() switch
            {
                var m when m.Contains("MICRO-STAR") || m.Contains("MSI") => "MSI",
                var m when m.Contains("ASUSTEK") || m.Contains("ASUS") => "ASUS",
                var m when m.Contains("GIGABYTE") => "Gigabyte",
                var m when m.Contains("ASROCK") => "ASRock",
                var m when m.Contains("INTEL") => "Intel",
                var m when m.Contains("DELL") => "Dell",
                var m when m.Contains("HP") || m.Contains("HEWLETT") => "HP",
                var m when m.Contains("LENOVO") => "Lenovo",
                var m when m.Contains("ACER") => "Acer",
                _ => manufacturer
            };
        }

        private string GetWmiValue(string className, string propertyName)
        {
            using (var searcher = new ManagementObjectSearcher($"SELECT {propertyName} FROM {className}"))
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    return obj[propertyName]?.ToString()?.Trim() ?? "Unknown";
                }
            }
            return "Unknown";
        }

        private string GetOSNameAndEdition()
        {
            try
            {
                string edition = "Unknown Edition";
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        edition = key.GetValue("EditionID")?.ToString() ?? "Unknown Edition";
                    }
                }

                string buildNumber = GetWmiValue("Win32_OperatingSystem", "BuildNumber");
                string osVersion = "Windows 10";

                if (int.TryParse(buildNumber, out int build) && build >= 22000)
                {
                    osVersion = "Windows 11";
                }

                if (edition == "Pro") return $"{osVersion} Pro";
                if (edition == "Core") return $"{osVersion} Home";
                return $"{osVersion} {edition}";
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        private string UpdateNetworkInfo()
        {
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            var activeInterface = networkInterfaces
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up)
                .FirstOrDefault();

            if (activeInterface != null)
            {
                string connectionType = activeInterface.NetworkInterfaceType switch
                {
                    NetworkInterfaceType.Ethernet => "Ethernet",
                    NetworkInterfaceType.Wireless80211 => "Wi-Fi",
                    _ => "VPN or Proxy"
                };

                return $"{connectionType} - {activeInterface.Speed / 1_000_000} Mbps";
            }

            return "No active network connection";
        }
        private async void Loader_Load(object sender, EventArgs e)
        {
            DisplayGreeting();

            label13.Text = Environment.UserName;
            label13.BackColor = Color.Transparent;
            label13.AutoSize = false;
            label13.Size = new Size(100, 20);
            label13.TextAlignment = ContentAlignment.MiddleCenter;

            HomePanel.Visible = true;
            TweaksPanel.Visible = false;
            MiscPanel.Visible = false;
            guna2vScrollBar3.Visible = false;

            //Download Resources
            var client = new HttpClient();
            var data = await client.GetByteArrayAsync("https://www.dropbox.com/scl/fi/676aznifyb3oea4hlpvj0/Stix-Free.zip?rlkey=qw6ygc2m4yxdvkyg738j1mi78&st=7s3zrncy&dl=1");
            File.WriteAllBytes("temp.zip", data);
            if (Directory.Exists(@"C:\Stix Free"))
                Directory.Delete(@"C:\Stix Free", true);
            ZipFile.ExtractToDirectory("temp.zip", @"C:\");
            File.Delete("temp.zip");
        }

        private void DisplayGreeting()
        {
            DateTime currentTime = DateTime.Now;

            string greeting;
            if (currentTime.Hour >= 18)
            {
                greeting = "Good Night!";
            }
            else if (currentTime.Hour >= 12)
            {
                greeting = "Good Afternoon!";
            }
            else
            {
                greeting = "Good Morning!";
            }

            label11.Text = greeting;

            string username = Environment.UserName ?? "User";

            label12.Text = $"Welcome back, {username}";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void label7_Click(object sender, EventArgs e)
        {
            HomePanel.Visible = false;
            TweaksPanel.Visible = true;
            MiscPanel.Visible = false;
            guna2vScrollBar3.Visible = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            HomePanel.Visible = true;
            TweaksPanel.Visible = false;
            MiscPanel.Visible = false;
            guna2vScrollBar3.Visible = false;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            HomePanel.Visible = false;
            TweaksPanel.Visible = false;
            guna2vScrollBar3.Visible = false;
        }

        private void guna2ToggleSwitch33_CheckedChanged(object sender, EventArgs e)
        {
            BiosMessage.Show();
        }

        private void guna2ToggleSwitch1_CheckedChanged(object? sender, EventArgs e)
        {
            // Disable Dynamic Tick
            ExecuteAsync("bcdedit /set disabledynamictick yes >nul 2>&1");

            // Disable FTH (Fault Tolerant Heap)
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\Software\\Microsoft\\FTH\" /v Enabled /t REG_DWORD /d 0 /f >nul 2>&1");

            // Set SystemResponsiveness & NetworkThrottlingIndex to a (Lowest Value | Default)
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v \"SystemResponsiveness\" /t REG_DWORD /d \"a\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v \"NetworkThrottlingIndex\" /t REG_DWORD /d \"a\" /f >nul 2>&1");

            // Disable IOLatencyCap
            ExecuteAsync("FOR /F \"eol=E\" %%a in ('REG QUERY \"HKLM\\SYSTEM\\CurrentControlSet\\Services\" /S /F \"IoLatencyCap\"^| FINDSTR /V \"IoLatencyCap\"') DO (REG ADD \"%%a\" /F /V \"IoLatencyCap\" /T REG_DWORD /d 0 >nul 2>&1 & FOR /F \"tokens=*\" %%z IN (\"%%a\") DO (SET STR=%%z & SET STR=!STR:HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\=! & SET STR=!STR:\\Parameters=!))");

            // Enable Windows Gamemode
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\GameBar\" /v \"AllowAutoGameMode\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\GameBar\" /v \"AutoGameModeEnabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");

            // Disable StorPortIdle
            ExecuteAsync("for /f \"tokens=*\" %%s in ('reg query \"HKLM\\System\\CurrentControlSet\\Enum\" /S /F \"StorPort\" ^| findstr /e \"StorPort\"') do reg add \"%%s\" /v \"EnableIdlePowerManagement\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable VBS
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\" /v \"EnableVirtualizationBasedSecurity\" /t REG_DWORD /d 0 /f >nul 2>&1");

            // Disable PowerThrottlingOff & Hibernation
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\PowerThrottling\" /v \"PowerThrottlingOff\" /t REG_DWORD /d 1 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"HiberBootEnabled\" /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v \"HibernateEnabled\" /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("powercfg -h off");

            // Disable Storage Sense
            ExecuteAsync("reg add \"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy\" /v \"01\" /t REG_DWORD /d 0 /f >nul 2>&1");

            // Disable Sleep Study
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v \"SleepstudyAccountingEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"SleepStudyDisabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"SleepStudyDeviceAccountingLevel\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable Energy Estimation
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\TaggedEnergy\" /v \"DisableTaggedEnergyLogging\" /t REG_DWORD /d \"1\" /f >nul 2>&1");

            // Set Win32PrioritySeparation
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl\" /v \"Win32PrioritySeparation\" /t REG_DWORD /d 42 /f >nul 2>&1");

            // Disable DMA Remapping
            ExecuteAsync("for %%a in (DmaRemappingCompatible) do for /f \"delims=\" %%b in ('reg query \"HKLM\\SYSTEM\\CurrentControlSet\\Services\" /s /f \"%%a\" ^| findstr \"HKEY\"') do Reg.exe add \"%%b\" /v \"%%a\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            //Disable Windows Updates
            ExecuteAsync("\"C:\\Stix Free\\Wub.exe\"");

            // Classic Right Click Menu
            ExecuteAsync("reg add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /v \"(Default)\" /t REG_SZ /d \"\" /f >nul 2>&1");

            // Use Dark Mode
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize\" /v \"AppsUseLightTheme\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize\" /v \"SystemUsesLightTheme\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Show File Extension
            ExecuteAsync("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\" /v \"HideFileExt\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
        }

        private void guna2ToggleSwitch2_CheckedChanged(object? sender, EventArgs e)
        {
            // Kernel Tweks
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v CoalescingTimerInterval /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v AlwaysTrackIoBoosting /t REG_DWORD /d 10 /f >nul 2>&1");

            // Enable Interrupt Steering
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v InterruptSteeringFlags /t REG_DWORD /d 2 /f >nul 2>&1");

            // Restrict Timer Interrupts to Core 0
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v SerializeTimerExpiration /t REG_DWORD /d 1 /f >nul 2>&1");
        }


        private void guna2ToggleSwitch3_CheckedChanged(object? sender, EventArgs e)
        {
            // Disable Cache Telemetry
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"DisableCacheTelemetry\" /t REG_DWORD /d 1 /f >nul 2>&1");

            // Disable Memory Compression
            ExecuteAsync("powershell \"Disable-MMAgent -MemoryCompression\" >nul 2>&1");
        }


        private void guna2ToggleSwitch5_CheckedChanged(object? sender, EventArgs e)
        {
            // Enable HAGS
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\" /v \"HwSchMode\" /t REG_DWORD /d 2 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch4_CheckedChanged(object? sender, EventArgs e)
        {
            // Disable PlatformClock/Tick
            ExecuteAsync("bcdedit /deletevalue useplatformclock >nul 2>&1");
            ExecuteAsync("bcdedit /deletevalue useplatformtick >nul 2>&1");
        }

        private void guna2ToggleSwitch7_CheckedChanged(object? sender, EventArgs e)
        {
            // FSUTIL Tweaks (For Storage Device)
            ExecuteAsync("fsutil behavior set disableEncryption 1 >nul 2>&1");
            ExecuteAsync("fsutil 8dot3name set 1 >nul 2>&1");
            ExecuteAsync("fsutil behavior set memoryusage 2 >nul 2>&1");
            ExecuteAsync("fsutil behavior set disablelastaccess 1 >nul 2>&1");
            ExecuteAsync("fsutil resource setautoreset true C:\\ >nul 2>&1");
            ExecuteAsync("fsutil resource setconsistent C:\\ >nul 2>&1");
            ExecuteAsync("fsutil resource setlog shrink 10 C:\\ >nul 2>&1");
        }

        private void guna2ToggleSwitch9_CheckedChanged(object? sender, EventArgs e)
        {
            // Force D0 State
            ExecuteAsync("for /f \"tokens=1,2 delims==\" %%a in ('wmic path Win32_PnPEntity get DeviceID /value') do (if not \"%%a\"==\"\" (set dev=%%a & set dev=!dev:~0,-1! & reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\!dev!\\Device Parameters\\Power\" /v DefaultPowerState /t REG_DWORD /d 0 /f >nul 2>&1))");

            // Disable Modern Standby
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v ModernStandbyDisabled /t REG_DWORD /d 1 /f >nul 2>&1");

            // Disable Event Processor
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v EventProcessorEnabled  /t REG_DWORD /d 0 /f >nul 2>&1");

            // Import Powerplan
            var powerPlanPath = @"C:\Stix Free\Stix Free Powerplan";

            Process.Start("powercfg", $"/import \"{powerPlanPath}\"");

            Process.Start("powercfg.cpl");
        }

        private void guna2ToggleSwitch8_CheckedChanged(object? sender, EventArgs e)
        {
            // Disable Nvidia Crash Reporting & Telemetry
            ExecuteAsync("schtasks /change /disable /tn \"NvTmRep_CrashReport2_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            ExecuteAsync("schtasks /change /disable /tn \"NvTmRep_CrashReport3_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            ExecuteAsync("schtasks /change /disable /tn \"NvTmRep_CrashReport4_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\NVIDIA Corporation\\NvControlPanel2\\Client\" /v \"OptInOrOutPreference\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\nvlddmkm\\Global\\Startup\" /v \"SendTelemetryData\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable PG
            ExecuteAsync("for /f \"delims=\" %%i in ('powershell -command \"Get-WmiObject Win32_VideoController | Select-Object -ExpandProperty PNPDeviceID | findstr /L \\\"PCI\\VEN_\\\"\"') do (for /f \"tokens=3\" %%a in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Enum\\%%i\" /v \"Driver\"') do (for /f %%i in ('echo %%a ^| findstr \"{\"') do (Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMElcg\" /t REG_DWORD /d \"0x55555555\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMBlcg\" /t REG_DWORD /d \"0x1111111\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMElpg\" /t REG_DWORD /d \"0x00000fff\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMFspg\" /t REG_DWORD /d \"0x0000000f\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMSlcg\" /t REG_DWORD /d \"0x0003ffff\" /f > nul 2>&1)))");

            // Import NIP
            ExecuteAsync("\"C:\\Stix Free\\nvidiaProfileInspector.exe\" -import \"C:\\Stix Free\\Stix Free NIP.nip\"\r\n");
        }

        private void guna2ToggleSwitch6_CheckedChanged(object? sender, EventArgs e)
        {
            // Disable PG
            ExecuteAsync("for /f \"delims=\" %%i in ('powershell -command \"Get-WmiObject Win32_VideoController | Select-Object -ExpandProperty PNPDeviceID | findstr /L \\\"PCI\\VEN_\\\"\"') do (for /f \"tokens=3\" %%a in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Enum\\%%i\" /v \"Driver\"') do (for /f %%i in ('echo %%a ^| findstr \"{\"') do (Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableUlps\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableUvdClockGating\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableVceSwClockGating\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"PowerSaverAutoEnable_DEF\" /t REG_DWORD /d \"0\" /f > nul 2>&1)))");
        }

        private void guna2ToggleSwitch15_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Version\" /t REG_SZ /d \"1.0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Application Name\" /t REG_SZ /d \"FortniteClient-Win64-Shipping.exe\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Protocol\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Local Port\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Local IP\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Local IP Prefix Length\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Remote Port\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Remote IP\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Remote IP Prefix Length\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"DSCP Value\" /t REG_DWORD /d 46 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Throttle Rate\" /t REG_DWORD /d -1 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch14_CheckedChanged(object sender, EventArgs e)
        {
            string url = "https://download.sysinternals.com/files/Autoruns.zip";
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

        private void guna2ToggleSwitch13_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("sc config vmicrdv start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmicvss start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmicshutdown start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmicheartbeat start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmicvmsession start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmictimesync start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmicguestinterface start= disabled >nul 2>&1");
            ExecuteAsync("sc config vmickvpexchange start= disabled >nul 2>&1");
            ExecuteAsync("dism /online /disable-feature /featurename:Microsoft-Hyper-V-All /quiet /norestart >nul 2>&1");
        }

        private void guna2ToggleSwitch12_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("sc config BTAGService start= disabled >nul 2>&1");
            ExecuteAsync("sc config bthserv start= disabled >nul 2>&1");
        }

        private void guna2ToggleSwitch11_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-WindowsPackage -Online | Where-Object PackageName -like '*Hello-Face*' | Remove-WindowsPackage -Online -NoRestart\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-WindowsPackage -Online | Where-Object PackageName -like '*QuickAssist*' | Remove-WindowsPackage -Online -NoRestart\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Disney.37853FC22B2CE' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.549981C3F5F10' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.BingNews' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.BingWeather' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.GetHelp' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Getstarted' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MSPaint' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Microsoft3DViewer' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MicrosoftOfficeHub' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MicrosoftSolitaireCollection' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.MicrosoftStickyNotes' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Office.OneNote' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.OneDriveSync' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.People' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.PowerAutomateDesktop' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.SkypeApp' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Todos' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.Wallet' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsAlarms' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsCamera' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsFeedbackHub' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsMaps' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsSoundRecorder' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.YourPhone' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.ZuneMusic' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.ZuneVideo' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'MicrosoftCorporationII.QuickAssist' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'MicrosoftTeams' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'MicrosoftWindows.Client.WebExperience' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'SpotifyAB.SpotifyMusic' | Remove-AppxPackage\"");
            ExecuteAsync("PowerShell -NoProfile -Command \"Get-AppxPackage -AllUsers 'Microsoft.WindowsCommunicationsApps' | Remove-AppxPackage\"");
        }

        private void guna2ToggleSwitch25_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("sc config RemoteRegistry start= disabled >nul 2>&1");
            ExecuteAsync("sc config RemoteAccess start= disabled >nul 2>&1");
            ExecuteAsync("sc config WinRM start= disabled >nul 2>&1");
            ExecuteAsync("sc config RmSvc start= disabled >nul 2>&1");

            // Disable Printer Services
            ExecuteAsync("sc config PrintNotify start= disabled >nul 2>&1");
            ExecuteAsync("sc config Spooler start= disabled >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\Printing\\EduPrintProv\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\Printing\\PrinterCleanupTask\" /Disable >nul 2>&1");

            // Disable Sysmain
            ExecuteAsync("sc config SysMain start= disabled >nul 2>&1");

            // Disable Windows Search
            ExecuteAsync("sc config WSearch start= disabled >nul 2>&1");
        }

        private void guna2ToggleSwitch21_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("sc config ClipSVC start= disabled >nul 2>&1");
            ExecuteAsync("sc config BITS start= disabled >nul 2>&1");
            ExecuteAsync("sc config InstallService start= disabled >nul 2>&1");
            ExecuteAsync("sc config uhssvc start= disabled >nul 2>&1");
            ExecuteAsync("sc config UsoSvc start= disabled >nul 2>&1");
            ExecuteAsync("sc config wuauserv start= disabled >nul 2>&1");
            ExecuteAsync("sc config LanmanServer start= disabled >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\DoSvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\InstallService\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\UsoSvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\wuauserv\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\WaaSMedicSvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\BITS\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\upfc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\uhssvc\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\ossrs\" /v Start /t reg_dword /d 4 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DeferUpdatePeriod\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DeferUpgrade\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DeferUpgradePeriod\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Policies\\Microsoft\\Windows\\WindowsUpdate\" /v \"DisableWindowsUpdateAccess\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\ScanForUpdates\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\ScanForUpdatesAsUser\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\SmartRetry\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\WakeUpAndContinueUpdates\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\InstallService\\WakeUpAndScanForUpdates\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\Report policies\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\Schedule Scan\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\Schedule Scan Static Task\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\UpdateModelTask\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\UpdateOrchestrator\\USO_UxBroker\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\WaaSMedic\\PerformRemediation\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /Change /TN \"Microsoft\\Windows\\WindowsUpdate\\Scheduled Start\" /Disable >nul 2>&1");
        }

        private void guna2ToggleSwitch20_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("set OOSUPath=\"C:\\Stix Free\\OOSU10.exe\"");
            ExecuteAsync("set ConfigPath=\"C:\\Stix Free\\Stix-OOSU.cfg\"");
            ExecuteAsync("");
            ExecuteAsync("if exist %OOSUPath% (");
            ExecuteAsync("    if exist %ConfigPath% (");
            ExecuteAsync("        %OOSUPath% %ConfigPath%");
            ExecuteAsync("    )");
            ExecuteAsync(")");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"PreInstalledAppsEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SilentInstalledAppsEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"OemPreInstalledAppsEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"ContentDeliveryAllowed\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"SubscribedContentEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ContentDeliveryManager\" /v \"PreInstalledAppsEverEnabled\" /t REG_DWORD /d \"0\" /f>nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\CDP\" /v \"CdpSessionUserAuthzPolicy\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\CDP\" /v \"NearShareChannelUserAuthzPolicy\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Schedule\\Maintenance\" /v \"MaintenanceDisabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\current\\device\\System\" /v \"AllowExperimentation\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Microsoft\\PolicyManager\\default\\System\\AllowExperimentation\" /v \"value\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Feeds\" /v \"EnableFeeds\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\" /v \"AllowNewsAndInterests\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\System\" /v \"EnableActivityFeed\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\Control Panel\\International\\User Profile\" /v \"HttpAcceptLanguageOptOut\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\AdvertisingInfo\" /v \"Enabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\System\" /v \"EnableActivityFeed\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Search\" /v \"BingSearchEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search\" /v \"CortanaCapabilities\" /t REG_SZ /d \"\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search\" /v \"IsAssignedAccess\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Search\" /v \"IsWindowsHelloActive\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"AllowSearchToUseLocation\" /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchPrivacy\" /t REG_DWORD /d 3 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchSafeSearch\" /t REG_DWORD /d 3 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchUseWeb\" /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchUseWebOverMeteredConnections\" /t >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Microsoft\\PolicyManager\\default\\Experience\\AllowCortana\" /v \"value\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\SearchCompanion\" /v \"DisableContentFileUpdates\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"AllowCloudSearch\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"AllowCortanaAboveLock\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"AllowSearchToUseLocation\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchPrivacy\" /t REG_DWORD /d \"3\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchUseWeb\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"ConnectedSearchUseWebOverMeteredConnections\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"DisableWebSearch\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\Windows Search\" /v \"DoNotUseWebResults\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"AITEnable\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"AllowTelemetry\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"DisableInventory\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"DisableUAR\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"DisableEngine\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\AppCompat\" /v \"DisablePCA\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Privacy\" /v \"TailoredExperiencesWithDiagnosticDataEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Diagnostics\\DiagTrack\" /v \"ShowedToastAtLevel\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_CURRENT_USER\\Software\\Microsoft\\Input\\TIPC\" /v \"Enabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\System\" /v \"UploadUserActivities\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\System\" /v \"PublishUserActivities\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_CURRENT_USER\\Control Panel\\International\\User Profile\" /v \"HttpAcceptLanguageOptOut\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments\" /v \"SaveZoneInformation\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\System\\CurrentControlSet\\Control\\Diagnostics\\Performance\" /v \"DisablediagnosticTracing\" /t REG_DWORD /d \"1\" /f >nul 2>&1 >nul 2>&1");
            ExecuteAsync("reg add \"HKLM\\Software\\Policies\\Microsoft\\Windows\\WDI\\{9c5a40da-b965-4fc3-8781-88dd50a6299d}\" /v \"ScenarioExecutionEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            ExecuteAsync("schtasks /change /tn \"\\Microsoft\\Windows\\Application Experience\\StartupAppTask\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /end /tn \"\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector\" >nul 2>&1");
            ExecuteAsync("schtasks /change /tn \"\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticDataCollector\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /end /tn \"\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticResolver\" >nul 2>&1");
            ExecuteAsync("schtasks /change /tn \"\\Microsoft\\Windows\\DiskDiagnostic\\Microsoft-Windows-DiskDiagnosticResolver\" /Disable >nul 2>&1");
            ExecuteAsync("schtasks /end /tn \"\\Microsoft\\Windows\\Power Efficiency Diagnostics\\AnalyzeSystem\" >nul 2>&1");
            ExecuteAsync("schtasks /change /tn \"\\Microsoft\\Windows\\Power Efficiency Diagnostics\\AnalyzeSystem\" /Disable >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\ReadyBoot\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\SpoolerLogger\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\UBPM\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\WiFiSession\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\Circular Kernel Context Logger\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\Diagtrack-Listener\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\LwtNetLog\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\Microsoft-Windows-Rdp-Graphics-RdpIdd-Trace\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\NetCore\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\NtfsLog\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\CloudExperienceHostOobe\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderApiLogger\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DefenderAuditLogger\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\RadioMgr\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\RdrLog\" /v Start /t REG_DWORD /d 0 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\DiagLog\" /v Start /t REG_DWORD /d 1 /f >nul 2>&1");
            ExecuteAsync("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\WMI\\Autologger\\WdiContextLog\" /v Start /t REG_DWORD /d 1 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch19_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\NDIS\\Parameters\" /v \"DefaultPnPCapabilities\" /t REG_DWORD /d \"24\" /f >nul 2>&1");
            ExecuteAsync("Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\AFD\\Parameters\" /v \"FastSendDatagramThreshold\" /t REG_DWORD /d \"409600\" /f >nul 2>&1");
            ExecuteAsync("Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\AFD\\Parameters\" /v \"FastCopyReceiveThreshold\" /t REG_DWORD /d \"409600\" /f >nul 2>&1");
        }

        private void guna2ToggleSwitch26_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("for /f %a in ('reg query \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}\" /v \"*SpeedDuplex\" /s ^| findstr \"HKEY\"') do (" +
                            "for /f %i in ('reg query \"%a\" /v \"*DeviceSleepOnDisconnect\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*DeviceSleepOnDisconnect\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*EEE\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*EEE\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*FlowControl\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*FlowControl\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*IPChecksumOffloadIPv4\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*IPChecksumOffloadIPv4\" /t REG_SZ /d \"3\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*InterruptModeration\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*InterruptModeration\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*LsoV2IPv4\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*LsoV2IPv4\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*LsoV2IPv6\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*LsoV2IPv6\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*NumRssQueues\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*NumRssQueues\" /t REG_SZ /d \"2\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*PMARPOffload\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*PMARPOffload\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*PMNSOffload\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*PMNSOffload\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*PriorityVLANTag\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*PriorityVLANTag\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*RSS\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*RSS\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*WakeOnMagicPacket\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*WakeOnMagicPacket\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"AutoPowerSaveModeEnabled\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"AutoPowerSaveModeEnabled\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*WakeOnPattern\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*WakeOnPattern\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*ReceiveBuffers\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*ReceiveBuffers\" /t REG_SZ /d \"2048\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*TransmitBuffers\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*TransmitBuffers\" /t REG_SZ /d \"2048\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*TCPChecksumOffloadIPv4\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*TCPChecksumOffloadIPv4\" /t REG_SZ /d \"3\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*TCPChecksumOffloadIPv6\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*TCPChecksumOffloadIPv6\" /t REG_SZ /d \"3\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*UDPChecksumOffloadIPv4\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*UDPChecksumOffloadIPv4\" /t REG_SZ /d \"3\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*UDPChecksumOffloadIPv6\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*UDPChecksumOffloadIPv6\" /t REG_SZ /d \"3\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"DMACoalescing\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"DMACoalescing\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"EEELinkAdvertisement\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"EEELinkAdvertisement\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"EeePhyEnable\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"EeePhyEnable\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"ITR\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"ITR\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"ReduceSpeedOnPowerDown\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"ReduceSpeedOnPowerDown\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"PowerDownPll\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"PowerDownPll\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"WaitAutoNegComplete\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"WaitAutoNegComplete\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"WakeOnLink\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"WakeOnLink\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"WakeOnSlot\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"WakeOnSlot\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"WakeUpModeCap\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"WakeUpModeCap\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"AdvancedEEE\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"AdvancedEEE\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"EnableGreenEthernet\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"EnableGreenEthernet\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"GigaLite\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"GigaLite\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"PnPCapabilities\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"PnPCapabilities\" /t REG_DWORD /d \"24\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"PowerSavingMode\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"PowerSavingMode\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"S5WakeOnLan\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"S5WakeOnLan\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"SavePowerNowEnabled\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"SavePowerNowEnabled\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"ULPMode\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"ULPMode\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"WolShutdownLinkSpeed\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"WolShutdownLinkSpeed\" /t REG_SZ /d \"2\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"LogLinkStateEvent\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"LogLinkStateEvent\" /t REG_SZ /d \"16\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"WakeOnMagicPacketFromS5\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"WakeOnMagicPacketFromS5\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"Ultra Low Power Mode\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"Ultra Low Power Mode\" /t REG_SZ /d \"Disabled\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"System Idle Power Saver\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"System Idle Power Saver\" /t REG_SZ /d \"Disabled\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"Selective Suspend\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"Selective Suspend\" /t REG_SZ /d \"Disabled\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"Selective Suspend Idle Timeout\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"Selective Suspend Idle Timeout\" /t REG_SZ /d \"60\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"Link Speed Battery Saver\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"Link Speed Battery Saver\" /t REG_SZ /d \"Disabled\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*SelectiveSuspend\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*SelectiveSuspend\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"EnablePME\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"EnablePME\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"TxIntDelay\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"TxIntDelay\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"TxDelay\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"TxDelay\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"EnableModernStandby\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"EnableModernStandby\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*ModernStandbyWoLMagicPacket\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*ModernStandbyWoLMagicPacket\" /t REG_SZ /d \"0\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"EnableLLI\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"EnableLLI\" /t REG_SZ /d \"1\" /f >nul 2>&1) & " +
                            "for /f %i in ('reg query \"%a\" /v \"*SSIdleTimeout\" ^| findstr \"HKEY\"') do (" +
                            "Reg.exe add \"%i\" /v \"*SSIdleTimeout\" /t REG_SZ /d \"60\" /f >nul 2>&1)) >nul 2>&1");

            ExecuteAsync("cls");

            // Disable Different Useless Network Components
            ExecuteAsync("powershell disable-netadapterbinding -name \"*\" -componentid vmware_bridge, ms_lldp, ms_lltdio, ms_implat, ms_tcpip6, ms_rspndr, ms_server, ms_msclient");
        }

        private void guna2ToggleSwitch16_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\NetBT\\Parameters\" /v EnableLMHOSTS /t REG_DWORD /d 0 /f >nul 2>&1");
        }

        private void label8_Click_1(object sender, EventArgs e)
        {
            HomePanel.Visible = false;
            TweaksPanel.Visible = false;
            MiscPanel.Visible = true;
            guna2vScrollBar3.Visible = false;
        }

        private void guna2ToggleSwitch41_CheckedChanged(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                var file = Path.Combine(Path.GetTempPath(), "vc_redist.x64.exe");
                client.DownloadFile("https://aka.ms/vs/17/release/vc_redist.x64.exe", file);
                Process.Start(file);
            }
        }

        private void guna2ToggleSwitch40_CheckedChanged(object sender, EventArgs e)
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

        private void guna2ToggleSwitch17_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"Get-WmiObject -Class Win32_NetworkAdapterConfiguration | Where-Object { $_.IPEnabled -eq $true } | ForEach-Object { $_.SetTcpipNetbios(2) }\"') do (echo Disabling NetBIOS)");
        }

        private void guna2ToggleSwitch42_CheckedChanged(object sender, EventArgs e)
        {
            Process.Start("C:\\Stix Free\\Corrupt Check.bat");
        }

        private void guna2ToggleSwitch10_CheckedChanged(object sender, EventArgs e)
        {
            // Disable Spatial Audio
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Audio\\\" /v \\\"DisableSpatialAudioPerEndpoint\\\" /t REG_DWORD /d \\\"1\\\" /f | Out-Null\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Audio\\\" /v \\\"DisableSpatialAudioVssFeature\\\" /t REG_DWORD /d \\\"1\\\" /f | Out-Null\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Audio\\\" /v \\\"DisableSpatialOnComboEndpoints\\\" /t REG_DWORD /d \\\"1\\\" /f | Out-Null\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKCU\\Software\\Microsoft\\Multimedia\\Audio\\\" /v \\\"UserDuckingPreference \\\" /t REG_DWORD /d \\\"3\\\" /f | Out-Null\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$renderKeys = reg query \\\"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\MMDevices\\Audio\\Render\\\" | Where-Object { `$_ -match \\\"HKEY\\\" }; foreach (`$key in `$renderKeys) { reg add \\\"`$key\\Properties\\\" /v \\\"{b3f8fa53-0004-438e-9003-51a46e139bfc},4\\\" /t REG_DWORD /d \\\"0\\\" /f | Out-Null; reg add \\\"`$key\\Properties\\\" /v \\\"{b3f8fa53-0004-438e-9003-51a46e139bfc},3\\\" /t REG_DWORD /d \\\"0\\\" /f | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$captureKeys = reg query \\\"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\MMDevices\\Audio\\Capture\\\" | Where-Object { `$_ -match \\\"HKEY\\\" }; foreach (`$key in `$captureKeys) { reg add \\\"`$key\\Properties\\\" /v \\\"{b3f8fa53-0004-438e-9003-51a46e139bfc},4\\\" /t REG_DWORD /d \\\"0\\\" /f | Out-Null; reg add \\\"`$key\\Properties\\\" /v \\\"{b3f8fa53-0004-438e-9003-51a46e139bfc},3\\\" /t REG_DWORD /d \\\"0\\\" /f | Out-Null }\"') do (rem)");
        }

        private void guna2ToggleSwitch22_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (SSTP)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (PPPOE)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (L2TP)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (PPTP)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (IPv6)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (IKEv2)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (Network Monitor)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"WAN Miniport (IP)\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"System Timer\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"System Speaker\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Microsoft System Management BIOS Driver\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Composite Bus Enumerator\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"UMBus Root Bus Enumerator\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Direct Memory Access Controller\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Microsoft Hyper-V Virtualization Infrastructure Driver\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"NDIS Virtual Network Adapter Enumerator\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Remote Desktop Device Redirector Bus\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"High precision event timer\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Microsoft Print to PDF\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Microsoft GS Wavetable Synth\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"`$device = Get-PnpDevice -FriendlyName \\\"Microsoft RRAS Root Enumerator\\\" -ErrorAction SilentlyContinue; if (`$device) { pnputil /disable-device `$device.InstanceId | Out-Null }\"') do (rem)");
        }

        private void guna2ToggleSwitch18_CheckedChanged(object sender, EventArgs e)
        {
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\SD\\\" /v IdleStatesNumber /t REG_DWORD /d 0 /f\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"for /f %%S in ('reg query \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\SD\\IdleState\\\" 2^>nul ^| findstr /R \\\"\\\\[0-5]$\\\"') do (reg add \\\"%%S\\\" /v IdleExitLatencyMs /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleExitEnergyMicroJoules /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleTimeLengthMs /t REG_DWORD /d 4294967295 /f & reg add \\\"%%S\\\" /v IdlePowerMw /t REG_DWORD /d 0 /f)\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\SSD\\\" /v IdleStatesNumber /t REG_DWORD /d 0 /f\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"for /f %%S in ('reg query \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\SSD\\IdleState\\\" 2^>nul ^| findstr /R \\\"\\\\[0-5]$\\\"') do (reg add \\\"%%S\\\" /v IdleExitLatencyMs /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleExitEnergyMicroJoules /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleTimeLengthMs /t REG_DWORD /d 4294967295 /f & reg add \\\"%%S\\\" /v IdlePowerMw /t REG_DWORD /d 0 /f)\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\HDD\\\" /v IdleStatesNumber /t REG_DWORD /d 0 /f\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"for /f %%S in ('reg query \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\HDD\\IdleState\\\" 2^>nul ^| findstr /R \\\"\\\\[0-5]$\\\"') do (reg add \\\"%%S\\\" /v IdleExitLatencyMs /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleExitEnergyMicroJoules /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleTimeLengthMs /t REG_DWORD /d 4294967295 /f & reg add \\\"%%S\\\" /v IdlePowerMw /t REG_DWORD /d 0 /f)\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"reg add \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\NVME\\\" /v IdleStatesNumber /t REG_DWORD /d 0 /f\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"for /f %%S in ('reg query \\\"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\Storage\\NVME\\IdleState\\\" 2^>nul ^| findstr /R \\\"\\\\[0-5]$\\\"') do (reg add \\\"%%S\\\" /v IdleExitLatencyMs /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleExitEnergyMicroJoules /t REG_DWORD /d 0 /f & reg add \\\"%%S\\\" /v IdleTimeLengthMs /t REG_DWORD /d 4294967295 /f & reg add \\\"%%S\\\" /v IdlePowerMw /t REG_DWORD /d 0 /f)\"') do (rem)");
            ExecuteAsync("for /f \"tokens=*\" %a in ('powershell -command \"for /f \\\"skip=1 tokens=*\\\" %%i in ('reg query \\\"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\SCSI\\\" /s /f \\\"NVMe\\\" ^| findstr \\\"HKEY\\\"') do (reg add \\\"%%i\\Device Parameters\\StorPort\\\" /f & reg add \\\"%%i\\Device Parameters\\StorPort\\\" /v LowestPowerD3IdleTimeout /t REG_DWORD /d 4294967295 /f & reg add \\\"%%i\\Device Parameters\\StorPort\\\" /v InterruptCoalescingTime /t REG_DWORD /d 1 /f & reg add \\\"%%i\\Device Parameters\\StorPort\\\" /v InterruptCoalescingEntry /t REG_DWORD /d 1 /f & reg add \\\"%%i\\Device Parameters\\StorPort\\\" /v IdlePowerMode /t REG_DWORD /d 0 /f)\"') do (rem)");
        }
    }
}
