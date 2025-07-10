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
    public partial class NetworkPanel : UserControl
    {
        public NetworkPanel()
        {
            InitializeComponent();
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
        private void guna2ToggleSwitch11_CheckedChanged(object sender, EventArgs e)
        {
            Execute("Reg.exe add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\NDIS\\Parameters\" /v \"DefaultPnPCapabilities\" /t REG_DWORD /d \"24\" /f >nul 2>&1");
        }

        private void guna2ToggleSwitch10_CheckedChanged(object sender, EventArgs e)
        {
            Execute("for /f %a in ('reg query \"HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{4d36e972-e325-11ce-bfc1-08002be10318}\" /v \"*SpeedDuplex\" /s ^| findstr \"HKEY\"') do (" +
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

            Execute("cls");
            Execute("powershell disable-netadapterbinding -name \"*\" -componentid vmware_bridge, ms_lldp, ms_lltdio, ms_implat, ms_tcpip6, ms_rspndr, ms_server, ms_msclient");
        }

        private void guna2ToggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            Execute("for /f \"tokens=*\" %a in ('powershell -command \"Get-WmiObject -Class Win32_NetworkAdapterConfiguration | Where-Object { $_.IPEnabled -eq $true } | ForEach-Object { $_.SetTcpipNetbios(2) }\"') do (echo Disabling NetBIOS)");
        }

        private void guna2ToggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            Execute("reg add \"HKLM\\SYSTEM\\CurrentControlSet\\Services\\NetBT\\Parameters\" /v EnableLMHOSTS /t REG_DWORD /d 0 /f >nul 2>&1");
        }

        private void guna2ToggleSwitch5_CheckedChanged(object sender, EventArgs e)
        {
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Version\" /t REG_SZ /d \"1.0\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Application Name\" /t REG_SZ /d \"FortniteClient-Win64-Shipping.exe\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Protocol\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Local Port\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Local IP\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Local IP Prefix Length\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Remote Port\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Remote IP\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Remote IP Prefix Length\" /t REG_SZ /d \"*\" /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"DSCP Value\" /t REG_DWORD /d 46 /f >nul 2>&1");
            Execute("reg add \"HKLM\\SOFTWARE\\Policies\\Microsoft\\Windows\\QoS\\FortnitePolicy\" /v \"Throttle Rate\" /t REG_DWORD /d -1 /f >nul 2>&1");
        }
    }
}
