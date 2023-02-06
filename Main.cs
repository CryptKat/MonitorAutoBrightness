using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorAutoBrightness
{
    public partial class Main : Form
    {
        private int _previousValue = int.MinValue;
        private int _previousBrightness = int.MinValue;
        private CancellationTokenSource _cts = null;
        private DateTime _lastModified = DateTime.MinValue;

        public Main()
        {
            InitializeComponent();
            var timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1] == "--Minimized")
            {
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;
            }
        }

        private void Main_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                ShowInTaskbar = false;
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
        }

        private void TrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
                WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
            }
        }

        private void CloseAppMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = (System.Timers.Timer)sender;
            timer.Enabled = false;

            var sensorValueLabelText = SensorValueLabel.Text;
            var brightnessValueLabelText = BrightnessValueLabel.Text;

            AdjustBrightness(ref sensorValueLabelText, ref brightnessValueLabelText);

            Invoke((Action)(() =>
            {
                SensorValueLabel.Text = sensorValueLabelText;
                BrightnessValueLabel.Text = brightnessValueLabelText;
            }));

            timer.Enabled = true;
        }

        private void AdjustBrightness(ref string sensorValueLabelText, ref string brightnessValueLabelText)
        {
            int? devicePort = null;
            var portString = ConfigurationManager.AppSettings["SensorUsbPort"];
            if (portString != null)
                devicePort = Convert.ToInt32(portString);
            if (devicePort == 0)
                devicePort = null;

            using (var digiSpark = new ArduinoUsbDevice(0x16c0, 0x05df, devicePort))
            {
                if (!digiSpark.IsAvailable)
                {
                    sensorValueLabelText = "No device";
                    brightnessValueLabelText = "-";
                    return;
                }

                var buffer = new List<byte>();
                Thread.Sleep(500);
                while (digiSpark.ReadByte(out byte value))
                    buffer.Add(value);

                var bufferAsString = System.Text.Encoding.ASCII.GetString(buffer.ToArray());
                var match = Regex.Match(bufferAsString, @".*\r\n(\d+)\r\n");
                if (!match.Success)
                {
                    sensorValueLabelText = "Not found";
                    brightnessValueLabelText = "-";
                    return;
                }

                var newValue = Convert.ToInt32(match.Groups[1].Value);
                bool isValueWrong = newValue < 0 || newValue >= 1023;
                if (isValueWrong)
                {
                    sensorValueLabelText = "Wrong";
                    brightnessValueLabelText = "-";
                    return;
                }

                newValue = 1023 - newValue;
                if (newValue == _previousValue)
                {
                    sensorValueLabelText = _previousValue.ToString();
                    brightnessValueLabelText = _previousBrightness.ToString();
                    return;
                }

                _previousValue = newValue;

                sensorValueLabelText = newValue.ToString();

                int brightness = GetBrightnessLevel(newValue);
                if (brightness == int.MinValue)
                {
                    brightnessValueLabelText = "Not found";
                    return;
                }

                if (brightness == _previousBrightness)
                    return;

                brightnessValueLabelText = brightness.ToString();

                var minimalTimeSpan = Convert.ToDouble(ConfigurationManager.AppSettings["MinimalTimeSpanBetweenAdjustments"]);
                if ((DateTime.Now - _lastModified).TotalSeconds < minimalTimeSpan)
                    return;   

                var previousBrightness = _previousBrightness;

                _cts?.Cancel();

                _cts = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    if (previousBrightness != int.MinValue)
                    {
                        var step = 1;
                        var incrementor = brightness > previousBrightness ? step : -step;
                        for (int i = previousBrightness + incrementor; i != brightness; i += incrementor)
                        {
                            if (_cts.IsCancellationRequested)
                                break;

                            SetBrightnessLevel(i);
                            await Task.Delay(1000, _cts.Token);
                        }
                    }

                    if (!_cts.IsCancellationRequested)
                        SetBrightnessLevel(brightness);

                }, _cts.Token);

                _previousBrightness = brightness;
                _lastModified = DateTime.Now;
            }
        }

        private int GetBrightnessLevel(int sensorValue)
        {
            // Read every time for "hot reload".
            var levelsFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "BrightnessLevels.txt");
            if (!File.Exists(levelsFilePath))
                return int.MinValue;

            var dict = new SortedDictionary<int, int>();

            var fileLines = File.ReadAllLines(levelsFilePath);
            foreach (var line in fileLines)
            {
                var lineParts = line.Split(new[] { ' ', '\t' });
                if (lineParts.Length < 2)
                    continue;

                var lineSensorValue = Convert.ToInt32(lineParts[0]);
                var lineBrightnessLevel = Convert.ToInt32(lineParts[1]);
                dict.Add(lineSensorValue, lineBrightnessLevel);
            }

            dict.Add(0, 0);
            dict.Add(1024, 100);

            int value;
            var index = Array.BinarySearch(dict.Keys.ToArray(), sensorValue);
            if (index >= 0)
                value = dict.ElementAt(index).Value;
            else
            {
                index = -index - 1;
                var point1 = new Point(dict.ElementAt(index - 1).Key, dict.ElementAt(index - 1).Value);
                var point2 = new Point(dict.ElementAt(index).Key, dict.ElementAt(index).Value);
                value = (int)GetY(point1, point2, sensorValue);
            }

            return value;
        }

        private void SetBrightnessLevel(int brightnessLevel)
        {
            var appPath = ConfigurationManager.AppSettings["BrightnessManagerAppExecutablePath"];
            var appArgs = ConfigurationManager.AppSettings["BrightnessManagerAppArgs"];

            using (Process.Start(new ProcessStartInfo
            {
                FileName = Environment.ExpandEnvironmentVariables(appPath),
                Arguments = string.Format(appArgs, brightnessLevel)
            })) { }
        }

        public static float GetY(Point point1, Point point2, float x)
        {
            var m = (float)(point2.Y - point1.Y) / (point2.X - point1.X);
            var b = point1.Y - (m * point1.X);
            return m * x + b;
        }
    }
}
