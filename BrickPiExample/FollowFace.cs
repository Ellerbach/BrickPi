using BrickPi.Tools;
using BrickPi;
using BrickPi.Sensors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;
using BrickPi.Movement;

namespace BrickPiExample
{
    public sealed partial class MainPage
    {

        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("_Your subscription key_"); //_Your subscription key_
        private Vehicule robot; 

        private async Task<StorageFile> TakePhoto()
        {
            StorageFile filestr = await USBCam.TakePhotoAsync("picture.jpg");
            Debug.WriteLine(string.Format("File name: {0}", filestr.Name));
            using (IRandomAccessStream myfileStream = await filestr.OpenAsync(FileAccessMode.Read))
            {
                WriteableBitmap bitmap = new WriteableBitmap(1, 1);

                await bitmap.SetSourceAsync(myfileStream);

                FacePhoto.Source = bitmap;
            }
            return filestr;
        }

        private async Task<FaceRectangle[]> UploadAndDetectFaces(StorageFile imageFilePath)
        {
            try
            {
                using (IRandomAccessStream imageFileStream = await imageFilePath.OpenAsync(FileAccessMode.Read))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream.AsStream());
                    var faceRects = faces.Select(face => face.FaceRectangle);
                    return faceRects.ToArray();
                }
            }
            catch (Exception)
            {
                return new FaceRectangle[0];
            }
        }

        private async Task MakePicture()
        {
            Title.Text = "Detecting...";
            StorageFile filePath = await TakePhoto();
            FaceRectangle[] faceRects = await UploadAndDetectFaces(filePath); 
            Title.Text = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);

            if (faceRects.Length > 0)
            {
                var maxRes = await USBCam.GetPictureRes();
                //move left if face too right and vice versa
                // need to have the rectangle centered, in a good proportion
                // proportion can be X% left and right of what is left
                // only use the first face detected
                int fWidth = faceRects[0].Width;
                int fLeft = faceRects[0].Left;
                int iWidth = (int)maxRes.Width;
                double percent = 0.20;

                if ((fLeft<(iWidth * percent)) && ((fLeft + fWidth) < iWidth * (1-percent)))
                { 
                    // 360° = 1 turn of motor = 90° real turn
                    // using a simple projection for the math
                    robot.TurnLeft(150, GetAngleToTurn(fLeft, iWidth, percent) * 4);
                } else if (((fLeft + fWidth)>(iWidth * (1-percent))) && (fLeft>(iWidth*percent)))
                {
                    robot.TurnRight(150, GetAngleToTurn(iWidth - (fLeft + fWidth), iWidth, percent) * 4);
                }
            }
        }

        private int GetAngleToTurn(int fLeft, int iWidth, double percent)
        {
            double a = iWidth * (0.5 - percent);
            double h = iWidth / 2 - fLeft;
            double alpha = Math.Acos(a / h);
            int angleDegree = (int)(alpha * 360 / (2 * Math.PI));
            return angleDegree;
        }

        private async Task LunchFollowMe()
        {
            EV3TouchSensor touch = new EV3TouchSensor(BrickPortSensor.PORT_S1);
            NXTUltraSonicSensor ultra = new NXTUltraSonicSensor(BrickPortSensor.PORT_S3, UltraSonicMode.Centimeter);
            robot = new Vehicule(BrickPortMotor.PORT_B, BrickPortMotor.PORT_C);
            while (!touch.IsPressed())
            {
                int valultra = ultra.Value;
                if ((valultra < 70) && (valultra!=0))
                {
                    Debug.WriteLine($"Taking picture, distance {valultra} cm");
                    await MakePicture();
                }
                    
                await Task.Delay(10);
            }
            Debug.Write("end of face tracking and detection");
        }

    }
}
