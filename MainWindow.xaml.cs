/*
 * Displaying clock, keep log
 * .NET48 x64
 * @jussivirkkala 
 * Transfer done checking for video file 
 * 2026-05-06 v1.0.1 Creating .ini file
 * 2025-12-09 v1.0.0 Clock Log
 *
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
          //  Log( "UserName\t" + Environment.UserName); // GDPR
            Log( "OS\t" + System.Runtime.InteropServices.RuntimeInformation.OSDescription );
            Log( "OSArchitecture\t" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture );
            Log( "ProcessArchitecture\t" + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture );
            Log( "Framework\t" + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription );

            InitializeComponent();

            // Load ini file
            if (!File.Exists(appName + ".ini"))
            {
                MessageBox.Show("Missing " + appName + ".ini. Created with {0:HH:mm:ss} 120 White", appName);
                using (StreamWriter sw = File.AppendText(appName + ".ini"))
                    sw.WriteLine("{0:HH:mm:ss}\n120\nWhite");
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
                                if (Int16.TryParse(line, out iWidth1) && iWidth1 > 0)
                                { 
                                Log("Normal width\t"+iWidth1.ToString("0"));
                                this.Width = iWidth1;
                                }
                                break;
                            case 3:
                                Color color = (Color)ColorConverter.ConvertFromString(line);
                                this.Background = new SolidColorBrush(color);
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

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            switch (e.Key)
            {
                case Key.Left:
                    this.Width = Math.Max(this.Width - 10,10);
                    break;

                case Key.Right:
                    this.Width = this.Width + 10;
                    break;

                case Key.Up:
                    this.Height = Math.Max(this.Height- 10,10);
                    break;

                case Key.Down:
                    this.Height = this.Height + 10;
                    break;
            }
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