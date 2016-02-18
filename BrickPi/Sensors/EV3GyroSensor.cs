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
using System.Threading.Tasks;

namespace BrickPi.Sensors
{
    /// <summary>
    /// Sensor modes when using a EV3 Gyro sensor
    /// </summary>
    public enum GyroMode
    {
#pragma warning disable
        /// <summary>
        /// Result will be in degrees
        /// </summary>
        Angle = BrickSensorType.EV3_GYRO_M0,
        /// <summary>
        /// Result will be in degrees per second
        /// </summary>
        AngularVelocity = BrickSensorType.EV3_GYRO_M1,
#pragma warning restore
    };

    public sealed class EV3GyroSensor: INotifyPropertyChanged, ISensor
    {
        private Brick brick = null;
        private GyroMode gmode;

        public EV3GyroSensor(BrickPortSensor port):this(port, GyroMode.Angle)
        { }

        public EV3GyroSensor(BrickPortSensor port, GyroMode mode):this(port, mode, 1000)
        { }

        public EV3GyroSensor(BrickPortSensor port, GyroMode mode, int timeout)
        {
            brick = new Brick();
            Port = port;
            gmode = mode;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)mode;
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
        /// To notify a property has changed. The minimum time can be set up
        /// with timeout property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private int periodRefresh;
        /// <summary>
        /// Period to refresh the notification of property changed in milliseconds
        /// </summary>
        public int PeriodRefresh
        {
            get { return periodRefresh; }
            set
            {
                periodRefresh = value;
                timer.Change(TimeSpan.FromMilliseconds(periodRefresh), TimeSpan.FromMilliseconds(periodRefresh));
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
        public GyroMode Mode
        {
            get { return gmode; }
            set {
                if(gmode != value)
                {
                    gmode = value;
                    brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)gmode;
                }
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
            switch (gmode)
            {
                case GyroMode.Angle:
                    s = Read().ToString() + " degree";
                    break;
                case GyroMode.AngularVelocity:
                    s = Read().ToString() + " deg/sec";
                    break;
            }
            return s;
        }

        /// <summary>
        /// Reset the sensor
        /// </summary>
        public new async void Reset()
        {
            if (Mode == GyroMode.Angle)
            {
                Mode = GyroMode.AngularVelocity;
                //System.Threading.Thread.Sleep(100);
                await Task.Delay(100);
                Mode = GyroMode.Angle;
            }
            else
            {
                Mode = GyroMode.Angle;
                //System.Threading.Thread.Sleep(100);
                await Task.Delay(100);
                Mode = GyroMode.AngularVelocity;
            }
        }

        /// <summary>
        /// Get the number of rotations (a rotation is 360 degrees) - only makes sense when in angle mode
        /// </summary>
        /// <returns>The number of rotations</returns>
        public int RotationCount()
        {
            if (Mode == GyroMode.Angle)
            {
                return brick.BrickPi.Sensor[(int)Port].Value / 360;
            }
            return 0;
        }


        /// <summary>
        /// Read the gyro sensor value. The returned value depends on the mode. 
        /// </summary>
        public int Read()
        {
            if (Mode == GyroMode.Angle)
            {
                return brick.BrickPi.Sensor[(int)Port].Value % 360;
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
            return "EV3 Gyro";
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
            return Enum.GetNames(typeof(GyroMode)).Length;

        }

        public string SelectedMode()
        {
            return Mode.ToString();
        }
    }
}
