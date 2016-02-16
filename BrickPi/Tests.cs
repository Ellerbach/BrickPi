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

//TODO: clean the test functions form here, move to another project

using BrickPi.Movement;
using BrickPi.Sensors;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BrickPi
{
    partial class StartupTask
    {
        private async Task TestMotor1Motor()
        {
            Motor motor = new Motor(BrickPortMotor.PORT_D);
            motor.SetSpeed(10);
            motor.Start();
            Stopwatch stopwatch = Stopwatch.StartNew();
            long initialTick = stopwatch.ElapsedTicks;
            double desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            double finalTick = initialTick + desiredTicks;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                Debug.WriteLine(string.Format("Encoder: {0}", motor.GetTachoCount()));
                await Task.Delay(200);
                motor.SetSpeed(motor.GetSpeed() + 10);

            }
            motor.SetPolarity(Polarity.OppositeDirection);
            desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            finalTick = stopwatch.ElapsedTicks + desiredTicks;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                Debug.WriteLine(string.Format("Encoder: {0}", motor.GetTachoCount()));
                await Task.Delay(200);
                motor.SetSpeed(motor.GetSpeed() + 10);
            }
            desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            finalTick = stopwatch.ElapsedTicks + desiredTicks;
            int pos = 0;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                Debug.WriteLine(string.Format("Encoder: {0}", motor.GetTachoCount()));
                await Task.Delay(200);
                motor.SetPosition(pos);
                pos += 100;
            }
            motor.Stop();
            //int port = (int)BrickPortMotor.PORT_D;
            //brick.BrickPi.MotorEnable[port] = 1;
            //brick.SetupSensors();
            //brick.BrickPi.MotorSpeed[port] = 200;
            //Stopwatch stopwatch = Stopwatch.StartNew();
            //long initialTick = stopwatch.ElapsedTicks;
            //long initialElapsed = stopwatch.ElapsedMilliseconds;
            //double desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            //double finalTick = initialTick + desiredTicks;
            //while (stopwatch.ElapsedTicks < finalTick)
            //{
            //    Debug.WriteLine(string.Format("Encoder: {0}", brick.BrickPi.Encoder[port]));
            //    await Task.Delay(200);
            //}
            //brick.BrickPi.MotorSpeed[3] = -100;
            //desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            //finalTick = stopwatch.ElapsedTicks + desiredTicks;
            //while (stopwatch.ElapsedTicks < finalTick)
            //{
            //    Debug.WriteLine(string.Format("Encoder: {0}", brick.BrickPi.Encoder[port]));
            //    await Task.Delay(200);
            //}
            //brick.BrickPi.MotorSpeed[port] = 0;
            //brick.BrickPi.MotorEnable[port] = 0;

            // brick.UpdateValues();
        }

        private async Task TestMotorBrick()
        {
            int port = (int)BrickPortMotor.PORT_D;
            brick.BrickPi.Motor[port].Enable = 1;
            brick.SetupSensors();
            brick.BrickPi.Motor[port].Speed = 200;
            Stopwatch stopwatch = Stopwatch.StartNew();
            long initialTick = stopwatch.ElapsedTicks;
            long initialElapsed = stopwatch.ElapsedMilliseconds;
            double desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            double finalTick = initialTick + desiredTicks;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                Debug.WriteLine(string.Format("Encoder: {0}", brick.BrickPi.Motor[port].Encoder));
                await Task.Delay(200);
            }
            brick.BrickPi.Motor[port].Speed = -100;
            desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            finalTick = stopwatch.ElapsedTicks + desiredTicks;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                Debug.WriteLine(string.Format("Encoder: {0}", brick.BrickPi.Motor[port].Encoder));
                await Task.Delay(200);
            }
            brick.BrickPi.Motor[port].Speed = 0;
            brick.BrickPi.Motor[port].Enable = 0;

            brick.UpdateValues();

        }

        private async Task TestMotor()
        {
            Motor[] motor = new Motor[3];
            motor[0] = new Motor(BrickPortMotor.PORT_D);
            motor[1] = new Motor(BrickPortMotor.PORT_A);
            motor[2] = new Motor(BrickPortMotor.PORT_C);
            for (int i = 0; i < motor.Length; i++)
            {
                motor[i].SetSpeed(0);
                motor[i].Start();
            }
            Stopwatch stopwatch = Stopwatch.StartNew();
            long initialTick = stopwatch.ElapsedTicks;
            double desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            double finalTick = initialTick + desiredTicks;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                for (int i = 0; i < motor.Length; i++)
                {
                    Debug.WriteLine(string.Format("Encoder motor {0}: {1}", i, motor[i].GetTachoCount()));
                    motor[i].SetSpeed(motor[i].GetSpeed() + 5);
                }
                await Task.Delay(200);
            }
            Debug.WriteLine("End speed increase");
            for (int i = 0; i < motor.Length; i++)
            {
                motor[i].SetPolarity(Polarity.OppositeDirection);
            }
            Debug.WriteLine("End of inverting rotation");
            desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            finalTick = stopwatch.ElapsedTicks + desiredTicks;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                for (int i = 0; i < motor.Length; i++)
                {
                    Debug.WriteLine(string.Format("Encoder motor {0}: {1}", i, motor[i].GetTachoCount()));
                    motor[i].SetSpeed(motor[i].GetSpeed() + 5);
                }
                await Task.Delay(200);
                    
            }
            Debug.WriteLine("End speed decrease");
            desiredTicks = 10000.0 / 1000.0 * Stopwatch.Frequency;
            finalTick = stopwatch.ElapsedTicks + desiredTicks;
            int pos = 0;
            while (stopwatch.ElapsedTicks < finalTick)
            {
                for (int i = 0; i < motor.Length; i++)
                {
                    Debug.WriteLine(string.Format("Encoder motor {0}: {1}", i, motor[i].GetTachoCount()));
                    motor[i].SetPosition(pos);
                }
                await Task.Delay(200);
                    
                pos ++;
            }
            Debug.WriteLine("End encoder offset test");
            for (int i = 0; i < motor.Length; i++)
            {
                motor[i].Stop();
            }
            Debug.WriteLine("All motors stoped");
        }

        private async Task TestbuttonBrcikPi()
        {
            brick.BrickPi.Sensor[0].Type = BrickSensorType.EV3_TOUCH_0;
            brick.BrickPi.Sensor[1].Type = BrickSensorType.ULTRASONIC_CONT;
            brick.BrickPi.Sensor[2].Type = BrickSensorType.COLOR_FULL;
            brick.SetupSensors();
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

        private async Task TestbuttonTouchCSSoud()
        {
            NXTTouchSensor touch = new NXTTouchSensor(BrickPortSensor.PORT_S2);
            EV3TouchSensor ev3Touch = new EV3TouchSensor(BrickPortSensor.PORT_S1);
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

        private async Task TestbuttonEV3Color()
        {
            EV3ColorSensor nxtlight = new EV3ColorSensor(BrickPortSensor.PORT_S4, ColorSensorMode.Reflection);
            RGBColor rgb;
            for (int i=0; i<nxtlight.NumberOfModes(); i++)
            { 
                int count = 0;
                while (count < 100)
                {
                    //Debug.WriteLine(string.Format("NXT Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", touch.ReadRaw(), touch.ReadAsString(), touch.IsPressed(), touch.NumberOfModes(), touch.GetSensorName()));
                    //Debug.WriteLine(string.Format("EV3 Touch, Raw: {0}, ReadASString: {1}, IsPressed: {2}, NumberNodes: {3}, SensorName: {4}", ev3Touch.ReadRaw(), ev3Touch.ReadAsString(), ev3Touch.IsPressed(), ev3Touch.NumberOfModes(), ev3Touch.GetSensorName()));
                    //Debug.WriteLine(string.Format("NXT Sound, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}", sound.ReadRaw(), sound.ReadAsString(), sound.NumberOfModes(), sound.GetSensorName()));
                    Debug.WriteLine(string.Format("EV3 Color Sensor, Raw: {0}, ReadASString: {1}, NumberNodes: {2}, SensorName: {3}",
                        nxtlight.ReadRaw(), nxtlight.ReadAsString(), nxtlight.NumberOfModes(), nxtlight.GetSensorName()));
                    rgb = nxtlight.ReadRGBColor();
                    Debug.WriteLine(string.Format("Color: {0}, Red: {1}, Green: {2}, Blue: {3}",
                        nxtlight.ReadColor(), rgb.Red, rgb.Green, rgb.Blue));

                    await Task.Delay(1000);
                    //if ((touch.IsPressed()) && ev3Touch.IsPressed())
                    count++;
                }
                nxtlight.SelectNextMode();
            }

        }
        //EV3IRSensor
        private async Task TestbuttonIRSensor()
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

        private async Task TestbuttonNEXTUS()
        {
            TestbuttonNXTLight();
            NXTUltraSonicSensor ultra = new NXTUltraSonicSensor(BrickPortSensor.PORT_S3);
            for(int i =0; i<ultra.NumberOfModes(); i++)
            { 
                int count = 0;
                while (count<100)
                {
                    Debug.WriteLine(string.Format("NXT Touch, Distance: {0}, ReadAsString: {1}, NumberNodes: {2}, SensorName: {3}",
                        ultra.ReadDistance(), ultra.ReadAsString(), ultra.NumberOfModes(), ultra.GetSensorName()));
                    await Task.Delay(300);
                }
                ultra.SelectNextMode();
            }
        }

        private async Task TestbuttonNXTLight()
        {
            //NXTTouchSensor touch = new NXTTouchSensor(BrickPortSensor.PORT_S2);
            //EV3TouchSensor ev3Touch = new EV3TouchSensor(BrickPortSensor.PORT_S1);
            //NXTSoundSensor sound = new NXTSoundSensor(BrickPortSensor.PORT_S4);
            NXTLightSensor nxtlight = new NXTLightSensor(BrickPortSensor.PORT_S4);
            int count = 0;
            while (count<100)
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
    }
}
