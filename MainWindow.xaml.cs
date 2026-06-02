/*
 * Displaying clock, keep log
 * .NET48 x64
 * @jussivirkkala 
 * 2026-06-02 Adding OS build.
 * 2026-05-28 Reading .ini every 15 second.
 * 2026-05-06 Creating .ini file
 * 2025-12-09 Clock Log
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
using Microsoft.Win32; // RegistryKey

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
        
        void dispatcherClock_Tick1(object sender, EventArgs e)
        {
            try
            {
              Time.Content = String.Format(sFormat,DateTime.Now.AddMilliseconds(500), Environment.MachineName);
            }
            catch (Exception)
            {
                Time.Content = ".ini error";
            }
		}
		void dispatcherClock_Tick2(object sender, EventArgs e)
		{
            try
            {
                int row = 0;
                string s = appName + ".ini";
                if (File.Exists(appName + "-" + Environment.MachineName + ".ini"))
                    s = appName + "-" + Environment.MachineName + ".ini";

                foreach (string line in File.ReadLines(s))
                {
                    if (!line.StartsWith("#"))
                    {
                        row += 1;
                        switch (row)
                        {
                            case 1:
                                if (!line.Equals(sFormat))
                                { 
                                    sFormat = line;
								    Time.Content = String.Format(sFormat, DateTime.Now.AddMilliseconds(500), Environment.MachineName);
                                    this.UpdateLayout();
                                    Time.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
								    this.Width = Time.DesiredSize.Width;
                                    if (sFormat.Equals(""))
                                        this.Visibility= Visibility.Hidden;
                                    else
										this.Visibility = Visibility.Visible;
									Log("Format\t" + sFormat);
								}
								break;
                            case 2:
                                Color color = (Color)ColorConverter.ConvertFromString(line);
                                this.Background = new SolidColorBrush(color);
                                break;
                        }
                    }
                    Time.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                    this.Width = Time.DesiredSize.Width;
                }
            }
            catch (Exception)
            {
                sFormat= "READ ERROR";
                Time.Content = sFormat;
				this.Visibility = Visibility.Visible;
				Time.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
				this.Width = Time.DesiredSize.Width;
			}
		}

       

		public MainWindow()
        {
            appName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

            Log( "Started\t" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).Comments);
            Log( "Version\t" + FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion );
            Log( "MachineName\t" + Environment.MachineName);
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
			var buildNumber = registryKey.GetValue("UBR").ToString();

			Log( "OS\t" + System.Runtime.InteropServices.RuntimeInformation.OSDescription.Trim()+"."+ buildNumber.ToString());
            Log( "OSArchitecture\t" + System.Runtime.InteropServices.RuntimeInformation.OSArchitecture );
            Log( "ProcessArchitecture\t" + System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture );
            Log( "Framework\t" + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription );

            InitializeComponent();

            // Load ini file
            if (!File.Exists(appName + ".ini"))
            {
                using (StreamWriter sw = File.AppendText(appName + ".ini"))
                    sw.WriteLine("{0:HH:mm:ss}\nWhite");
            }

                int row = 0;
                string s = appName + ".ini";
                if (File.Exists(appName + "-" + Environment.MachineName + ".ini"))
                    s = appName + "-" + Environment.MachineName + ".ini";

                foreach (string line in File.ReadLines(s))
                {
                    if (!line.StartsWith("#"))
                    {
                        row += 1;
                        switch (row)
                        {
                            case 1:
                                sFormat= line;
    							if (sFormat.Equals(""))
	    							this.Visibility = Visibility.Hidden;
		    					else
			    					this.Visibility = Visibility.Visible;
				    			Time.Content = String.Format(sFormat, DateTime.Now.AddMilliseconds(500), Environment.MachineName);
								Time.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
								this.Width = Time.DesiredSize.Width;
                                Log("Format\t"+ sFormat);

							    break;
                            case 2:
                                Color color = (Color)ColorConverter.ConvertFromString(line);
                                this.Background = new SolidColorBrush(color);
                                break;

                        }
                        if (sFormat.Contains("{0"))
                        {
                            DispatcherTimer dispatcherTimer1 = new System.Windows.Threading.DispatcherTimer();
                            dispatcherTimer1.Tick += new EventHandler(dispatcherClock_Tick1);
                            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, 500);
					    	Log("Timer1 started");
					}
				}
				

            }
			DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
			dispatcherTimer2.Tick += new EventHandler(dispatcherClock_Tick2);
			dispatcherTimer2.Interval = new TimeSpan(0, 0, 0, 15, 0);
			dispatcherTimer2.Start();
			Log("Timer2 started");
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