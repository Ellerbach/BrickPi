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

using BrickPi;
using BrickPi.Movement;
using BrickPi.Sensors;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace BrickPiTests
{

    public sealed partial class MainPage : Page
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
                await Task.Delay(2000);
                motor.SetTachoCount(pos);
            }
            motor.Stop();

        }

        private async Task TestMotorDirectStructAccess()
        {
            int port = (int)BrickPortMotor.PORT_D;
            brick.BrickPi.Motor[port].Enable = 1;
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
                    motor[i].SetSpeed(motor[i].GetSpeed() + 1);
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
                    motor[i].SetTachoCount(pos);
                }
                await Task.Delay(1000);

            }
            Debug.WriteLine("End encoder offset test");
            for (int i = 0; i < motor.Length; i++)
            {
                motor[i].Stop();
            }
            Debug.WriteLine("All motors stoped");
        }

        private async Task TestVehicule()
        {
            Vehicule veh = new Vehicule(BrickPortMotor.PORT_B, BrickPortMotor.PORT_C);
            veh.DirectionOpposite = true;
            veh.Backward(30, 5000);
            veh.Foreward(30, 5000);
            veh.TrunLeftTime(30, 5000);
            veh.TrunRightTime(30, 5000);
            veh.TurnLeft(30, 180);
            veh.TurnRight(30, 180);

        }
    }
}