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
            Execute("bcdedit /set disabledynamictick yes >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v \"ThreadDpcEnable\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\Software\\Microsoft\\FTH\" /v Enabled /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\csrss.exe\\PerfOptions\" /v \"CpuPriorityClass\" /t REG_DWORD /d \"3\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\csrss.exe\\PerfOptions\" /v \"IoPriority\" /t REG_DWORD /d \"3\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Multimedia\\SystemProfile\" /v \"SystemResponsiveness\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("FOR /F \"eol=E\" %%a in ('REG QUERY \"HKLM\\SYSTEM\\CurrentControlSet\\Services\" /S /F \"IoLatencyCap\"^| FINDSTR /V \"IoLatencyCap\"') DO (REG ADD \"%%a\" /F /V \"IoLatencyCap\" /T REG_DWORD /d 0 >nul 2>&1 & FOR /F \"tokens=*\" %%z IN (\"%%a\") DO (SET STR=%%z & SET STR=!STR:HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\=! & SET STR=!STR:\\Parameters=!))");
            Execute("reg add \"HKCU\\SOFTWARE\\Microsoft\\GameBar\" /v \"AllowAutoGameMode\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKCU\\SOFTWARE\\Microsoft\\GameBar\" /v \"AutoGameModeEnabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("for /f \"tokens=*\" %%s in ('reg query \"HKLM\\System\\CurrentControlSet\\Enum\" /S /F \"StorPort\" ^| findstr /e \"StorPort\"') do reg add \"%%s\" /v \"EnableIdlePowerManagement\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\" /v \"HwSchMode\" /t REG_DWORD /d 2 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\DeviceGuard\" /v \"EnableVirtualizationBasedSecurity\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\PowerThrottling\" /v \"PowerThrottlingOff\" /t REG_DWORD /d 1 /f >nul 2>&1");
            Execute("powercfg -h off");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"HiberBootEnabled\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v \"HibernateEnabled\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\StorageSense\\Parameters\\StoragePolicy\" /v \"01\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v \"SleepstudyAccountingEnabled\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"SleepStudyDisabled\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Power\" /v \"SleepStudyDeviceAccountingLevel\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\TaggedEnergy\" /v \"DisableTaggedEnergyLogging\" /t REG_DWORD /d \"1\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\TaggedEnergy\" /v \"TelemetryMaxApplication\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\\EnergyEstimation\\TaggedEnergy\" /v \"TelemetryMaxTagPerApplication\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\MMCSS\" /v \"Start\" /t REG_DWORD /d \"4\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\PriorityControl\" /v \"Win32PrioritySeparation\" /t REG_DWORD /d 42 /f >nul 2>&1");
            Execute("for %%a in (DmaRemappingCompatible) do for /f \"delims=\" %%b in ('reg query \"HKLM\\SYSTEM\\CurrentControlSet\\Services\" /s /f \"%%a\" ^| findstr \"HKEY\"') do Reg.exe add \"%%b\" /v \"%%a\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("\"C:\\Stix Free\\Wub.exe\"");
        }

        private void guna2ToggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v DpcWatchdogProfileOffset /t REG_DWORD /d 2710 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v MaxDynamicTickDuration /t REG_DWORD /d 10 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\kernel\" /v MinimumDpcRate /t REG_DWORD /d 1 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"ClearPageFileAtShutdown\" /t REG_DWORD /d 0x00000000 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"LargeSystemCache\" /t REG_DWORD /d 0x00000001 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"NonPagedPoolQuota\" /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"NonPagedPoolSize\" /t REG_DWORD /d 0x00000000 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Session Manager\\Memory Management\" /v \"PhysicalAddressExtension\" /t REG_DWORD /d 0x00000001 /f >nul 2>&1");
            Execute("powershell \"Disable-MMAgent -MemoryCompression\" >nul 2>&1");
        }

        private void guna2ToggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\GraphicsDrivers\" /v \"HwSchMode\" /t REG_DWORD /d 2 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch5_CheckedChanged(object sender, EventArgs e)
        {
            Execute("bcdedit /deletevalue useplatformclock >nul 2>&1");
            Execute("bcdedit /deletevalue useplatformtick >nul 2>&1");
        }

        private void guna2ToggleSwitch6_CheckedChanged(object sender, EventArgs e)
        {
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
            Execute("for /f \"tokens=1,2 delims==\" %%a in ('wmic path Win32_PnPEntity get DeviceID /value') do (if not \"%%a\"==\"\" (set dev=%%a & set dev=!dev:~0,-1! & reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\!dev!\\Device Parameters\\Power\" /v DefaultPowerState /t REG_DWORD /d 0 /f >nul 2>&1))");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v ModernStandbyDisabled /t REG_DWORD /d 1 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\System\\CurrentControlSet\\Control\\Session Manager\\Power\" /v HibernateEnabled /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FlyoutMenuSettings\" /v ShowHibernateOption /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v InitialUnparkCount /t REG_DWORD /d 100 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v EventProcessorEnabled  /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Power\" /v CStates /t REG_DWORD /d 0 /f >nul 2>&1");
            Execute("powercfg -h off >nul 2>&1");
            var powerPlanPath = @"C:\Stix Free\Stix Free Powerplan";

            if (File.Exists(powerPlanPath))
            {
                Process.Start("powercfg", $"/import \"{powerPlanPath}\"");
            }

            Process.Start("powercfg.cpl");
        }

        private void guna2ToggleSwitch8_CheckedChanged(object sender, EventArgs e)
        {
            Execute("schtasks /change /disable /tn \"NvTmRep_CrashReport2_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            Execute("schtasks /change /disable /tn \"NvTmRep_CrashReport3_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            Execute("schtasks /change /disable /tn \"NvTmRep_CrashReport4_{B2FE1952-0186-46C3-BAEC-A80AA35AC5B8}\" >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\NVIDIA Corporation\\NvControlPanel2\\Client\" /v \"OptInOrOutPreference\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\nvlddmkm\\Global\\Startup\" /v \"SendTelemetryData\" /t REG_DWORD /d \"0\" /f >nul 2>&1");
            Execute("for /f \"delims=\" %%i in ('powershell -command \"Get-WmiObject Win32_VideoController | Select-Object -ExpandProperty PNPDeviceID | findstr /L \\\"PCI\\VEN_\\\"\"') do (for /f \"tokens=3\" %%a in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Enum\\%%i\" /v \"Driver\"') do (for /f %%i in ('echo %%a ^| findstr \"{\"') do (Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMPowerFeature\" /t REG_DWORD /d \"0x55455555\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMPowerFeature2\" /t REG_DWORD /d \"0x05555554\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMElcg\" /t REG_DWORD /d \"0x55555555\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMBlcg\" /t REG_DWORD /d \"0x1111111\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMElpg\" /t REG_DWORD /d \"0x00000fff\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMFspg\" /t REG_DWORD /d \"0x0000000f\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"RMSlcg\" /t REG_DWORD /d \"0x0003ffff\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"DisableDynamicPstate\" /t REG_DWORD /d \"1\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"DisableAsyncPstates\" /t REG_DWORD /d \"1\" /f > nul 2>&1)))");
            Execute("\"C:\\Stix Free\\nvidiaProfileInspector.exe\" -import \"C:\\Stix Free\\Stix Free NIP.nip\"\r\n");
        }

        private void guna2ToggleSwitch9_CheckedChanged(object sender, EventArgs e)
        {
            Execute("for /f \"delims=\" %%i in ('powershell -command \"Get-WmiObject Win32_VideoController | Select-Object -ExpandProperty PNPDeviceID | findstr /L \\\"PCI\\VEN_\\\"\"') do (for /f \"tokens=3\" %%a in ('reg query \"HKLM\\SYSTEM\\ControlSet001\\Enum\\%%i\" /v \"Driver\"') do (for /f %%i in ('echo %%a ^| findstr \"{\"') do (Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableUlps\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableUvdClockGating\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"EnableVceSwClockGating\" /t REG_DWORD /d \"0\" /f > nul 2>&1 & Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Control\\Class\\%%i\" /v \"PowerSaverAutoEnable_DEF\" /t REG_DWORD /d \"0\" /f > nul 2>&1)))");
        }
    }
}
