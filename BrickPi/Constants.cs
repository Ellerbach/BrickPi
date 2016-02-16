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
    /// Motor ports A, B, C and D
    /// </summary>
    public enum BrickPortMotor
    {
        PORT_A = 0,
        PORT_B = 1,
        PORT_C = 2,
        PORT_D = 3
    }

    /// <summary>
    /// Sensor ports 1, 2, 3 and 4
    /// </summary>
    public enum BrickPortSensor
    {
        PORT_S1 = 0,
        PORT_S2 = 1,
        PORT_S3 = 2,
        PORT_S4 = 3
    }

    /// <summary>
    /// All possible type of sensors
    /// </summary>
    public enum BrickSensorType
    {
        SENSOR_RAW = 0,
        LIGHT_OFF = 0,
        LIGHT_ON = (0x01 | 0x08),
        TOUCH = 32,
        ULTRASONIC_CONT = 33,
        ULTRASONIC_SS = 34,
        RCX_LIGHT = 35, // # tested minimally
        COLOR_FULL = 36,
        COLOR_RED = 37,
        COLOR_GREEN = 38,
        COLOR_BLUE = 39,
        COLOR_NONE = 40,
        I2C = 41,
        I2C_9V = 42,

        //# Mode information for EV3 is here: https://github.com/mindboards/ev3dev/wiki/LEGO-EV3-Ultrasonic-Sensor-%2845504%29

        EV3_US_M0 = 43, //	# Continuous measurement, distance, cm
        EV3_US_M1 = 44, //	# Continuous measurement, distance, in
        EV3_US_M2 = 45, //	# Listen // 0 r 1 depending on presence of another US sensor.
        EV3_US_M3 = 46,
        EV3_US_M4 = 47,
        EV3_US_M5 = 48,
        EV3_US_M6 = 49,

        EV3_COLOR_M0 = 50, //	# Reflected
        EV3_COLOR_M1 = 51, //	# Ambient
        EV3_COLOR_M2 = 52, //	# Color  // Min is 0, max is 7 (brown)
        EV3_COLOR_M3 = 53, //	# Raw reflected
        EV3_COLOR_M4 = 54, //	# Raw Color Components
        EV3_COLOR_M5 = 55, //	# Calibration???  Not currently implemented.

        EV3_GYRO_M0 = 56, //	# Angle
        EV3_GYRO_M1 = 57, //	# Rotational Speed
        EV3_GYRO_M2 = 58, // 	# Raw sensor value ???
        EV3_GYRO_M3 = 59, //	# Angle and Rotational Speed?
        EV3_GYRO_M4 = 60, //	# Calibration ???

        //# Mode information is here:  https://github.com/mindboards/ev3dev/wiki/LEGO-EV3-Infrared-Sensor-%2845509%29
        EV3_INFRARED_M0 = 61, //	# Proximity, 0 to 100
        EV3_INFRARED_M1 = 62, //	# IR Seek, -25 (far left) to 25 (far right)
        EV3_INFRARED_M2 = 63, //	# IR Remote Control, 0 - 11
        EV3_INFRARED_M3 = 64,
        EV3_INFRARED_M4 = 65,
        EV3_INFRARED_M5 = 66,

        EV3_TOUCH_0 = 67,

        EV3_TOUCH_DEBOUNCE = 68, //	# EV3 Touch sensor, debounced.
        TOUCH_DEBOUNCE = 69 //	# NXT Touch sensor, debounced.


    }
    // Slowly migrating all those constants to more usable enums
    //Need to continue to clean them and remove unnecessary
    public partial class Brick
    {
        int US_I2C_SPEED = 10; //#tweak this value
        int US_I2C_IDX = 0;
        int LEGO_US_I2C_ADDR = 0x02;
        int LEGO_US_I2C_DATA_REG = 0x42;
        //#######################

        const int PORT_1 = 0;
        const int PORT_2 = 1;
        const int PORT_3 = 2;
        const int PORT_4 = 3;

        const int MASK_D0_M = 0x01;
        const int MASK_D1_M = 0x02;
        const int MASK_9V = 0x04;
        const int MASK_D0_S = 0x08;
        const int MASK_D1_S = 0x10;

        const int BYTE_MSG_TYPE = 0; // # MSG_TYPE is the first byte.
        const int MSG_TYPE_CHANGE_ADDR = 1; // # Change the UART address.
        const int MSG_TYPE_SENSOR_TYPE = 2; // # Change/set the sensor type.
        const int MSG_TYPE_VALUES = 3; // # Set the motor speed and direction, and return the sesnors and encoders.
        const int MSG_TYPE_E_STOP = 4; // # Float motors immidately
        const int MSG_TYPE_TIMEOUT_SETTINGS = 5; // # Set the timeout
        //# New UART address (MSG_TYPE_CHANGE_ADDR)
        const int BYTE_NEW_ADDRESS = 1;

        //# Sensor setup (MSG_TYPE_SENSOR_TYPE)
        const int BYTE_SENSOR_1_TYPE = 1;
        const int BYTE_SENSOR_2_TYPE = 2;

        const int BYTE_TIMEOUT = 1;

        const int TYPE_MOTOR_PWM = 0;
        const int TYPE_MOTOR_SPEED = 1;
        const int TYPE_MOTOR_POSITION = 2;

        //const int TYPE_SENSOR_RAW = (int)BrickSensorType.SENSOR_RAW;
        const int TYPE_SENSOR_LIGHT_OFF = 0;
        const int TYPE_SENSOR_LIGHT_ON = (MASK_D0_M | MASK_D0_S);

        const int RETURN_VERSION = 70; //	# Returns firmware version.

        const int BIT_I2C_MID = 0x01; //  # Do one of those funny clock pulses between writing and reading. defined for each device.
        const int BIT_I2C_SAME = 0x02; //  # The transmit data, and the number of bytes to read and write isn't going to change. defined for each device.

        const int INDEX_RED = 0;
        const int INDEX_GREEN = 1;
        const int INDEX_BLUE = 2;
        const int INDEX_BLANK = 3;

    }
}
