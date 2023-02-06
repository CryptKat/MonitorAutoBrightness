using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace MonitorAutoBrightness
{
    public partial class Main : Form
    {
        private int previousValue = 0;

        public Main()
        {
            InitializeComponent();
            var timer = new System.Timers.Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 1000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var sensorValueLabelText = SensorValueLabel.Text;
            var brightnessValueLabelText = BrightnessValueLabel.Text;

            AdjustBrightness(ref sensorValueLabelText, ref brightnessValueLabelText);

            Invoke((Action)(() =>
            {
                SensorValueLabel.Text = sensorValueLabelText;
                BrightnessValueLabel.Text = brightnessValueLabelText;
            }));
        }

        private void AdjustBrightness(ref string sensorValueLabelText, ref string brightnessValueLabelText)
        {
            using (var digiSpark = new ArduinoUsbDevice())
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
                var match = Regex.Match(bufferAsString, @".+\r\n(\d+)\r\n");
                if (!match.Success)
                {
                    sensorValueLabelText = "Value not found";
                    brightnessValueLabelText = "-";
                    return;
                }

                var newValue = Convert.ToInt32(match.Groups[1].Value);
                if (newValue == previousValue)
                    return;

                previousValue = newValue;
                bool isValueWrong = newValue < 0 || newValue >= 1023;
                if (isValueWrong)
                {
                    sensorValueLabelText = "Wrong value";
                    brightnessValueLabelText = "-";
                    return;
                }

                newValue = 1023 - newValue;
                sensorValueLabelText = newValue.ToString();

                int brightness = GetBrightnessLevel(newValue);
                if (brightness == int.MinValue)
                {
                    brightnessValueLabelText = "Not found";
                    return;
                }

                brightnessValueLabelText = brightness.ToString();
                var appPath = ConfigurationManager.AppSettings["BrightnessManagerAppExecutablePath"];
                var appArgs = ConfigurationManager.AppSettings["BrightnessManagerAppArgs"];

                using (Process.Start(new ProcessStartInfo
                {
                    FileName = Environment.ExpandEnvironmentVariables(appPath),
                    Arguments = string.Format(appArgs, brightness)
                })) { }
            }
        }

        private int GetBrightnessLevel(int sensorValue)
        {
            // Read every time for "hot reload".
            var levelsFilePath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "BrightnessLevels.txt");
            if (!File.Exists(levelsFilePath))
                return int.MinValue;

            var fileLines = File.ReadAllLines(levelsFilePath);
            foreach (var line in fileLines)
            {
                var lineParts = line.Split(new[] { ' ', '\t' });
                if (lineParts.Length < 2)
                    continue;
                try
                {
                    var lineSensorValue = Convert.ToInt32(lineParts[0]);
                    var lineBrightnessLevel = Convert.ToInt32(lineParts[1]);

                    if (sensorValue <= lineSensorValue)
                        return lineBrightnessLevel;
                }
                catch (FormatException) { }
            }

            return int.MinValue;
        }
    }
}
