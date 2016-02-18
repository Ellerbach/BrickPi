using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrickPi.Movement
{
    class Vehicule
    {
        private Brick brick = null;
        private BrickPortMotor portleft;
        private BrickPortMotor portright;

        public Vehicule(BrickPortMotor left, BrickPortMotor right)
        {
            brick = new Brick();
            brick.Start();
            portleft = left;
            portright = right;
        }

        public void Backward(int speed)
        {
            StartMotor((int)PortLeft, speed);
            StartMotor((int)PortRight, speed);
        }

        public void Foreward(int speed)
        {
            Backward(-speed);
        }

        public void TurnLeft(int speed, int degrees)
        {
            RunMotorSyncDegrees(new BrickPortMotor[2] { portleft, PortRight }, new int[2] { -speed, speed}, new int[2] { degrees, degrees } ).Wait();
        }

        public void TurnRight(int speed, int degrees)
        {
            RunMotorSyncDegrees(new BrickPortMotor[2] { portleft, PortRight }, new int[2] { speed, -speed }, new int[2] { degrees, degrees }).Wait();
        }

        public void TrunLeftTime(int speed, int timeout)
        {
            RunMotorSyncTime(new BrickPortMotor[2] { portleft, portright }, new int[2] { -speed, speed }, timeout).Wait();
        }

        public void TrunRightTime(int speed, int timeout)
        {
            RunMotorSyncTime(new BrickPortMotor[2] { portleft, portright }, new int[2] { speed, -speed }, timeout).Wait();
        }

        public void Stop()
        {
            StopMotor((int)PortLeft);
            StopMotor((int)PortRight);
        }

        public void Backward(int speed, int timeout)
        {
            RunMotorSyncTime(new BrickPortMotor[2] { portleft, portright }, new int[2] { speed, speed }, timeout).Wait();
        }

        public void Foreward(int speed, int timeout)
        {
            Backward(-speed, timeout);
        }

        public BrickPortMotor PortLeft
        { get { return portleft; } }

        public BrickPortMotor PortRight
        { get { return portright; } }

        private Timer timer;
        private async Task RunMotorSyncTime(BrickPortMotor[] ports, int[] speeds, int timeout)
        {
            if ((ports == null) || (speeds == null))
                return;
            if (ports.Length != speeds.Length)
                return;
            //create a timer for the needed time to run
            timer = new Timer(RunUntil, ports, TimeSpan.FromMilliseconds(timeout), Timeout.InfiniteTimeSpan);
            //initialize the speed and enable motors
            for(int i=0; i<ports.Length; i++)
            {
                StartMotor((int)ports[i], speeds[i]);
            }
        }

        private void RunUntil(object state)
        {
            //stop all motors!
            BrickPortMotor[] ports = (BrickPortMotor[])state;
            for (int i = 0; i < ports.Length; i++)
            {
                StopMotor((int)ports[i]);
            }
        }

        private void StopMotor(int port)
        {
            brick.BrickPi.Motor[port].Speed = 0;
            brick.BrickPi.Motor[port].Enable = 0;
        }

        private void StartMotor(int port, int speed)
        {
            if (speed > 255)
                speed = 255;
            if (speed < -255)
                speed = -255;
            brick.BrickPi.Motor[port].Speed = speed;
            brick.BrickPi.Motor[port].Enable = 1;
        }

        private async Task RunMotorSyncDegrees(BrickPortMotor[] ports, int[] speeds, int[] degrees)
        {
            if ((ports == null) || (speeds == null) || degrees == null)
                return;
            if ((ports.Length != speeds.Length) && (degrees.Length != speeds.Length))
                return;
            //make sure we have only positive degrees
            for (int i = 0; i < degrees.Length; i++)
                if (degrees[i] < 0)
                    degrees[i] = -degrees[i];
            //initialize the speed and enable motors
            int[] initval = new int[ports.Length];
            for (int i = 0; i < ports.Length; i++)
            {
                initval[i] = brick.BrickPi.Motor[(int)ports[i]].Encoder;
                StartMotor((int)ports[i], speeds[i]);
            }
            bool nonstop = true;
            while(nonstop)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    if (speeds[i] > 0)
                    {
                        if (brick.BrickPi.Motor[(int)ports[i]].Encoder >= (initval[i] + degrees[i] * 2))
                        {
                            StopMotor((int)ports[i]);
                        }
                    } else
                    {
                        if (brick.BrickPi.Motor[(int)ports[i]].Encoder <= (initval[i] - degrees[i] * 2))
                        {
                            StopMotor((int)ports[i]);
                        }

                    }
                    nonstop |= IsRunning((int)ports[i]);
                }
            }


        }

        private bool IsRunning(int port)
        {
            if (brick.BrickPi.Motor[port].Enable == 0)
                return false;
            return true;
        }
    }
}
