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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrickPi.Sensors
{
    public sealed class AnalogSensor: INotifyPropertyChanged, ISensor
    {
        private Brick brick = null;      

        /// <summary>
        /// Initialize an Analog Sensor
        /// </summary>
        /// <param name="port">Sensor port</param>
        public AnalogSensor(BrickPortSensor port):this(port, 1000)
        { }

        /// <summary>
        /// Initialize an Analog Sensor
        /// </summary>
        /// <param name="port">Sensor port</param>
        /// <param name="timeout">Period in millisecond to check sensor value changes</param>
        public AnalogSensor(BrickPortSensor port, int timeout)
        {
            brick = new Brick();
            Port = port;
            brick.BrickPi.Sensor[(int)Port].Type = BrickSensorType.SENSOR_RAW;
            brick.SetupSensors();
            periodRefresh = timeout;
            timer = new Timer(UpdateSensor, this, TimeSpan.FromMilliseconds(timeout), TimeSpan.FromMilliseconds(timeout));
        }

        private Timer timer = null;
        private void StopTimerInternal()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }
        }

        public BrickPortSensor Port
        {
            get; internal set;
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        /// <summary>
        /// To notify a property has changed. The minimum time can be set up
        /// with timeout property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private int periodRefresh;
        /// <summary>
        /// Period to refresh the notification of property changed in milliseconds
        /// </summary>
        public int PeriodRefresh
        {   get { return periodRefresh; }
            set {
                periodRefresh = value;
                timer.Change(TimeSpan.FromMilliseconds(periodRefresh), TimeSpan.FromMilliseconds(periodRefresh));
            }
        }

        /// <summary>
        /// Update the sensor and this will raised an event on the interface
        /// </summary>
        public void UpdateSensor(object state)
        {
            Value = ReadRaw();
            ValueAsString = ReadAsString();
        }
        private int value;
        private string valueAsString;

        /// <summary>
        /// Return the raw value of the sensor
        /// </summary>
        public int Value
        {
            get { return ReadRaw(); }
            set
            {
                if (value != this.value)
                {
                    this.value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Return the raw value  as a string of the sensor
        /// </summary>
        public string ValueAsString
        {
            get { return ReadAsString(); }
            set
            {
                if (valueAsString != value)
                {
                    valueAsString = value;
                    OnPropertyChanged(nameof(ValueAsString));
                }
            }
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
