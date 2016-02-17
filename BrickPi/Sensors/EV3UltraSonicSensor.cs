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
    public sealed class EV3UltraSonicSensor: INotifyPropertyChanged, ISensor
    {
        private Brick brick = null;
        private UltraSonicMode mode;

        public EV3UltraSonicSensor(BrickPortSensor port):this(port, UltraSonicMode.Centimeter, 1000)
        { }

        public EV3UltraSonicSensor(BrickPortSensor port, UltraSonicMode usmode):this(port, usmode, 1000)
        { }

        public EV3UltraSonicSensor(BrickPortSensor port, UltraSonicMode usmode, int timeout)
        {
            brick = new Brick();
            Port = port;
            if (UltraSonicMode.Listen == mode)
                mode = UltraSonicMode.Centimeter;
            mode = usmode;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)BrickSensorType.EV3_US_M0;
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
