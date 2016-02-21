using BrickPi.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace BrickPiTests
{
    public sealed partial class MainPage : Page
    {
        private async Task TestCam()
        {
            StorageFile filestr = await USBCam.TakePhotoAsync("maxime.jpg");
            Debug.WriteLine(string.Format("File name: {0}", filestr.Name));
        }
    }
}
