using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BrickPi.Tools
{
    /// <summary>
    /// Class to control USB camera, to be used as a singleton
    /// </summary>
    public static class USBCam
    {

        private static MediaCapture mediaCapture = null;

        /// <summary>
        /// Async method to take a photo. Do not forget to declare capabilities on the manifest
        ///    <uap:Capability Name="videosLibrary" />  
        ///   <uap:Capability Name = "picturesLibrary" />
        ///     < DeviceCapability Name="microphone" />  
        ///  <DeviceCapability Name = "webcam" />
        /// </summary>
        /// <param name="filename">Filename to store the file</param>
        /// <returns>A storage file containing the captured photo. To be used from XAML like
        /// //captureImage is the XAML image instance name
        /// IRandomAccessStream photoStream = await photoFile.OpenReadAsync();
        /// BitmapImage bitmap = new BitmapImage();
        /// bitmap.SetSource(photoStream);
        /// captureImage.Source = bitmap;
        /// </returns>
        public static async Task<StorageFile> TakePhotoAsync(string filename)
        {
            StorageFile photoFile = await KnownFolders.PicturesLibrary.CreateFileAsync(
                    filename, CreationCollisionOption.GenerateUniqueName);
            ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
            // done in the next function: await InitCam();
            ////highest res possible
            var maxRes = await GetPictureRes();
            // Set to picture format
            await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxRes);
            //clic clac, smile :-)
            await mediaCapture.CapturePhotoToStorageFileAsync(imageProperties, photoFile);
            return photoFile;
            
        }
        public static async Task<VideoEncodingProperties> GetPictureRes()
        {
            await InitCam();
            var resolutions = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Select(x => x as VideoEncodingProperties);
            ////highest res possible
            var maxRes = resolutions.OrderByDescending(x => x.Height * x.Width).FirstOrDefault();
            return maxRes;
        }

        private static async Task InitCam()
        {
            if (mediaCapture == null)
            {
                mediaCapture = new MediaCapture();
                //need to be initialized
                await mediaCapture.InitializeAsync();
            }

        }
    }
}
