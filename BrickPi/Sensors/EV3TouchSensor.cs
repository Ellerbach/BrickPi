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

namespace BrickPi.Sensors
{
    class EV3TouchSensor : SensorNotificationBase, ISensor
    {
        private Brick brick = null;
        // in the BrickPi source code, this value is 1020
        private int NXTCutoff = 1015;

        /// <summary>
        /// Initialise a new EV3 Touch sensor
        /// </summary>
        /// <param name="port">Port where the NXT sensor is plugged</param>
        public EV3TouchSensor(BrickPortSensor port)
        {
            brick = new Brick();
            Port = port;
            brick.BrickPi.Sensor[(int)Port].Type = BrickSensorType.EV3_TOUCH_0;
            brick.SetupSensors();
        }

        /// <summary>
        /// Reads the sensor value as a string.
        /// </summary>
        /// <returns>The value as a string</returns>
        public string ReadAsString()
        {
            string s = "";
            if (IsPressed())
            {
                s = "Pressed";
            }
            else {
                s = "Not pressed";
            }
            return s;
        }

        /// <summary>
        /// Determines whether the touch sensor is pressed.
        /// </summary>
        /// <returns><c>true</c> if the sensor is pressed; otherwise, <c>false</c>.</returns>
        public bool IsPressed()
        {
            if (ReadRaw() > NXTCutoff)
                return true;
            return false;
        }

        /// <summary>
        /// Reads the raw sensor value
        /// </summary>
        /// <returns>The raw.</returns>
        public int ReadRaw()
        {
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        /// <summary>
        /// Return port
        /// </summary>
        public BrickPortSensor Port
        { get; internal set; }

        public string GetSensorName()
        {
            return "EV3 Touch";
        }

        public int NumberOfModes()
        {
            return 1;
        }

        public string SelectedMode()
        {
            return "Analog";
        }

        public void SelectNextMode()
        {
            return;
        }

        public void SelectPreviousMode()
        {
            return;
        }
    }
}
