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
using System.ComponentModel;
using System.Threading;

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
    public sealed class NXTUltraSonicSensor: INotifyPropertyChanged, ISensor
    {
        private Brick brick = null;
        private UltraSonicMode sonarMode;

        public NXTUltraSonicSensor(BrickPortSensor port):this(port, UltraSonicMode.Centimeter, 1000)
        { }
        public NXTUltraSonicSensor(BrickPortSensor port, UltraSonicMode mode):this(port, mode, 1000)
        { }
        public NXTUltraSonicSensor(BrickPortSensor port, UltraSonicMode mode, int timeout)
        {
            brick = new Brick();
            Port = port;
            if (UltraSonicMode.Listen == mode)
                mode = UltraSonicMode.Centimeter;
            sonarMode = mode;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)BrickSensorType.ULTRASONIC_CONT;
            brick.SetupSensors();
            timer = new Timer(UpdateSensor, this, TimeSpan.FromMilliseconds(timeout), TimeSpan.FromMilliseconds(timeout));
        }

        private Timer timer = null;
        private void StopTimerInternal()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public BrickPortSensor Port
        {
            get; internal set;
        }
        private int value;
        private string valueAsString;

        /// <summary>
        /// Return the raw value of the sensor
        /// </summary>
        public int Value
        {
            get { return value; }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Return the raw value  as a string of the sensor
        /// </summary>
        public string ValueAsString
        {
            get { return valueAsString; }
            set
            {
                if (valueAsString != value)
                {
                    valueAsString = value;
                    OnPropertyChanged(nameof(ValueAsString));
                }
            }
        }
        /// <summary>
        /// Update the sensor and this will raised an event on the interface
        /// </summary>
        public void UpdateSensor(object state)
        {
            Value = ReadRaw();
            ValueAsString = ReadAsString();
        }

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
