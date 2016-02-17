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
using System.ComponentModel;

namespace BrickPi.Movement
{

    /// <summary>
    /// Polarity of the motor
    /// </summary>
    public enum Polarity
    {
#pragma warning disable
        Backward = -1, Forward = 1, OppositeDirection = 0
#pragma warning restore
    };

    public sealed class Motor 
    {
        // represent the Brick
        private Brick brick = null;

        public Motor(BrickPortMotor port)
        {
            brick = new Brick();
            Port = port;
            brick.Start();
        }

        /// <summary>
        /// Set the speed of the motor
        /// </summary>
        /// <param name="speed">speed is between -255 and +255</param>
        public void SetSpeed(int speed)
        {
            if (speed > 255)
                speed = 255;
            if (speed < -255)
                speed = -255;
            brick.BrickPi.Motor[(int)Port].Speed = speed;

            //raise the event to notify the UI
            OnPropertyChanged(nameof(Speed));
        }

        /// <summary>
        /// Set Encoder offste
        /// TBD: need to be tested and see impact, not sure about implementation
        /// </summary>
        /// <param name="position">new offset</param>
        public void SetPosition(Int32 position)
        {
            brick.BrickPi.Motor[(int)Port].EncoderOffset = position;
        }

        /// <summary>
        /// Stop the Motor
        /// </summary>
        public void Stop()
        {
            brick.BrickPi.Motor[(int)Port].Enable = 0;
        }

        /// <summary>
        /// Start the motor
        /// </summary>
        public void Start()
        {
            brick.BrickPi.Motor[(int)Port].Enable = 1;
        }

        /// <summary>
        /// Start with the specified speed
        /// </summary>
        /// <param name="speed">speed is between -255 and +255</param>
        public void Start(int speed)
        {
            SetSpeed(speed);
            Start();
        }

        /// <summary>
        /// Change the polatity of the motor
        /// </summary>
        /// <param name="polarity">Polarity of the motor, backward, forward or opposite</param>
        public void SetPolarity(Polarity polarity)
        {
            switch (polarity)
            {
                case Polarity.Backward:
                    if (brick.BrickPi.Motor[(int)Port].Speed > 0)
                        brick.BrickPi.Motor[(int)Port].Speed = -brick.BrickPi.Motor[(int)Port].Speed;
                    break;
                case Polarity.Forward:
                    if (brick.BrickPi.Motor[(int)Port].Speed < 0)
                        brick.BrickPi.Motor[(int)Port].Speed = -brick.BrickPi.Motor[(int)Port].Speed;
                    break;
                case Polarity.OppositeDirection:
                    brick.BrickPi.Motor[(int)Port].Speed = -brick.BrickPi.Motor[(int)Port].Speed;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the tacho count
        /// </summary>
        /// <returns>The tacho count in 0.5 of degrees</returns>
        public Int32 GetTachoCount()
        {
            return brick.BrickPi.Motor[(int)Port].Encoder;
        }

        /// <summary>
        /// Get the speed
        /// </summary>
        /// <returns>speed is between -255 and +255</returns>
        public int GetSpeed()
        {
            return brick.BrickPi.Motor[(int)Port].Speed;
        }

        /// <summary>
        /// Set or read the speed of the motor
        /// speed is between -255 and +255
        /// </summary>
        public int Speed
        { get { return GetSpeed();  } set { SetSpeed(value);  } }

        public BrickPortMotor Port { get; internal set; }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

    }
}
