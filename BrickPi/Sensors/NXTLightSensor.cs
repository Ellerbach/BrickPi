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
    /// Sensor mode when using a NXT light sensor
    /// </summary>
    public enum LightMode
    {
        /// <summary>
        /// Use the lgith sensor to read reflected light
        /// </summary>
        Relection = BrickSensorType.LIGHT_ON,

        /// <summary>
        /// Use the light sensor to detect the light intensity
        /// </summary>
        Ambient = BrickSensorType.LIGHT_OFF,
    };

    class NXTLightSensor:ISensor
    {
        private LightMode lightMode;
        private Brick brick = null;

        public NXTLightSensor(BrickPortSensor port):this(port, LightMode.Relection)
        { }

        public NXTLightSensor(BrickPortSensor port, LightMode mode)
        {
            brick = new Brick();
            Port = port;
            lightMode = mode;
            CutOff = 512;
            brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)mode;
            brick.SetupSensors();

        }

        public int CutOff { get; set; }

        public LightMode LightMode
        {
            get
            {
                return lightMode;
            }

            set
            {
                if (value != lightMode)
                {
                    lightMode = value;
                    brick.BrickPi.Sensor[(int)Port].Type = (BrickSensorType)lightMode;
                    brick.SetupSensors();
                }
            }
        }

        public BrickPortSensor Port
        {
            get; internal set;
        }

        public void SelectNextMode()
        {
            LightMode = LightMode.Next();
            return;
        }

        public void SelectPreviousMode()
        {
            LightMode = LightMode.Previous();
            return;
        }

        public int NumberOfModes()
        {
            return Enum.GetNames(typeof(LightMode)).Length;
        }

        public string SelectedMode()
        {
            return LightMode.ToString();
        }

        public int ReadRaw()
        {
            return brick.BrickPi.Sensor[(int)Port].Value;
        }

        public string ReadAsString()
        {
            if (ReadRaw() > CutOff)
                return "Dark";
            return "Clear";
        }

        public string GetSensorName()
        {
            return "NXT Light";
        }
    }
}
