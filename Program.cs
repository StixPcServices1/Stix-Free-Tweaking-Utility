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
        private static readonly DiscordRpcClient client = new DiscordRpcClient("1395436773487939694");

        static void Main()
        {
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
                Details = "Using The Best Free Pc Optimizer",
                Assets = new Assets()
                {
                    LargeImageKey = "tutorial",
                    LargeImageText = "Example Image Text"
                },
                Buttons = new Button[]
                {
                new Button()
                {
                    Label = "Download Now",
                    Url = "https://stixtweaks.com/pages/stix-free-tweaking-utility"
                }
                }
            };

            client.SetPresence(presence);
        }
    }
}
