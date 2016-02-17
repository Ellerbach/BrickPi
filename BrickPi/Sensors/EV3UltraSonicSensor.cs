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
    public sealed class EV3UltraSonicSensor: ISensor
    {
        private Brick brick = null;
        private UltraSonicMode mode;

        public EV3UltraSonicSensor(BrickPortSensor port):this(port, UltraSonicMode.Centimeter)
        { }

        public EV3UltraSonicSensor(BrickPortSensor port, UltraSonicMode usmode)
        {
            brick = new Brick();
            Port = port;
            if (UltraSonicMode.Listen == mode)
                mode = UltraSonicMode.Centimeter;
            mode = usmode;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)BrickSensorType.EV3_US_M0;
            brick.SetupSensors();

        }

        private SensorNotificationBase notification;
        /// <summary>
        /// Update the sensor and this will raised an event on the interface
        /// </summary>
        public void UpdateSensor()
        {
            notification.Value = ReadRaw();
            notification.ValueAsString = ReadAsString();
        }

        /// <summary>
        /// Use this property when you want to get notification into a UI
        /// </summary>
        public SensorNotificationBase Notification
        { get { return notification; } internal set { notification = value; } }

        /// <summary>
        /// Gets or sets the Gyro mode. 
        /// </summary>
        /// <value>The mode.</value>
        public UltraSonicMode Mode
        {
            get { return mode; }
            set {
                if (mode!=value)
                {
                    mode = value;
                    brick.BrickPi.Sensor[(int)Port].Type = GetEV3Type(mode);
                    brick.SetupSensors();
                }
            }
        }

        private BrickSensorType GetEV3Type(UltraSonicMode usmode)
        {
            switch (usmode)
            {
                case UltraSonicMode.Centimeter:
                    return BrickSensorType.EV3_US_M0;
                case UltraSonicMode.Inch:
                    return BrickSensorType.EV3_US_M1;
                case UltraSonicMode.Listen:
                    return BrickSensorType.EV3_US_M2;
                default:
                    return BrickSensorType.EV3_US_M0;
            }
        }


        public BrickPortSensor Port
        {
            get; internal set;
        }

        /// <summary>
        /// Reads the sensor value as a string.
        /// </summary>
        /// <returns>The value as a string</returns>
        public string ReadAsString()
        {
            string s = "";
            switch (mode)
            {
                case UltraSonicMode.Centimeter:
                    s = Read().ToString() + " cm";
                    break;
                case UltraSonicMode.Inch:
                    s = Read().ToString() + " inch";
                    break;
                case UltraSonicMode.Listen:
                    s = Read().ToString();
                    break;
            }
            return s;
        }

        /// <summary>
        /// Read the sensor value. Result depends on the mode
        /// </summary>
        /// <returns>Value as a int</returns>
        public int Read()
        {
            if (Mode == UltraSonicMode.Listen)
            {
                if (brick.BrickPi.Sensor[(int)Port].Value != 0)
                    return 1;
                return 0;
            }
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        /// <summary>
        /// Read the sensor value
        /// </summary>
        /// <returns>Value as a int</returns>
        public int ReadRaw()
        { return brick.BrickPi.Sensor[(int)Port].Value; }

        public string GetSensorName()
        {
            return "EV3 Ultrasonic";
        }

        public void SelectNextMode()
        {
            Mode = Mode.Next();
            return;
        }

        public void SelectPreviousMode()
        {
            Mode = Mode.Previous();
            return;
        }

        public int NumberOfModes()
        {
            return Enum.GetNames(typeof(UltraSonicMode)).Length;

        }

        public string SelectedMode()
        {
            return Mode.ToString();
        }
    }
}
