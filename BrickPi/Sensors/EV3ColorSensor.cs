//////////////////////////////////////////////////////////
// This code has been originally created by Laurent Ellerbach
// It intend to make the excellent BrickPi from Dexter Industries working
// on a RaspberryPi 2 runing Windows 10 IoT Core in Universal
// Windows Platform.
// Credits:
// - Dexter Industries Code
// - MonoBrick for great inspiration regarding sensors implementation in C#
//
// This code is under https://opensource.org/licenses/ms-pl
//
//////////////////////////////////////////////////////////

using BrickPi.Extensions;
using System;

namespace BrickPi.Sensors
{
    class EV3ColorSensor: SensorNotificationBase, ISensor
    {
        private Brick brick = null;
        private ColorSensorMode colorMode;
        private Int16[] rawValues = new Int16[4];

        private const int RedIndex = 0;
        private const int GreenIndex = 1;
        private const int BlueIndex = 2;
        private const int BackgroundIndex = 3;

        public EV3ColorSensor(BrickPortSensor port):this(port,ColorSensorMode.Color)
        { }

        public EV3ColorSensor(BrickPortSensor port, ColorSensorMode mode)
        {
            brick = new Brick();
            Port = port;
            colorMode = mode;
            brick.BrickPi.Sensor[(int)Port].Type = GetEV3Mode(mode);
            brick.SetupSensors();

        }

        private BrickSensorType GetEV3Mode(ColorSensorMode mode)
        {
            BrickSensorType ret = BrickSensorType.EV3_COLOR_M0;
            switch (mode)
            {
                //question is about the various modes and if theyr are supported and how
                case ColorSensorMode.Color:
                    ret = BrickSensorType.EV3_COLOR_M3;
                    break;
                case ColorSensorMode.Reflection:
                    ret = BrickSensorType.EV3_COLOR_M0;
                    break;
                case ColorSensorMode.Green:
                    ret = BrickSensorType.EV3_COLOR_M2;
                    break;
                case ColorSensorMode.Blue:
                    ret = BrickSensorType.EV3_COLOR_M2;
                    break;
                case ColorSensorMode.Ambient:
                    ret = BrickSensorType.EV3_COLOR_M1; 
                    break;
            }
            return ret;

        }

        public ColorSensorMode ColorMode
        {
            get { return colorMode; }
            set
            {
                if (value != colorMode)
                {
                    colorMode = value;
                    brick.BrickPi.Sensor[(int)Port].Type = GetEV3Mode(colorMode);
                    brick.SetupSensors();
                }
            }

            
        }

        /// <summary>
        /// Update the sensor and this will raised an event on the interface
        /// </summary>
        public void UpdateSensor()
        {
            this.Value = ReadRaw();
            this.ValueAsString = ReadAsString();
        }

        private void GetRawValues()
        {
            rawValues[RedIndex] = (short)(brick.BrickPi.Sensor[(int)Port].Value & 0xFF);
            rawValues[GreenIndex] = (short)((brick.BrickPi.Sensor[(int)Port].Value >> 8)& 0xFF);
            rawValues[BlueIndex] = (short)((brick.BrickPi.Sensor[(int)Port].Value >> 16) & 0xFF);
            rawValues[BackgroundIndex] = (short)((brick.BrickPi.Sensor[(int)Port].Value >> 24) & 0xFF);
        }

        public int ReadRaw()
        {
            int val = 0;
            switch (colorMode)
            {
                case ColorSensorMode.Color:
                    val = (int)ReadColor();
                    break;
                case ColorSensorMode.Reflection:
                case ColorSensorMode.Green:
                case ColorSensorMode.Blue:
                    val = CalculateRawAverage();
                    break;
                case ColorSensorMode.Ambient:
                    val = CalculateRawAverage();
                    break;
            }
            return val;
        }

        /// <summary>
        /// Read the intensity of the reflected or ambient light in percent. In color mode the color index is returned
        /// </summary>
        public int Read()
        {
            int val = 0;
            switch (colorMode)
            {
                case ColorSensorMode.Ambient:
                    val = CalculateRawAverageAsPct();
                    break;
                case ColorSensorMode.Color:
                    val = (int)ReadColor();
                    break;
                case ColorSensorMode.Reflection:
                    val = CalculateRawAverageAsPct();
                    break;
                default:
                    val = CalculateRawAverageAsPct();
                    break;
            }
            return val;
        }

