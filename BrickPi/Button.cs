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

// This code is for compatibility and simetry with the Python code
// Need to reviewed and reimplemented when access to a Joypad
namespace BrickPi
{
    public sealed class BrickButtonsStruct
    {
        private bool l1;
        private bool l2;
        private bool r1;
        private bool r2;
        private bool a;
        private bool b;
        private bool c;
        private bool d;
        private bool triangle;
        private bool square;
        private bool circle;
        private bool cross;
        private bool leftJoystick;
        private bool rightJoystick;
        private int leftJoystickX;
        private int leftJoystickY;
        private int rightJoystickX;
        private int rightJoystickY;

        public bool L1
        {
            get
            {
                return l1;
            }

            set
            {
                l1 = value;
            }
        }

        public bool L2
        {
            get
            {
                return l2;
            }

            set
            {
                l2 = value;
            }
        }

        public bool R1
        {
            get
            {
                return r1;
            }

            set
            {
                r1 = value;
            }
        }

        public bool R2
        {
            get
            {
                return r2;
            }

            set
            {
                r2 = value;
            }
        }

        public bool A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
            }
        }

        public bool B
        {
            get
            {
                return b;
            }

            set
            {
                b = value;
            }
        }

        public bool C
        {
            get
            {
                return c;
            }

            set
            {
                c = value;
            }
        }

        public bool D
        {
            get
            {
                return d;
            }

            set
            {
                d = value;
            }
        }

        public bool Triangle
        {
            get
            {
                return triangle;
            }

            set
            {
                triangle = value;
            }
        }

        public bool Square
        {
            get
            {
                return square;
            }

            set
            {
                square = value;
            }
        }

        public bool Circle
        {
            get
            {
                return circle;
            }

            set
            {
                circle = value;
            }
        }

        public bool Cross
        {
            get
            {
                return cross;
            }

            set
            {
                cross = value;
            }
        }

        public bool LeftJoystick
        {
            get
            {
                return leftJoystick;
            }

            set
            {
                leftJoystick = value;
            }
        }

        public bool RightJoystick
        {
            get
            {
                return rightJoystick;
            }

            set
            {
                rightJoystick = value;
            }
        }

        public int LeftJoystickX
        {
            get
            {
                return leftJoystickX;
            }

            set
            {
                leftJoystickX = value;
            }
        }

        public int LeftJoystickY
        {
            get
            {
                return leftJoystickY;
            }

            set
            {
                leftJoystickY = value;
            }
        }

        public int RightJoystickX
        {
            get
            {
                return rightJoystickX;
            }

            set
            {
                rightJoystickX = value;
            }
        }

        public int RightJoystickY
        {
            get
            {
                return rightJoystickY;
            }

            set
            {
                rightJoystickY = value;
            }
        }
    }


    public partial class  Brick
    {
        private void UpdateButtons()
        {
//            class button :
//# Initialize all the buttons to 0
//    def init(self):
//      self.l1=0
//      self.l2=0
//      self.r1=0
//      self.r2=0
//      self.a=0
//      self.b=0
//      self.c=0
//      self.d=0
//      self.tri=0
//      self.sqr=0
//      self.cir=0
//      self.cro=0
//      self.ljb=0
//      self.ljx=0
//      self.ljy=0
//      self.rjx=0
//      rjy=0

//    # Update all the buttons
//    def upd(self, I2C_PORT):
//      #For all buttons:
//      #0:  Unpressed
//      #1:  Pressed
//      #
//      #Left and right joystick: -127 to 127
//      self.ljb=~(BrickPi.SensorI2CIn[I2C_PORT][0][0]>>1)&1
//      self.rjb=~(BrickPi.SensorI2CIn[I2C_PORT][0][0]>>2)&1

//      #For buttons a,b,c,d
//      self.d=~(BrickPi.SensorI2CIn[I2C_PORT][0][0]>>4)&1
//      self.c=~(BrickPi.SensorI2CIn[I2C_PORT][0][0]>>5)&1
//      self.b=~(BrickPi.SensorI2CIn[I2C_PORT][0][0]>>6)&1
//      self.a=~(BrickPi.SensorI2CIn[I2C_PORT][0][0]>>7)&1

//      #For buttons l1,l2,r1,r2
//      self.l2=~(BrickPi.SensorI2CIn[I2C_PORT][0][1])&1
//      self.r2=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>1)&1
//      self.l1=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>2)&1
//      self.r1=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>3)&1

//      #For buttons square,triangle,cross,circle
//      self.tri=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>4)&1
//      self.cir=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>5)&1
//      self.cro=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>6)&1
//      self.sqr=~(BrickPi.SensorI2CIn[I2C_PORT][0][1]>>7)&1

//      #Left joystick x and y , -127 to 127
//      self.ljx=BrickPi.SensorI2CIn[I2C_PORT][0][2]-128
//      self.ljy=~BrickPi.SensorI2CIn[I2C_PORT][0][3]+129

//      #Right joystick x and y , -127 to 127
//      self.rjx=BrickPi.SensorI2CIn[I2C_PORT][0][4]-128
//      self.rjy=~BrickPi.SensorI2CIn[I2C_PORT][0][5]+129

//    #Show button values
//    def show_val(self):
//      print("ljb","rjb","d","c","b","a","l2","r2","l1","r1","tri","cir","cro","sqr","ljx","ljy","rjx","rjy")
//      print(self.ljb," ", self.rjb," ", self.d, self.c, self.b, self.a, self.l2,"", self.r2,"", self.l1,"", self.r1,"", self.tri," ", self.cir," ", self.cro," ", self.sqr," ", self.ljx," ", self.ljy," ", self.rjx," ", self.rjy)
//      print("")




        }
}
}
