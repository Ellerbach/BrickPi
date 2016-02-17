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

    class EV3GyroSensor: SensorNotificationBase, ISensor
    {
        private Brick brick = null;
        private GyroMode gmode;

        public EV3GyroSensor(BrickPortSensor port):this(port, GyroMode.Angle)
        { }

        public EV3GyroSensor(BrickPortSensor port, GyroMode mode)
        {
            brick = new Brick();
            Port = port;
            gmode = mode;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)mode;
            brick.SetupSensors();
        }

        /// <summary>
        /// Update the sensor and this will raised an event on the interface
        /// </summary>
        public void UpdateSensor()
        {
            this.Value = ReadRaw();
            this.ValueAsString = ReadAsString();
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
