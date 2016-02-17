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

    /// <summary>
	/// Sensor mode when using a Sonar sensor
	/// </summary>
    public enum UltraSonicMode
    {
        /// <summary>
        /// Result will be in centimeter
        /// </summary>
        Centimeter = BrickSensorType.EV3_US_M0, 

        /// <summary>
        /// Result will be in centi-inch
        /// </summary>
        Inch = BrickSensorType.EV3_US_M1, 

        /// <summary>
        /// Sensor is in listen mode
        /// </summary>
        Listen = BrickSensorType.EV3_US_M2, 
    };
    public sealed class NXTUltraSonicSensor: ISensor
    {
        private Brick brick = null;
        private UltraSonicMode sonarMode;

        public NXTUltraSonicSensor(BrickPortSensor port):this(port, UltraSonicMode.Centimeter)
        { }

        public NXTUltraSonicSensor(BrickPortSensor port, UltraSonicMode mode)
        {
            brick = new Brick();
            Port = port;
            if (UltraSonicMode.Listen == mode)
                mode = UltraSonicMode.Centimeter;
            sonarMode = mode;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)BrickSensorType.ULTRASONIC_CONT;
            brick.SetupSensors();

        }

        public BrickPortSensor Port
        {
            get; internal set;
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
        /// Gets or sets the sonar mode.
        /// </summary>
        /// <value>
        /// The sonar mode 
        /// </value>
        public UltraSonicMode Mode
        {
            get { return sonarMode; }
            set { sonarMode = value; }
        }

        public string GetSensorName()
        {
            return "NXT Ultrasonic";
        }

        public string ReadAsString()
        {
            string s = ReadDistance().ToString();
            if (Mode == UltraSonicMode.Inch)
                s = s + " inch";
            else
                s = s + " cm";
            return s;

        }

        /// <summary>
        /// Read the distance in either centiinches or centimeter
        /// </summary>
        /// <returns>Distance as a float</returns>
        public float ReadDistance()
        {
            int reading = brick.BrickPi.Sensor[(int)Port].Value;
            if (Mode == UltraSonicMode.Inch)
                return (reading * 39370) / 100;
            return reading;
        }

        /// <summary>
        /// The raw value from the sensor
        /// </summary>
        /// <returns>Value as a int</returns>
        public int ReadRaw()
        {
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        public void SelectNextMode()
        {
            Mode = Mode.Next();
            if (Mode == UltraSonicMode.Listen)
                Mode = Mode.Next();
            return;
        }

        public void SelectPreviousMode()
        {
            Mode = Mode.Previous();
            if (Mode == UltraSonicMode.Listen)
                Mode = Mode.Previous();
            return;
        }

        public int NumberOfModes()
        {
            return Enum.GetNames(typeof(UltraSonicMode)).Length - 1;//listen mode not supported
        }

        public string SelectedMode()
        {
            return Mode.ToString();
        }
    }
}
