using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class SystemInfo : IDisposable
{
    private readonly Label[] allLabels;
    private PerformanceCounter downloadCounter;
    private PerformanceCounter uploadCounter;
    private NetworkInterface activeAdapter;
    private volatile bool isMonitoring;
    private CancellationTokenSource cts;
    private Task monitoringTask;

    private static readonly Lazy<(string cpu, string cores, string threads)> cpuInfo =
        new Lazy<(string, string, string)>(GetCpuInfoInternal);
    private static readonly Lazy<(string gpu, string driver, string vram)> gpuInfo =
        new Lazy<(string, string, string)>(GetGpuInfoInternal);

    public SystemInfo(params Label[] labels)
    {
        if (labels.Length != 9) throw new ArgumentException("Exactly 9 labels required");
        allLabels = labels;

        Task.Run(InitializeAsync);
    }

    private async Task InitializeAsync()
    {
        try
        {
            await InitializeSystemInfoAsync();
        }
        catch
        {
            SetAllLabelsToNA();
        }
    }

    private async Task InitializeSystemInfoAsync()
    {
        var cpuTask = Task.Run(() => UpdateCpuLabels());
        var gpuTask = Task.Run(() => UpdateGpuLabels());

        await UpdateNetworkAdapterInfoAsync();

        await Task.WhenAll(cpuTask, gpuTask);

        InitializePerformanceCounters();
        StartNetworkMonitoring();
    }

    private void UpdateCpuLabels()
    {
        var (cpu, cores, threads) = cpuInfo.Value;
        SafeLabelUpdate(allLabels[0], cpu);
        SafeLabelUpdate(allLabels[1], cores);
        SafeLabelUpdate(allLabels[2], threads);
    }

    private void UpdateGpuLabels()
    {
        var (gpu, driver, vram) = gpuInfo.Value;
        SafeLabelUpdate(allLabels[3], gpu);
        SafeLabelUpdate(allLabels[4], driver);
        SafeLabelUpdate(allLabels[5], vram);
    }

    private static (string cpu, string cores, string threads) GetCpuInfoInternal()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                var collection = searcher.Get();
                var cpu = collection.Cast<ManagementObject>().FirstOrDefault();

                string cpuName = cpu?["Name"]?.ToString().Trim() ?? "N/A";
                if (cpuName != "N/A")
                {
                    cpuName = cpuName.Replace("AMD ", "").Replace("Intel(R) ", "").Replace("(R)", "").Replace("(TM)", "");

                    if (cpuName.Contains("Core Processor"))
                    {
                        int coreIndex = cpuName.IndexOf("-Core");
                        if (coreIndex > 0)
                        {
                            cpuName = cpuName.Substring(0, coreIndex).Trim();
                        }
                    }

                    cpuName = cpuName.Replace("  ", " ").Trim();

                    string[] parts = cpuName.Split(' ');
                    if (parts.Length > 2)
                    {
                        if (parts[0] == "Ryzen")
                        {
                            if (parts.Length >= 3 && (parts[2].Contains("X3D") || parts[2] == "X3D"))
                            {
                                cpuName = $"{parts[0]} {parts[1]} {parts[2]}";
                            }
                            else
                            {
                                cpuName = $"{parts[0]} {parts[1]} {parts[2]}";
                            }
                        }
                        else if (parts[0] == "Core" && parts[1] == "Ultra")
                        {
                            if (parts.Length >= 4)
                            {
                                cpuName = $"{parts[0]} {parts[1]} {parts[2]} {parts[3]}";
                            }
                        }
                        else if (parts[0] == "Core")
                        {
                            if (parts.Length >= 3)
                            {
                                cpuName = $"{parts[0]} {parts[1]} {parts[2]}";
                            }
                        }
                    }
                }

                return (
                    cpuName,
                    $"Cores: {cpu?["NumberOfCores"] ?? "N/A"}",
                    $"Threads: {cpu?["NumberOfLogicalProcessors"] ?? "N/A"}"
                );
            }
        }
        catch
        {
            return ("N/A", "Cores: N/A", "Threads: N/A");
        }
    }

    private static (string gpu, string driver, string vram) GetGpuInfoInternal()
    {
        try
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
            {
                var collection = searcher.Get();
                var gpu = collection.Cast<ManagementObject>().FirstOrDefault();

                string vramInfo = "N/A";
                if (gpu?["AdapterRAM"] != null)
                {
                    try
                    {
                        uint vramBytes = Convert.ToUInt32(gpu["AdapterRAM"]);
                        if (vramBytes > 0)
                        {
                            double vramGB = vramBytes / (1024.0 * 1024.0 * 1024.0);
                            vramInfo = $"{Math.Round(vramGB, 1)}GB";
                        }
                    }
                    catch { }
                }

                if (vramInfo == "N/A")
                {
                    try
                    {
                        using (var memSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController"))
                        {
                            foreach (ManagementObject mo in memSearcher.Get())
                            {
                                if (mo["AdapterRAM"] != null)
                                {
                                    object ramValue = mo["AdapterRAM"];
                                    if (ramValue is uint && (uint)ramValue > 0)
                                    {
                                        double vramGB = (uint)ramValue / (1024.0 * 1024.0 * 1024.0);
                                        vramInfo = $"{Math.Round(vramGB, 1)}GB";
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }

                string driverVersion = "N/A";
                try
                {
                    using (var regKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Class\{4d36e968-e325-11ce-bfc1-08002be10318}\0000"))
                    {
                        if (regKey != null)
                        {
                            var radeonVersion = regKey.GetValue("RadeonSoftwareVersion");
                            if (radeonVersion != null)
                            {
                                driverVersion = radeonVersion.ToString();
                            }
                            else
                            {
                                var regularVersion = regKey.GetValue("DriverVersion");
                                if (regularVersion != null)
                                {
                                    driverVersion = regularVersion.ToString();
                                }
                            }
                        }
                    }
                }
                catch { }

                return (
                    gpu?["Name"]?.ToString() ?? "N/A",
                    $"Version: {driverVersion}",
                    $"VRAM: {vramInfo}"
                );
            }
        }
        catch
        {
            return ("N/A", "Driver Version: N/A", "VRAM: N/A");
        }
    }

    private async Task UpdateNetworkAdapterInfoAsync()
    {
        try
        {
            await Task.Run(() =>
            {
                activeAdapter = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(n => n.OperationalStatus == OperationalStatus.Up &&
                                       n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

                string adapterName = activeAdapter?.Description ?? "N/A";
                if (adapterName != "N/A")
                {
                    if (adapterName.ToLower().Contains("wi-fi") || adapterName.ToLower().Contains("wireless"))
                    {
                        adapterName = "WiFi";
                    }
                    else if (adapterName.ToLower().Contains("ethernet"))
                    {
                        adapterName = "Ethernet";
                    }
                }

                SafeLabelUpdate(allLabels[6], adapterName);
            });
        }
        catch
        {
            SafeLabelUpdate(allLabels[6], "N/A");
        }
    }

    private void InitializePerformanceCounters()
    {
        try
        {
            if (activeAdapter != null)
            {
                var interfaceName = GetPerformanceCounterInterfaceName(activeAdapter.Description);
                if (!string.IsNullOrEmpty(interfaceName))
                {
                    downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", interfaceName);
                    uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", interfaceName);

                    downloadCounter.NextValue();
                    uploadCounter.NextValue();
                    return;
                }
            }

            var category = new PerformanceCounterCategory("Network Interface");
            var instances = category.GetInstanceNames().Where(name =>
                !name.Contains("Loopback") &&
                !name.Contains("isatap") &&
                !name.Contains("Teredo")).ToArray();

            if (instances.Length > 0)
            {
                downloadCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", instances[0]);
                uploadCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", instances[0]);

                downloadCounter.NextValue();
                uploadCounter.NextValue();
            }
        }
        catch
        {
            SafeLabelUpdate(allLabels[7], "Download: N/A");
            SafeLabelUpdate(allLabels[8], "Upload: N/A");
        }
    }

    private string GetPerformanceCounterInterfaceName(string description)
    {
        try
        {
            var category = new PerformanceCounterCategory("Network Interface");
            return category.GetInstanceNames()
                .FirstOrDefault(name => description.Contains(name) || name.Contains(description));
        }
        catch
        {
            return null;
        }
    }

    private void StartNetworkMonitoring()
    {
        if (downloadCounter == null || uploadCounter == null) return;

        isMonitoring = true;
        cts = new CancellationTokenSource();

        monitoringTask = Task.Run(async () =>
        {
            await Task.Delay(1000);

            while (isMonitoring && !cts.Token.IsCancellationRequested)
            {
                UpdateNetworkSpeeds();
                await Task.Delay(1000);
            }
        }, cts.Token);
    }

    private void UpdateNetworkSpeeds()
    {
        try
        {
            float downloadSpeed = downloadCounter.NextValue();
            float uploadSpeed = uploadCounter.NextValue();

            double downloadMbps = (downloadSpeed * 8) / (1024 * 1024);
            double uploadMbps = (uploadSpeed * 8) / (1024 * 1024);

            SafeLabelUpdate(allLabels[7], $"Download: {Math.Round(downloadMbps, 1)}Mbps");
            SafeLabelUpdate(allLabels[8], $"Upload: {Math.Round(uploadMbps, 1)}Mbps");
        }
        catch
        {
            SafeLabelUpdate(allLabels[7], "Download: N/A");
            SafeLabelUpdate(allLabels[8], "Upload: N/A");
        }
    }

    private void SafeLabelUpdate(Label label, string text)
    {
        if (label.InvokeRequired)
        {
            label.BeginInvoke((MethodInvoker)(() => label.Text = text));
        }
        else
        {
            label.Text = text;
        }
    }

    private void SetAllLabelsToNA()
    {
        foreach (var label in allLabels)
        {
            SafeLabelUpdate(label, "N/A");
        }
    }

    public void Dispose()
    {
        isMonitoring = false;
        cts?.Cancel();

        try
        {
            monitoringTask?.Wait(500);
        }
        catch
        {

        }

        downloadCounter?.Dispose();
        uploadCounter?.Dispose();
        cts?.Dispose();
    }
}