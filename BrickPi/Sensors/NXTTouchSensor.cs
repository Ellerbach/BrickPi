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
    public sealed class NXTTouchSensor: ISensor
    {
        private Brick brick = null;
        

        /// <summary>
        /// Initialise a new NXT Touch sensor
        /// </summary>
        /// <param name="port">Port where the NXT sensor is plugged</param>
        public NXTTouchSensor(BrickPortSensor port)
        {
            brick = new Brick();
            Port = port;
            brick.BrickPi.Sensor[(int)Port].Type = BrickSensorType.TOUCH;
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
            if (ReadRaw() == 1)
                return true;
            return false;
        }

        /// <summary>
        /// Reads the raw sensor value
        /// </summary>
        /// <returns>The raw.</returns>
        public int ReadRaw(){
            return brick.BrickPi.Sensor[(int)Port].Value;
		}

        /// <summary>
        /// Return port
        /// </summary>
        public BrickPortSensor Port
        { get; internal set; }

        public string GetSensorName()
        {
            return "NXT Touch";
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
