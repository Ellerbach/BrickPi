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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickPi.Sensors
{
    class AnalogSensor : SensorNotificationBase, ISensor
    {
        private Brick brick = null;

        public AnalogSensor(BrickPortSensor port)
        {
            brick = new Brick();
            Port = port;
            brick.BrickPi.Sensor[(int)Port].Type = BrickSensorType.SENSOR_RAW;
            brick.SetupSensors();
        }
        public BrickPortSensor Port
        {
            get; internal set;
        }

        /// <summary>
        /// Update the sensor and this will raised an event on the interface
        /// </summary>
        public void UpdateSensor()
        {
            this.Value = ReadRaw();
            this.ValueAsString = ReadAsString();
        }



        public int ReadRaw()
        {
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        public string GetSensorName()
        {
            return "Analog Raw";
        }

        public int NumberOfModes()
        {
            return 1;
        }

        public string ReadAsString()
        {
            string s = "";
            s = ReadRaw().ToString();
            return s;
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
