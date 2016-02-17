using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BrickPi.Sensors
{
    internal class SensorNotificationBase
    {
        DispatcherTimer timer;

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
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(miliseconds);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void StopTimerInternal()
        {
            if (timer != null)
            {
                timer.Tick -= Timer_Tick;
                timer.Stop();
                timer = null;
            }
        }

        private void Timer_Tick(object sender, object e)
        {
            OnPropertyChanged(nameof(ValueAsString));
            OnPropertyChanged(nameof(Value));
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
