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

using BrickPi;
using BrickPi.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BrickPiTests
{
    public sealed partial class MainPage : Page
    {
        private async Task TestMultipleSensorsDirctBrickStruct()
        {
            brick.BrickPi.Sensor[0].Type = BrickSensorType.EV3_TOUCH_0;
            brick.BrickPi.Sensor[1].Type = BrickSensorType.ULTRASONIC_CONT;
            brick.BrickPi.Sensor[2].Type = BrickSensorType.COLOR_FULL;
            bool bwait = true;
            while (bwait)
            {
                //brick.UpdateValues();
                for (int i = 0; i < 3; i++)
                {
                    Debug.WriteLine(string.Format("Sensor {0}, ID {1} data {2} - {3} - {4} -{5}", i, brick.BrickPi.Sensor[i].Value,
                        brick.BrickPi.Sensor[i].Array[0],
                        brick.BrickPi.Sensor[i].Array[1], brick.BrickPi.Sensor[i].Array[2], brick.BrickPi.Sensor[i].Array[3]));
                    Task.Delay(1000).Wait();

                }
                if (brick.BrickPi.Sensor[0].Array[0] != 0)
                    bwait = false;
            }
        }

        private async Task TestMultipleSensorsTouchCSSoud()
        {
            NXTTouchSensor touch = new NXTTouchSensor(BrickPortSensor.PORT_S2);
            EV3TouchSensor ev3Touch = new EV3TouchSensor(BrickPortSensor.PORT_S1, 20);
            NXTSoundSensor sound = new NXTSoundSensor(BrickPortSensor.PORT_S4);
            NXTColorSensor nxtlight = new NXTColorSensor(BrickPortSensor.PORT_S3);
            RGBColor rgb;
            bool bwait = true;
            while (bwait)
            {
                Debug.WriteLine(string.Format("NXT Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", touch.ReadRaw(), touch.ReadAsString(), touch.IsPressed(), touch.NumberOfModes(), touch.GetSensorName()));
                Debug.WriteLine(string.Format("EV3 Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", ev3Touch.ReadRaw(), ev3Touch.ReadAsString(), ev3Touch.IsPressed(), ev3Touch.NumberOfModes(), ev3Touch.GetSensorName()));
                Debug.WriteLine(string.Format("NXT Sound, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}", sound.ReadRaw(), sound.ReadAsString(), sound.NumberOfModes(), sound.GetSensorName()));
                Debug.WriteLine(string.Format("NXT Color Sensor, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}",
                    nxtlight.ReadRaw(), nxtlight.ReadAsString(), nxtlight.NumberOfModes(), nxtlight.GetSensorName()));
                rgb = nxtlight.ReadRGBColor();
                Debug.WriteLine(string.Format("Color: {0}, Red: {1}, Green: {2}, Blue: {3}",
                    nxtlight.ReadColor(), rgb.Red, rgb.Green, rgb.Blue));
                //                Debug.WriteLine(string.Format("raw {0}", nxtlight.ReadTest()));
                await Task.Delay(300);
                if ((touch.IsPressed()) && ev3Touch.IsPressed())
                    bwait = false;
            }
        }

        private async Task TestEV3Color()
        {
            //brick.Stop();
            //brick.SetTimeout(250);
            EV3ColorSensor nxtlight = new EV3ColorSensor(BrickPortSensor.PORT_S4, ColorSensorMode.Reflection);
            EV3TouchSensor touch = new EV3TouchSensor(BrickPortSensor.PORT_S1);
            //brick.Stop();
            //brick.SetupSensors();
            RGBColor rgb;
            await Task.Delay(5000);
            for (int i = 0; i < nxtlight.NumberOfModes(); i++)
            {
                int count = 0;
                while ((count < 100) && !touch.IsPressed())
                {
                    //Debug.WriteLine(string.Format("NXT Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", touch.ReadRaw(), touch.ReadAsString(), touch.IsPressed(), touch.NumberOfModes(), touch.GetSensorName()));
                    //Debug.WriteLine(string.Format("EV3 Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", ev3Touch.ReadRaw(), ev3Touch.ReadAsString(), ev3Touch.IsPressed(), ev3Touch.NumberOfModes(), ev3Touch.GetSensorName()));
                    //Debug.WriteLine(string.Format("NXT Sound, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}", sound.ReadRaw(), sound.ReadAsString(), sound.NumberOfModes(), sound.GetSensorName()));

                    //brick.UpdateValues();
                    Debug.WriteLine(string.Format("EV3 Color Sensor, Raw: {0}, ReadASString: {1}",
                        nxtlight.ReadRaw(), nxtlight.ReadAsString()));
                    rgb = nxtlight.ReadRGBColor();
                    Debug.WriteLine(string.Format("Color: {0}, Red: {1}, Green: {2}, Blue: {3}",
                        nxtlight.ReadColor(), rgb.Red, rgb.Green, rgb.Blue));
                    //brick.Stop();
                    //brick.Start();
                    //nxtlight.ColorMode = ColorSensorMode.Ambient;
                    await Task.Delay(1000);
                    //if ((touch.IsPressed()) && ev3Touch.IsPressed())
                    count++;
                    //nxtlight.ColorMode = ColorSensorMode.Color;
                }
                if (nxtlight.ColorMode == ColorSensorMode.Reflection)
                    nxtlight.ColorMode = ColorSensorMode.Color;
                else
                    nxtlight.ColorMode = ColorSensorMode.Reflection;
                //brick.SetupSensors();
                await Task.Delay(5000);
            }

        }
        //EV3IRSensor
        private async Task TestIRSensor()
        {

            EV3IRSensor ultra = new EV3IRSensor(BrickPortSensor.PORT_S4, IRMode.Remote);
            for (int i = 0; i < ultra.NumberOfModes(); i++)
            {
                int count = 0;
                while (count < 100)
                {
                    Debug.WriteLine(string.Format("NXT ultra, Distance: {0}, ReadAsString: {1}, NumberNodes: {2}, SensorName: {3}",
                        ultra.ReadBeaconLocation(), ultra.ReadAsString(), ultra.Mode, ultra.GetSensorName()));
                    await Task.Delay(300);
                }
                ultra.SelectNextMode();
            }
        }

        //TODO build test for EV3 Ultra Sound

        private async Task TestNXTUS()
        {
            NXTUltraSonicSensor ultra = new NXTUltraSonicSensor(BrickPortSensor.PORT_S4);
            for (int i = 0; i < ultra.NumberOfModes(); i++)
            {
                int count = 0;
                while (count < 100)
                {
                    Debug.WriteLine(string.Format("NXT US, Distance: {0}, ReadAsString: {1}, Selected mode: {2}",
                        ultra.ReadDistance(), ultra.ReadAsString(), ultra.SelectedMode()));
                    await Task.Delay(300);
                }
                ultra.SelectNextMode();
            }
        }

        private async Task TestNXTLight()
        {
            //NXTTouchSensor touch = new NXTTouchSensor(BrickPortSensor.PORT_S2);
            //EV3TouchSensor ev3Touch = new EV3TouchSensor(BrickPortSensor.PORT_S1);
            //NXTSoundSensor sound = new NXTSoundSensor(BrickPortSensor.PORT_S4);
            NXTLightSensor nxtlight = new NXTLightSensor(BrickPortSensor.PORT_S4);
            int count = 0;
            while (count < 100)
            {
                //Debug.WriteLine(string.Format("NXT Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", touch.ReadRaw(), touch.ReadAsString(), touch.IsPressed(), touch.NumberOfModes(), touch.GetSensorName()));
                //Debug.WriteLine(string.Format("EV3 Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", ev3Touch.ReadRaw(), ev3Touch.ReadAsString(), ev3Touch.IsPressed(), ev3Touch.NumberOfModes(), ev3Touch.GetSensorName()));
                //Debug.WriteLine(string.Format("NXT Sound, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}", sound.ReadRaw(), sound.ReadAsString(), sound.NumberOfModes(), sound.GetSensorName()));
                Debug.WriteLine(string.Format("NXT Color Sensor, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}",
                    nxtlight.ReadRaw(), nxtlight.ReadAsString(), nxtlight.NumberOfModes(), nxtlight.GetSensorName()));
                Debug.WriteLine(string.Format("Color: {0}, ",
                    nxtlight.ReadRaw()));

                await Task.Delay(300);
                //if ((touch.IsPressed()) && ev3Touch.IsPressed())
                count++;
            }
            count = 0;
            nxtlight.SelectNextMode();
            while (count < 100)
            {
                //Debug.WriteLine(string.Format("NXT Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", touch.ReadRaw(), touch.ReadAsString(), touch.IsPressed(), touch.NumberOfModes(), touch.GetSensorName()));
                //Debug.WriteLine(string.Format("EV3 Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", ev3Touch.ReadRaw(), ev3Touch.ReadAsString(), ev3Touch.IsPressed(), ev3Touch.NumberOfModes(), ev3Touch.GetSensorName()));
                //Debug.WriteLine(string.Format("NXT Sound, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}", sound.ReadRaw(), sound.ReadAsString(), sound.NumberOfModes(), sound.GetSensorName()));
                Debug.WriteLine(string.Format("NXT Color Sensor, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}",
                    nxtlight.ReadRaw(), nxtlight.ReadAsString(), nxtlight.NumberOfModes(), nxtlight.GetSensorName()));
                Debug.WriteLine(string.Format("Color: {0}, ",
                    nxtlight.ReadRaw()));

                await Task.Delay(300);
                //if ((touch.IsPressed()) && ev3Touch.IsPressed())
                count++;
            }
        }

        private async Task GetVersion()
        {
            int val = brick.GetBrickVersion();
            Debug.WriteLine(string.Format("Version number: {0}", val));

        }
        //TODO: build other sensor tests
    }
}
