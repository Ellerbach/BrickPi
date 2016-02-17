using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace BrickPi.Sensors
{
    public sealed class SensorNotificationBase
    {
        Timer timer;

        public SensorNotificationBase() : this(1000)
        {
        }

        public SensorNotificationBase(int miliseconds)
        {
            InitializeTimer(miliseconds);
        }

        public void StopSendingNotifications()
        {
            StopTimerInternal();
        }

        public void InitializeTimer(int miliseconds)
        {
            StopTimerInternal();
            timer = new Timer(Timer_Tick, this, TimeSpan.FromMilliseconds(miliseconds), TimeSpan.FromMilliseconds(miliseconds));

        }

        private void Timer_Tick(object state)
        {
            OnPropertyChanged(nameof(ValueAsString));
            OnPropertyChanged(nameof(Value));
        }

        private void StopTimerInternal()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }


        private int value;
        private string valueAsString;

        public int Value
        {
            get
            {
                return value;
            }

            set
            {
                if (value != this.value)
                {
                    this.value = value;
                }
            }
        }

        public string ValueAsString
        {
            get
            {
                return valueAsString;
            }
            set
            {
                if (valueAsString != value)
                {
                    valueAsString = value;
                }
            }
        }

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
