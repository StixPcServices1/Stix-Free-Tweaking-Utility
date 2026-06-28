using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stix.Design
{
    public partial class TweaksPanel : UserControl
    {
        public TweaksPanel()
        {
            InitializeComponent();
        }

        private void TweaksBackPanel_Paint(object sender, PaintEventArgs e)
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
            // Disable Dynamic Tick
            Execute("bcdedit /set disabledynamictick yes >nul 2>&1");

            // Disable FTH (Fault Tolerant Heap)
            Execute("reg add \"HKEY_LOCAL_MACHINE\\Software\\Microsoft\\FTH\" /v Enabled /t REG_DWORD /d 0 /f >nul 2>&1");

            // Set SystemResponsiveness & NetworkThrottlingIndex to a (Lowest Value | Default)
            Execute("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v \"SystemResponsiveness\" /t REG_DWORD /d \"a\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v \"NetworkThrottlingIndex\" /t REG_DWORD /d \"a\" /f >nul 2>&1");

            // Disable IOLatencyCap
            Execute("FOR /F \"eol=E\" %%a in ('REG QUERY \"HKLM\\SYSTEM\\CurrentControlSet\\Services\" /S /F \"IoLatencyCap\"^| FINDSTR /V \"IoLatencyCap\"') DO (REG ADD \"%%a\" /F /V \"IoLatencyCap\" /T REG_DWORD /d 0 >nul 2>&1 & FOR /F \"tokens=*\" %%z IN (\"%%a\") DO (SET STR=%%z & SET STR=!STR:HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\=! & SET STR=!STR:\\Parameters=!))");

            // Enable Windows Gamemode
            Execute("reg add \"HKCU\\SOFTWARE\\Microsoft\\GameBar\" /v \"AllowAutoGameMode\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKCU\\SOFTWARE\\Microsoft\\GameBar\" /v \"AutoGameModeEnabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");

            // Disable StorPortIdle
            Execute("for /f \"tokens=*\" %%s in ('reg query \"HKLM\\System\\CurrentControlSet\\Enum\" /S /F \"StorPort\" ^| findstr /e \"StorPort\"') do reg add \"%%s\" /v \"EnableIdlePowerManagement\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable VBS
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\" /v \"EnableVirtualizationBasedSecurity\" /t REG_DWORD /d 0 /f >nul 2>&1");

            // Disable PowerThrottlingOff & Hibernation
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\PowerThrottling\" /v \"PowerThrottlingOff\" /t REG_DWORD /d 1 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"HiberBootEnabled\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v \"HibernateEnabled\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("powercfg -h off");

            // Disable Storage Sense
            Execute("reg add \"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy\" /v \"01\" /t REG_DWORD /d 0 /f >nul 2>&1");

            // Disable Sleep Study
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v \"SleepstudyAccountingEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"SleepStudyDisabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"SleepStudyDeviceAccountingLevel\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable Energy Estimation
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\TaggedEnergy\" /v \"DisableTaggedEnergyLogging\" /t REG_DWORD /d \"1\" /f >nul 2>&1");

            // Set Win32PrioritySeparation
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl\" /v \"Win32PrioritySeparation\" /t REG_DWORD /d 42 /f >nul 2>&1");

            // Disable DMA Remapping
            Execute("for %%a in (DmaRemappingCompatible) do for /f \"delims=\" %%b in ('reg query \"HKLM\\SYSTEM\\CurrentControlSet\\Services\" /s /f \"%%a\" ^| findstr \"HKEY\"') do Reg.exe add \"%%b\" /v \"%%a\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable Windows Updates
            Execute("\"C:\\Stix Free\\Wub.exe\"");
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            // Kernel Tweks
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v CoalescingTimerInterval /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v AlwaysTrackIoBoosting /t REG_DWORD /d 10 /f >nul 2>&1");

            // Enable Interrupt Steering
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v InterruptSteeringFlags /t REG_DWORD /d 2 /f >nul 2>&1");

            // Restrict Timer Interrupts to Core 0
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v SerializeTimerExpiration /t REG_DWORD /d 1 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            // Disable Cache Telemetry
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"DisableCacheTelemetry\" /t REG_DWORD /d 1 /f >nul 2>&1");

            // Disable Memory Compression
            Execute("powershell \"Disable-MMAgent -MemoryCompression\" >nul 2>&1");
        }

        private void guna2ToggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            // Enable HAGS
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\" /v \"HwSchMode\" /t REG_DWORD /d 2 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch5_CheckedChanged(object sender, EventArgs e)
        {
            // Disable PlatformClock/Tick
            Execute("bcdedit /deletevalue useplatformclock >nul 2>&1");
            Execute("bcdedit /deletevalue useplatformtick >nul 2>&1");
        }

        private void guna2ToggleSwitch6_CheckedChanged(object sender, EventArgs e)
        {
            // FSUTIL Tweaks (For Storage Device)
            Execute("fsutil behavior set disableEncryption 1 >nul 2>&1");
            Execute("fsutil 8dot3name set 1 >nul 2>&1");
            Execute("fsutil behavior set memoryusage 2 >nul 2>&1");
            Execute("fsutil behavior set disablelastaccess 1 >nul 2>&1");
            Execute("fsutil resource setautoreset true C:\\ >nul 2>&1");
            Execute("fsutil resource setconsistent C:\\ >nul 2>&1");
            Execute("fsutil resource setlog shrink 10 C:\\ >nul 2>&1");
        }

        private void guna2ToggleSwitch7_CheckedChanged(object sender, EventArgs e)
        {
            // Force D0 State
            Execute("for /f \"tokens=1,2 delims==\" %%a in ('wmic path Win32_PnPEntity get DeviceID /value') do (if not \"%%a\"==\"\" (set dev=%%a & set dev=!dev:~0,-1! & reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\!dev!\\Device Parameters\\Power\" /v DefaultPowerState /t REG_DWORD /d 0 /f >nul 2>&1))");

            // Disable Modern Standby
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v ModernStandbyDisabled /t REG_DWORD /d 1 /f >nul 2>&1");

            // Disable Event Processor
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v EventProcessorEnabled  /t REG_DWORD /d 0 /f >nul 2>&1");

            // Import Powerplan
            var powerPlanPath = @"C:\Stix Free\Stix Free Powerplan";

            if (File.Exists(powerPlanPath))
            {
                Process.Start("powercfg", $"/import \"{powerPlanPath}\"");
            }

            Process.Start("powercfg.cpl");
        }

        private void guna2ToggleSwitch8_CheckedChanged(object sender, EventArgs e)
        {
            // Disable Nvidia Crash Reporting & Telemetry
            Execute("schtasks /change /disable /tn \"NvTmRep_CrashReport2_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            Execute("schtasks /change /disable /tn \"NvTmRep_CrashReport3_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            Execute("schtasks /change /disable /tn \"NvTmRep_CrashReport4_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\NVIDIA Corporation\\NvControlPanel2\\Client\" /v \"OptInOrOutPreference\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\nvlddmkm\\Global\\Startup\" /v \"SendTelemetryData\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Disable PG
            Execute("for /f \"delims=\" %%i in ('powershell -command \"Get-WmiObject Win32_VideoController | Select-Object -ExpandProperty PNPDeviceID | findstr /L \\\"PCI\\VEN_\\\"\"') do (for /f \"tokens=3\" %%a in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Enum\\%%i\" /v \"Driver\"') do (for /f %%i in ('echo %%a ^| findstr \"{\"') do (Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMElcg\" /t REG_DWORD /d \"0x55555555\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMBlcg\" /t REG_DWORD /d \"0x1111111\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMElpg\" /t REG_DWORD /d \"0x00000fff\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMFspg\" /t REG_DWORD /d \"0x0000000f\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMSlcg\" /t REG_DWORD /d \"0x0003ffff\" /f > nul 2>&1)))");

            // Import NIP
            Execute("\"C:\\Stix Free\\nvidiaProfileInspector.exe\" -import \"C:\\Stix Free\\Stix Free NIP.nip\"\r\n");
        }

        private void guna2ToggleSwitch9_CheckedChanged(object sender, EventArgs e)
        {
            // Disable PG
            Execute("for /f \"delims=\" %%i in ('powershell -command \"Get-WmiObject Win32_VideoController | Select-Object -ExpandProperty PNPDeviceID | findstr /L \\\"PCI\\VEN_\\\"\"') do (for /f \"tokens=3\" %%a in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Enum\\%%i\" /v \"Driver\"') do (for /f %%i in ('echo %%a ^| findstr \"{\"') do (Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableUlps\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableUvdClockGating\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableVceSwClockGating\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"PowerSaverAutoEnable_DEF\" /t REG_DWORD /d \"0\" /f > nul 2>&1)))");
        }

        private void guna2ToggleSwitch10_CheckedChanged(object sender, EventArgs e)
        {
            // Classic Right Click Menu
            Execute("reg add \"HKCU\\Software\\Classes\\CLSID\\{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}\\InprocServer32\" /v \"(Default)\" /t REG_SZ /d \"\" /f >nul 2>&1");

            // Use Dark Mode
            Execute("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize\" /v \"AppsUseLightTheme\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize\" /v \"SystemUsesLightTheme\" /t REG_DWORD /d \"0\" /f >nul 2>&1");

            // Show File Extension
            Execute("reg add \"HKCU\\Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\Advanced\" /v \"HideFileExt\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
        }
    }
}
