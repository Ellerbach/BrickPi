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
    /// IR channels
    /// </summary>
    public enum IRChannel
    {
#pragma warning disable
        One = 0, Two = 1, Three = 2, Four = 3
#pragma warning restore
    };


    /// <summary>
    /// Sensor mode when using a EV3 IR Sensor
    /// </summary>
    public enum IRMode
    {
        /// <summary>
        /// Use the IR sensor as a distance sensor
        /// </summary>
        Proximity = BrickSensorType.EV3_INFRARED_M0,

        /// <summary>
        /// Use the IR sensor to detect the location of the IR Remote
        /// </summary>
        Seek = BrickSensorType.EV3_INFRARED_M1,

        /// <summary>
        /// Use the IR sensor to detect wich Buttons where pressed on the IR Remote
        /// </summary>
        Remote = BrickSensorType.EV3_INFRARED_M2,
    };

    /// <summary>
    /// Class for IR beacon location.
    /// </summary>
    public sealed class BeaconLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoBrickFirmware.IO.IRBeaconLocation"/> class.
        /// </summary>
        /// <param name="location">Location.</param>
        /// <param name="distance">Distance.</param>
        public BeaconLocation(int location, int distance) { this.Location = location; this.Distance = distance; }

        /// <summary>
        /// Gets the location of the beacon ranging from minus to plus increasing clockwise when pointing towards the beacon
        /// </summary>
        /// <value>The location of the beacon.</value>
        public int Location { get; private set; } //was sbyte

        /// <summary>
        /// Gets the distance of the beacon in CM (0-100)
        /// </summary>
        /// <value>The distance to the beacon.</value>
        public int Distance { get; private set; } //was sbyte

    }

    class EV3IRSensor:ISensor
    {
        private Brick brick = null;
        private IRMode mode;

        public EV3IRSensor(BrickPortSensor port) : this(port, IRMode.Proximity)
		{

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoBrick.EV3.IRSensor"/> class.
        /// </summary>
        /// <param name="mode">Mode.</param>
        public EV3IRSensor(BrickPortSensor port, IRMode mode)
        {
            brick = new Brick();
            Mode = mode;
            Channel = IRChannel.One;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)mode;
            brick.SetupSensors();
        }

        /// <summary>
        /// Gets or sets the IR mode. 
        /// </summary>
        /// <value>The mode.</value>
        public IRMode Mode
        {
            get { return mode; }
            set {
                if (mode != value)
                {
                    mode = value;
                    brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)mode;
                    brick.SetupSensors();
                } 
            }
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
                case IRMode.Proximity:
                    s = ReadDistance() + " cm";
                    break;
                case IRMode.Remote:
                    s = ReadRemoteCommand() + " on channel " + Channel;
                    break;
                case IRMode.Seek:
                    //BeaconLocation location = ReadBeaconLocation();
                    s = "Location: " + ReadBeaconLocation() + " Distance: TBD cm";
                    break;
            }
            return s;
        }

        /// <summary>
        /// Read the sensor value. The returned value depends on the mode. Distance in proximity mode. 
        /// Remote command number in remote mode. Beacon location in seek mode. 
        /// </summary>
        public int Read()
        {
            int value = 0;
            switch (Mode)
            {
                case IRMode.Proximity:
                    value = ReadDistance();
                    break;
                case IRMode.Remote:
                    value = (int)ReadRemoteCommand();
                    break;
                case IRMode.Seek:
                    value = (int)ReadBeaconLocation(); //if using class beacon: .Location
                    break;
            }
            return value;
        }
        /// <summary>
        /// Read the sensor value
        /// </summary>
        /// <returns>Value as a int</returns>
        public int ReadRaw()
        { return brick.BrickPi.Sensor[(int)Port].Value; }

        /// <summary>
        /// Read the distance of the sensor in CM (0-100). This will change mode to proximity
        /// </summary>
        public int ReadDistance()
        {
            if (mode != IRMode.Proximity)
            {
                Mode = IRMode.Proximity;
            }
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        /// <summary>
        /// Reads commands from the IR-Remote. This will change mode to remote
        /// </summary>
        /// <returns>The remote command.</returns>
        public byte ReadRemoteCommand()
        {
            if (Mode != IRMode.Remote)
            {
                Mode = IRMode.Remote;
            }
            return (byte)((brick.BrickPi.Sensor[(int)Port].Value >> (int)Channel) & 0x0F);
        }

        /// <summary>
        /// Gets the beacon location. This will change the mode to seek
        /// </summary>
        /// <returns>The beacon location.</returns>
        public int ReadBeaconLocation()
        {
            if (Mode != IRMode.Seek)
            {
                Mode = IRMode.Seek;
            }
            //byte[] data = mUartSensor.ReadBytes(4 * 2);
            //return new BeaconLocation((sbyte)data[(int)Channel * 2], (sbyte)data[((int)Channel * 2) + 1]);
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        /// <summary>
        /// Gets or sets the IR channel used for reading remote commands or beacon location
        /// </summary>
        /// <value>The channel.</value>
        public IRChannel Channel { get; set; }

        public BrickPortSensor Port
        {
            get; internal set;
        }

        public string GetSensorName()
        {
            return "EV3 IR";
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
            return Enum.GetNames(typeof(IRMode)).Length;

        }

        public string SelectedMode()
        {
            return Mode.ToString();
        }
    }
}
