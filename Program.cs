using DiscordRPC;
using Stix.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Button = DiscordRPC.Button;

namespace Stix
{
    internal static class Program
    {
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1322068883418517535");

        static void Main()
        {
            ExecuteUserClientOperations();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            InitRPC();
            UpdateRPC();

            Application.Run(new Main());
        }
        public static void InitRPC()
        {
            client.Initialize();
        }

        public static void UpdateRPC()
        {
            var presence = new RichPresence()
            {
                State = " On The Market",
                Details = "Using The Best Pc Optimizer",
                Assets = new Assets()
                {
                    LargeImageKey = "tutorial",
                    LargeImageText = "Example Image Text"
                },
                Buttons = new Button[]
                {
                new Button()
                {
                    Label = "Buy Now",
                    Url = "https://www.stixtweaks.com"
                }
                }
            };

            client.SetPresence(presence);
        }


        public enum UserOp : uint
        {
            Register = 1,
            Login = 2,
            Download = 3,
            GetVariable = 6
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        public struct UserOpParams
        {
            public UserOp op;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string username;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string password;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string licenseKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string fileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string fileLocation;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string programKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string variableName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)] public string variableValue;
            public int resultCode;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string outMessage;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate int RunUserClientOp(ref UserOpParams pParams);

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DOS_HEADER
        {
            public ushort e_magic;
            public ushort e_cblp;
            public ushort e_cp;
            public ushort e_crlc;
            public ushort e_cparhdr;
            public ushort e_minalloc;
            public ushort e_maxalloc;
            public ushort e_ss;
            public ushort e_sp;
            public ushort e_csum;
            public ushort e_ip;
            public ushort e_cs;
            public ushort e_lfarlc;
            public ushort e_ovno;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)] public ushort[] e_res1;
            public ushort e_oemid;
            public ushort e_oeminfo;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)] public ushort[] e_res2;
            public int e_lfanew;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_DATA_DIRECTORY
        {
            public uint VirtualAddress;
            public uint Size;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public ushort Magic;
            public byte MajorLinkerVersion;
            public byte MinorLinkerVersion;
            public uint SizeOfCode;
            public uint SizeOfInitializedData;
            public uint SizeOfUninitializedData;
            public uint AddressOfEntryPoint;
            public uint BaseOfCode;
            public ulong ImageBase;
            public uint SectionAlignment;
            public uint FileAlignment;
            public ushort MajorOperatingSystemVersion;
            public ushort MinorOperatingSystemVersion;
            public ushort MajorImageVersion;
            public ushort MinorImageVersion;
            public ushort MajorSubsystemVersion;
            public ushort MinorSubsystemVersion;
            public uint Win32VersionValue;
            public uint SizeOfImage;
            public uint SizeOfHeaders;
            public uint CheckSum;
            public ushort Subsystem;
            public ushort DllCharacteristics;
            public ulong SizeOfStackReserve;
            public ulong SizeOfStackCommit;
            public ulong SizeOfHeapReserve;
            public ulong SizeOfHeapCommit;
            public uint LoaderFlags;
            public uint NumberOfRvaAndSizes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)] public IMAGE_DATA_DIRECTORY[] DataDirectory;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGE_NT_HEADERS64
        {
            public uint Signature;
            public IMAGE_FILE_HEADER FileHeader;
            public IMAGE_OPTIONAL_HEADER64 OptionalHeader;
        }

        static int FindDataMarker(byte[] data, byte[] pattern)
        {
            if (pattern.Length > data.Length)
                return -1;

            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (data[i + j] != pattern[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                    return i;
            }
            return -1;
        }

        static void ExecuteUserClientOperations()
        {
            // Load the DLL
            IntPtr hMod = LoadLibrary("UserClient.dll");
            if (hMod == IntPtr.Zero)
            {
                Console.WriteLine("Failed to load UserClient.dll");
                return;
            }

            try
            {
                IMAGE_DOS_HEADER dosHeader = Marshal.PtrToStructure<IMAGE_DOS_HEADER>(hMod);
                if (dosHeader.e_magic != 0x5A4D) // 'MZ'
                {
                    Console.WriteLine("Invalid DOS header magic.");
                    return;
                }

                IntPtr ntHeaderPtr = IntPtr.Add(hMod, dosHeader.e_lfanew);
                IMAGE_NT_HEADERS64 ntHeaders = Marshal.PtrToStructure<IMAGE_NT_HEADERS64>(ntHeaderPtr);
                if (ntHeaders.Signature != 0x00004550) // 'PE\0\0'
                {
                    Console.WriteLine("Invalid NT headers signature.");
                    return;
                }

                uint sizeOfImage = ntHeaders.OptionalHeader.SizeOfImage;

                byte[] moduleBytes = new byte[sizeOfImage];
                Marshal.Copy(hMod, moduleBytes, 0, (int)sizeOfImage);

                string markerString = "[UserOpsEntry]";
                byte[] pattern = Encoding.ASCII.GetBytes(markerString);

                int found = FindDataMarker(moduleBytes, pattern);
                if (found == -1)
                {
                    Console.WriteLine("Marker not found.");
                    return;
                }

                int fnPtrOffset = found - IntPtr.Size;
                if (fnPtrOffset < 0)
                {
                    Console.WriteLine("Function pointer offset is out of range.");
                    return;
                }

                ulong fnPtrValue = BitConverter.ToUInt64(moduleBytes, fnPtrOffset);
                IntPtr fnPtr = new IntPtr((long)fnPtrValue);
                Console.WriteLine($"Function pointer address: 0x{fnPtr.ToInt64():X}");

                RunUserClientOp pFn = Marshal.GetDelegateForFunctionPointer<RunUserClientOp>(fnPtr);
                if (pFn == null)
                {
                    Console.WriteLine("Failed to create delegate for function pointer.");
                    return;
                }

                // Perform the Login operation
                UserOpParams loginParams = new UserOpParams
                {
                    op = UserOp.Login,
                    username = "test_user",
                    password = "test_user",
                    programKey = "9u9g1ug9gu1f"
                };

                int loginRet = pFn(ref loginParams);
                Console.WriteLine($"[Login] ret={loginRet}, message={loginParams.outMessage}");

                if (loginRet != 0)
                {
                    Console.WriteLine("Login failed. Exiting.");
                    return;
                }

                // GetVariable operation
                UserOpParams registerParams = new UserOpParams
                {
                    op = UserOp.GetVariable,
                    variableName = "long_var",
                };

                int registerRet = pFn(ref registerParams);
                Console.WriteLine($"[GetVariable] ret={registerRet}, value={registerParams.variableValue}");

                if (registerRet != 0)
                {
                    Console.WriteLine("GetVariable failed. Exiting.");
                    return;
                }

                // Download operation
                UserOpParams downloadParams = new UserOpParams
                {
                    op = UserOp.Download,
                    fileName = "crashes.dll",
                    fileLocation = "C:\\test.dll"
                };

                int downloadRet = pFn(ref downloadParams);
                Console.WriteLine($"[Download] ret={downloadRet}, message={downloadParams.outMessage}");

                if (downloadRet != 0)
                {
                    Console.WriteLine("Download failed. Exiting.");
                    return;
                }
            }
            finally
            {
                // Free the loaded DLL
                FreeLibrary(hMod);
            }
        }
    }
}
