This example shows how to use Project Oxford to track faces and move a BrickPi based robot to track the face.
To get access to Project Oxford, follow the instructions here: https://www.projectoxford.ai/.
Follow the instruction to add as well the needed nuget packages.
You'll need to setup your unique key for the Face API, and replace it in the code:
```cs
private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("_Your subscription key_"); 
```
The rover is a simple robot with 2 motors. Using the vehicule class and asking for a 360° turn will turn the vehicule of 90°. This depends of your vehicule and you may want to change it. You can adjust as well the window you want to track faces (symetrical so far) by changing those 2 variables:
```cs
double percent = 0.20;
int degreesperRotation = 4;
```
The first one to adjust at which percentage on the left and right side of the screen, the robot will start adjusting its angle
The second is the number of motor rotation to make a full turn of the robot. In my case 4 as 1 full turn will make the robot turn 90°

Please note that the USBCam helper class take a picture and saves it. All pictures are saved into the the image folder of the default user. Can be usefull for debugging and replaying it. 
Project Oxford API is very well documented on the Project Oxford site. 
