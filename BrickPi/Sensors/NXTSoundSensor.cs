﻿//////////////////////////////////////////////////////////
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
    class NXTSoundSensor : SensorNotificationBase, ISensor
    {
        private Brick brick = null;
        private const int NXTCutoff = 512;

        /// <summary>
        /// Initialise a new NXT Touch sensor
        /// </summary>
        /// <param name="port">Port where the NXT sensor is plugged</param>
        public NXTSoundSensor(BrickPortSensor port)
        {
            brick = new Brick();
            Port = port;
            brick.BrickPi.Sensor[(int)Port].Type = (byte)BrickSensorType.SENSOR_RAW;
            brick.SetupSensors();
        }

        /// <summary>
        /// Reads the sensor value as a string.
        /// </summary>
        /// <returns>The value as a string</returns>
        public string ReadAsString()
        {
            string s = "";
            s = Read().ToString();
            return s;
        }

        private int Read()
        {
            return (100 - brick.BrickPi.Sensor[(int)Port].Value);
        }

        /// <summary>
        /// Reads the raw sensor value
        /// </summary>
        /// <returns>The raw.</returns>
        public int ReadRaw()
        {
            return (1023 - brick.BrickPi.Sensor[(int)Port].Value);
        }

        /// <summary>
        /// Return port
        /// </summary>
        public BrickPortSensor Port
        { get; internal set; }

        public string GetSensorName()
        {
            return "NXT Sound";
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