using BrickPi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BrickPiTests
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Need a brick and a serial port
        private Brick brick = null;
        private SerialDevice serialPort = null;
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        /// <summary>
        /// This functions initimlaize a serial port and pass it to the brick
        /// </summary>
        /// <returns></returns>
        private async Task InitSerial()
        {
            string aqs = SerialDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);

            for (int i = 0; i < dis.Count; i++)
            {
                Debug.WriteLine(string.Format("Serial device found: {0}", dis[i].Id));
                if (dis[i].Id.IndexOf("UART0") != -1)
                {
                    serialPort = await SerialDevice.FromIdAsync(dis[i].Id);
                }
            }
            if (serialPort != null)
            {
                Debug.WriteLine("Serial port initialiazed");
                brick = new Brick(serialPort);
                Debug.WriteLine("Brick initialiazed");
            }

        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitSerial();
            //call the tests from here
            //await TestVehicule();
            await TestEV3Color();
            brick.Stop();
        }
    }
}