        private int CalculateRawAverage()
        {
            if (colorMode == ColorSensorMode.Color)
            {
                GetRawValues();
                return (int)(rawValues[RedIndex] + rawValues[BlueIndex] + rawValues[GreenIndex]) / 3;
            }
            else
                return brick.BrickPi.Sensor[(int)Port].Value;
        }

        private int CalculateRawAverageAsPct()
        {
            //Need to find out what is the ADC resolution
            //1023 is probablt not the correct one
            return (CalculateRawAverage() * 100) / 1023;
        }

        public string ReadTest()
        {
            GetRawValues();
            string ret = "";
            for (int i = 0; i < rawValues.Length; i++)
                ret += " " + rawValues[i];
            ret += " " + brick.BrickPi.Sensor[(int)Port].Value;
            return ret;

        }

        public string ReadAsString()
        {
            string s = "";
            switch (colorMode)
            {
                case ColorSensorMode.Color:
                    s = ReadColor().ToString();
                    break;
                case ColorSensorMode.Reflection:
                case ColorSensorMode.Green:
                case ColorSensorMode.Blue:
                    s = Read().ToString();
                    break;
                case ColorSensorMode.Ambient:
                    s = Read().ToString();
                    break;
            }

            return s;
        }

        /// <summary>
        /// Reads the color.
        /// </summary>
        /// <returns>The color.</returns>
        public Color ReadColor()
        {
            Color color = Color.None;
            if (colorMode == ColorSensorMode.Color)
            {
                color = CalculateColor();
            }
            return color;
        }

        /// <summary>
        /// Reads the color of the RGB.
        /// </summary>
        /// <returns>The RGB color.</returns>
        public RGBColor ReadRGBColor()
        {
            GetRawValues();
            return new RGBColor((byte)rawValues[RedIndex], (byte)rawValues[GreenIndex], (byte)rawValues[BlueIndex]);
        }

        private Color CalculateColor()
        {
            //Taken from the LeJos source code - thanks ;-)
            GetRawValues();
            int red = rawValues[RedIndex];
            int blue = rawValues[BlueIndex];
            int green = rawValues[GreenIndex];
            int blank = rawValues[BackgroundIndex];
            // we have calibrated values, now use them to determine the color

            // The following algorithm comes from the 1.29 Lego firmware.
            if (red > blue && red > green)
            {
                // Red dominant color
                if (red < 65 || (blank < 40 && red < 110))
                    return Color.Black;
                if (((blue >> 2) + (blue >> 3) + blue < green) &&
                        ((green << 1) > red))
                    return Color.Yellow;
                if ((green << 1) - (green >> 2) < red)
                    return Color.Red;
                if (blue < 70 || green < 70 || (blank < 140 && red < 140))
                    return Color.Black;
                return Color.White;
            }
            else if (green > blue)
            {
                // Green dominant color
                if (green < 40 || (blank < 30 && green < 70))
                    return Color.Black;
                if ((blue << 1) < red)
                    return Color.Yellow;
                if ((red + (red >> 2)) < green ||
                        (blue + (blue >> 2)) < green)
                    return Color.Green;
                if (red < 70 || blue < 70 || (blank < 140 && green < 140))
                    return Color.Black;
                return Color.White;
            }
            else
            {
                // Blue dominant color
                if (blue < 48 || (blank < 25 && blue < 85))
                    return Color.Black;
                if ((((red * 48) >> 5) < blue && ((green * 48) >> 5) < blue) ||
                        ((red * 58) >> 5) < blue || ((green * 58) >> 5) < blue)
                    return Color.Blue;
                if (red < 60 || green < 60 || (blank < 110 && blue < 120))
                    return Color.Black;
                if ((red + (red >> 3)) < blue || (green + (green >> 3)) < blue)
                    return Color.Blue;
                return Color.White;
            }
        }


        public BrickPortSensor Port
        {
            get; internal set;
        }

        public string GetSensorName()
        {
            return "EV3 Color Sensor";
        }

        public void SelectNextMode()
        {
            colorMode = ColorMode.Next();
            return;
        }

        public void SelectPreviousMode()
        {
            colorMode = ColorMode.Previous();
            return;
        }

        public int NumberOfModes()
        {
            return Enum.GetNames(typeof(ColorSensorMode)).Length;
        }

        public string SelectedMode()
        {
            return ColorMode.ToString();
        }
    }
}
