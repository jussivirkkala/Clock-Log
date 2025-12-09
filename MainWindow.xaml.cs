/*
 * Displaying clock, keep log and alarm when files not updated.
 * .NET48 x64
 * @jussivirkkala 
 * Transfer done checking for video file 
 * 2025-12-09 v1.0.0 Clock Log
 *
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading; // DispatcherTimer

namespace Clock_Log
{
    /// <summary>
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string appName= "";
        string sFormat = @"{0:HH:mm:ss}";
        short iWidth1 = 120;

      
        // Update clock
        string sError = "";
        void dispatcherClock_Tick(object sender, EventArgs e)
        {
            try
            {
              Time.Content = String.Format(sFormat,DateTime.Now.AddMilliseconds(500));
            }
            catch (Exception)
            {
                Time.Content = ".ini error";
            }
        }

        public MainWindow()
        {
            appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            Log( "Started\t" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).Comments);
            Log( "Version\t" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion );
            Log( "MachineName\t" + Environment.MachineName);
          //  Log( "UserName\t" + Environment.UserName);
            Log( "OS\t" + System.Runtime.InteropServices.RuntimeInformation.OSDescription );
            Log( "OSArchitecture\t" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture );
            Log( "ProcessArchitecture\t" + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture );
            Log( "Framework\t" + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription );

            InitializeComponent();

            // Load ini file
            if (!File.Exists(appName + ".ini"))
            {
                MessageBox.Show("Missing " + appName + ".ini with label, width rows. Using {0:HH:mm:ss} 120", appName);
            }
            else
            {
                int row = 0;

                // 2020-12-01 Look for -computername.ini
                string s = appName + ".ini";
                if (File.Exists(appName + "-" + Environment.MachineName + ".ini"))
                    s = appName + "-" + Environment.MachineName + ".ini";

                foreach (string line1 in File.ReadLines(s))
                {
                    String line = line1.Trim(); 
                    if (!line.StartsWith("#"))
                    {
                        row += 1;
                        switch (row)
                        {
                            case 1:
                                sFormat= line;
                                Log("Clock format\t"+ sFormat);
                                break;
                            case 2:
                                Int16.TryParse(line, out iWidth1);
                                Log("Normal width\t"+iWidth1.ToString("0"));
                                this.Width = iWidth1;
                                break;
                        }
                    }
                }

            }

            DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer1.Tick += new EventHandler(dispatcherClock_Tick);
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dispatcherTimer1.Start();
            Log("Timer started");

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        // Logging to file
        // 2022-07-01 Removed Log: and 
        // 2020-11-29 Machinename in log file.
        void Log(string s)
        {
            try
            {
                Console.WriteLine(DateTime.Now.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fff") + DateTime.Now.ToString("zzz") + "\t"+ s); //  "\tLog: "+s);
                using (StreamWriter sw = File.AppendText(appName + "-" + Environment.MachineName + ".log"))
                    sw.WriteLine(DateTime.Now.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fff") + DateTime.Now.ToString("zzz") + "\t" + s);
            }
            catch
            {
                Console.WriteLine("Error: Writing log");
            }
        }
    }
}

// End