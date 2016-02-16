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

namespace BrickPi
{

    /// <summary>
    /// Used to store Sensor data in the main BrickPi structure
    /// data includes an array used for color sensors, Type and default values as well as wettings
    /// NOTE: this structure is to be compatible with initial code, don't use like this
    /// </summary>
    public sealed class BrickSensor
    {
        private int val;
        private int[] array = new int[4];
        private BrickSensorType type;
        private int[] settings = new int[8];

        /// <summary>
        /// Main returned value from the sensor
        /// </summary>
        public int Value
        { get { return val; } set { val = value; } }

        /// <summary>
        /// Arrays of 4 int, storing data like colors from sensors
        /// </summary>
        public int[] Array
        { get { return array; } set { array = value; }         }

        /// <summary>
        /// Store the type of sensor
        /// </summary>
        public BrickSensorType Type
        { get { return type; } set { type = value; } }

        /// <summary>
        /// Stores I2C specific settings
        /// </summary>
        public int[] Settings { get { return settings; } set { settings = value; } }
    }

    /// <summary>
    /// Used to store data from I2C sensors or sensors you are considering as I2C
    /// </summary>
    public sealed class BrickSensorI2C
    {
        private int devices;
        private int speed;
        private int[] address = new int[8];
        private int[] write = new int[8];
        private int[] read = new int[8];
        private BrickSensorI2CInOut[] oout = new BrickSensorI2CInOut[8];
        private BrickSensorI2CInOut[] iin = new BrickSensorI2CInOut[8];

        /// <summary>
        /// initialize SensorI2C class
        /// </summary>
        public BrickSensorI2C()
        {
            for(int i=0; i<oout.Length;i++)
            {
                oout[i] = new BrickSensorI2CInOut();
                iin[i] = new BrickSensorI2CInOut();
            }
        }

        /// <summary>
        /// Store number of devices in the I2C chain sensor
        /// </summary>
        public int Devices
        { get { return devices; } set { devices = value; } }

        /// <summary>
        /// I2C Speed. Please see constants from the project to use it
        /// mainly used to tweak the UltraSonic sensor
        /// </summary>
        public int Speed
        { get { return speed; } set { speed = value; } }

        /// <summary>
        /// Address of the I2C device. Please refer to your I2C device to find out this value
        /// </summary>
        public int[] Address
        { get { return address; } set { address = value; } }

        /// <summary>
        /// I2C data to write
        /// </summary>
        public int[] Write
        { get { return write; } set { write = value; } }

        /// <summary>
        /// I2C data to read
        /// </summary>
        public int[] Read
        { get { return read; } set { read = value; } }

        /// <summary>
        /// Out I2C data for transfer in and out
        /// </summary>
        public BrickSensorI2CInOut[] Out
        { get { return oout; } set { oout = value; } }

        /// <summary>
        /// In I2C data for transfer in and out
        /// </summary>
        public BrickSensorI2CInOut[] In
        { get { return iin; } set { iin = value; } }
    }

    /// <summary>
    /// In and Out data for I2C sensor
    /// </summary>
    public sealed class BrickSensorI2CInOut
    {
        private int[] inOut = new int[16];

        public int[] InOut
        { get { return inOut; } set { inOut = value; } }
    }


    /// <summary>
    /// Contains all motor information
    /// </summary>
    public sealed class BrickMotor
    {
        private int motorSpeed;
        private int motorEnable;
        private int encoderOffset;
        private int encoder;
        /// <summary>
        /// Set the speed of motors, max is 255 and min is -255, 0 is stopped
        /// </summary>
        public int Speed
        { get { return motorSpeed; } set { motorSpeed = value; } }

        /// <summary>
        /// Enable motors with 1, stop with 0
        /// </summary>
        public int Enable
        { get { return motorEnable; } set { motorEnable = value; } }

        /// <summary>
        /// Change the encoder offset
        /// </summary>
        public int EncoderOffset
        { get { return encoderOffset; } set { encoderOffset = value; } }

        /// <summary>
        /// Encoder of the motors, 1 = 0.5 degreese, 720 = 360 degrees
        /// </summary>
        public int Encoder
        { get { return encoder; } set { encoder = value; } }

    }

    /// <summary>
    /// Main structure containing all sensor data
    /// it does contains Motor info, Sensor as well as I2C
    /// </summary>
    public sealed class BrickPiStruct
    {
        private int[] address = new int[2];
        private BrickSensor[] sensor = new BrickSensor[4];
        private BrickSensorI2C[] i2C = new BrickSensorI2C[4];
        private int timeout = 1000;
        private BrickMotor[] motor = new BrickMotor[4];
        /// <summary>
        /// initialiaze BrickPiStruct
        /// </summary>
        public BrickPiStruct()
        {
            //address for the Arduinos, 1 and 2
            address[0] = 1;
            address[1] = 2;
            for (int i = 0; i < i2C.Length; i++)
                i2C[i] = new BrickSensorI2C();
            for (int i = 0; i < sensor.Length; i++)
            { 
                //create new sensors
                sensor[i] = new BrickSensor();
                //initial setup is RAW sensor
                sensor[i].Value = (int)BrickSensorType.SENSOR_RAW;
            }
        }
        //no set
        /// <summary>
        /// Read address of the Arduinos
        /// Note: changing this value does not change it on the Arduino
        /// </summary>
        public int[] Address
        { get { return address; } set { address = value; } }

        /// <summary>
        /// Contains all Motor information speed, enabling motors, encoders
        /// </summary>
        public BrickMotor[] Motor
        { get { return motor; } set { motor = value; } }
        /// <summary>
        /// Changing timeout on the Arduinos
        /// Note: changing this value does not change it on the Arduino
        /// </summary>
        public int Timeout
        { get { return timeout; } set {
                if (value < 0)
                    value = 0;
                timeout = value;
            }
        }

        /// <summary>
        /// Contains all the Sensor info
        /// </summary>
        public BrickSensor[] Sensor
        { get { return sensor; } set { sensor = value; } }

        /// <summary>
        /// Constains all I2C sensor info
        /// </summary>
        public BrickSensorI2C[] I2C
        { get { return i2C; } set { i2C = value; } }
    }
}
