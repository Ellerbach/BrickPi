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
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BrickPi
{
    public partial class Brick
    {
        /// <summary>
        /// Private variables all are static as only 1 brick can run with this code.
        /// </summary>
        static private Task ThreadReading = null;
        static private bool needTimeout = false;
        static private int[] prevType = new int[4] { -1, -1, -1, -1 }; //to force update at start
        /// <summary>
        /// Used to change the address of the Ardunio
        /// WARNING: don't use it except if you really know what you are doing
        /// </summary>
        /// <param name="OldAddr">old address</param>
        /// <param name="NewAddr">new address</param>
        /// <returns></returns>
        private async Task<bool> BrickPiChangeAddress(byte OldAddr, byte NewAddr)
        {
            DataArray dataArray = new DataArray();
            dataArray.myArray[BYTE_MSG_TYPE] = MSG_TYPE_CHANGE_ADDR;
            dataArray.myArray[BYTE_NEW_ADDRESS] = NewAddr;
            BrickPiTx(OldAddr, 2, dataArray.myArray);
            byte[] InArray = await BrickPiRx(5);
            byte[] OutArray;
            return CheckRetMessage(InArray, MSG_TYPE_CHANGE_ADDR, out OutArray);
        }

        /// <summary>
        /// Check if the returned buffer is all correct
        /// </summary>
        /// <param name="InArray">input array</param>
        /// <param name="msg">the message to test</param>
        /// <param name="OutArray">the array as an ouput</param>
        /// <param name="OutArrayLength">number of bytes for the return array</param>
        /// <returns></returns>
        private bool CheckRetMessage(byte[] InArray, byte msg, out byte[] OutArray, int OutArrayLength = 256)
        {
            OutArray = null;
            if (InArray == null)
                return false;
            if (InArray.Length == 1)
                return false;
            //is the value really changed? 
            if (!(InArray[BYTE_MSG_TYPE] == msg))
                return false;
            if (OutArrayLength < InArray.Length)
                return false;
            //saved new data to the main buffer
            OutArray = new byte[OutArrayLength];
            for (int i = 0; i < InArray.Length; i++)
                OutArray[i] = InArray[i];
            //all good!
            return true;

        }

        /// <summary>
        /// Set a new timeout on the brick
        /// WARNING: This has not been tested intensively, most likely, need to change some of the timeouts
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BrickPiSetTimeout()
        {
            DataArray dataArray = new DataArray();
            bool retval = true;
            for (int i = 0; i < 2; i++)
            {
                dataArray.myArray[BYTE_MSG_TYPE] = MSG_TYPE_TIMEOUT_SETTINGS;
                dataArray.myArray[BYTE_TIMEOUT] = (byte)(brickPi.Timeout & 0xFF);
                dataArray.myArray[BYTE_TIMEOUT + 1] = (byte)((brickPi.Timeout / 256) & 0xFF);
                dataArray.myArray[BYTE_TIMEOUT + 2] = (byte)((brickPi.Timeout / 65536) & 0xFF);
                dataArray.myArray[BYTE_TIMEOUT + 3] = (byte)((brickPi.Timeout / 16777216) & 0xFF);
                BrickPiTx(brickPi.Address[i], 5, dataArray.myArray);
                // IMPORTANT: theoritical timeout is 2.5 ms
                // Working all the time with 20
                byte[] InArray = await BrickPiRx(20); //0.002500
                byte[] OutArray;
                retval &= CheckRetMessage(InArray, MSG_TYPE_TIMEOUT_SETTINGS, out OutArray);
            }
            return retval;
        }

        public void UpdateValues()
        {
            BrickPiUpdateValues().Wait();
        }

        private void IsSetupNeeded()
        {
            
            for(int idxArduino = 0; idxArduino<2; idxArduino++)
            {
                bool needsetup = false;
                for (int i=0; i<2; i++)
                {
                    int port = idxArduino * 2 + i;
                    if (prevType[port] != (int)brickPi.Sensor[port].Type)
                    {
                        needsetup = true;
                        prevType[port] = (int)brickPi.Sensor[port].Type;
                    }
                }
                if (needsetup)
                    BrickPiSetupSensors(idxArduino).Wait();
            }
        }

        /// <summary>
        /// Main function running in the main thread, checking all the time the status of sensors
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BrickPiUpdateValues()
        {
            //If setup is needed, then first setup the sensors
            IsSetupNeeded();
            //if need to change the timeout of the brick
            if (needTimeout)
            {
                BrickPiSetTimeout().Wait();
                needTimeout = false;
            }


            DataArray dataArray = new DataArray();
            int Retried = 0;

            bool ret = false;
            int idxArduino = 0;
            while (idxArduino < 2)
            {
                if (!ret)
                    Retried = 0;
                //Fill the header of buffer for communication
                // BYTE_MSG_TYPE, position 0, Message to send
                for (int ii = 0; ii < dataArray.myArray.Length; ii++)
                    dataArray.myArray[ii] = 0;
                dataArray.myArray[BYTE_MSG_TYPE] = MSG_TYPE_VALUES;
                dataArray.Bit_Offset = 0;
                // This second part is specific to motors encoders.
                for (int idxPort = 0; idxPort < 2; idxPort++)
                {
                    int port = (idxArduino * 2) + idxPort;
                    if (BrickPi.Motor[port].EncoderOffset != 0)
                    {
                        int Temp_Value = BrickPi.Motor[port].EncoderOffset;
                        dataArray.AddBits(1, 0, 1, 1);
                        int Temp_ENC_DIR = 0;
                        if (Temp_Value < 0)
                        {
                            Temp_ENC_DIR = 1;
                            Temp_Value *= -1;
                        }
                        byte Temp_BitsNeeded = (byte)(dataArray.BitsNeeded(Temp_Value) + 1);
                        dataArray.AddBits(1, 0, 5, Temp_BitsNeeded);
                        Temp_Value *= 2;
                        Temp_Value |= Temp_ENC_DIR;
                        dataArray.AddBits(1, 0, Temp_BitsNeeded, Temp_Value);
                    }
                    else
                        dataArray.AddBits(1, 0, 1, 0);
                }
                // This third part is specific to motors speed and direction
                for (int idxPort = 0; idxPort < 2; idxPort++)
                {
                    int port = (idxArduino * 2) + idxPort;
                    int speed = BrickPi.Motor[port].Speed;
                    int direc = 0;
                    if (speed < 0)
                    {
                        direc = 1;
                        speed *= -1;
                    }
                    if (speed > 255)
                        speed = 255;
                    dataArray.AddBits(1, 0, 10, ((((speed & 0xFF) << 2) | (direc << 1) | (BrickPi.Motor[port].Enable & 0x01)) & 0x3FF));
                }
                // This part is specific to I2C sensors
                for (int idxPort = 0; idxPort < 2; idxPort++)
                {
                    int port = (idxArduino * 2) + idxPort;
                    if ((BrickPi.Sensor[port].Type == BrickSensorType.I2C) || 
                        (BrickPi.Sensor[port].Type == BrickSensorType.I2C_9V) || 
                        (BrickPi.Sensor[port].Type == BrickSensorType.ULTRASONIC_CONT))
                    {
                        for (int device = 0; device < BrickPi.I2C[port].Devices; device++)
                        {
                            if ((BrickPi.Sensor[port].Settings[device] & BIT_I2C_SAME) != BIT_I2C_SAME)
                            {
                                dataArray.AddBits(1, 0, 4, BrickPi.I2C[port].Write[device]);
                                dataArray.AddBits(1, 0, 4, BrickPi.I2C[port].Read[device]);
                                for (int out_byte = 0; out_byte < BrickPi.I2C[port].Write[device]; out_byte++)
                                    dataArray.AddBits(1, 0, 8, BrickPi.I2C[port].Out[device].InOut[out_byte]);
                            }
                            // ITODO don't understadn why??? so letting it as it is:
                            device += 1;
                        }
                    }
                }
                //Send the data
                int tx_bytes = (((dataArray.Bit_Offset + 7) / 8) + 1); // #eq to UART_TX_BYTES
                BrickPiTx(BrickPi.Address[idxArduino], tx_bytes, dataArray.myArray);
                //wait for answer
                // IMPORTANT: in theory, waiting time if 7.5 ms but it does create problems
                // so it is working fine with a 20 ms timeout. 
                byte[] InArray = await BrickPiRx(20); //theory 7.5 ms
                if (InArray == null)
                    continue;
                // first part is about encoders
                BrickPi.Motor[(idxArduino * 2) + (int)BrickPortMotor.PORT_A].EncoderOffset = 0;
                BrickPi.Motor[(idxArduino * 2) + (int)BrickPortMotor.PORT_B].EncoderOffset = 0;
                byte[] OutArray;
                //check if we got the right answer to the question
                if (!CheckRetMessage(InArray, MSG_TYPE_VALUES, out OutArray))
                {
                    Debug.WriteLine(String.Format("BrickPi Error reading value in Updating values"));
                    if (Retried < 2)
                    {
                        ret = true;
                        Retried += 1;
                        continue;
                    }
                }
                dataArray.myArray = OutArray;

                ret = false;
                dataArray.Bit_Offset = 0;
                // number of bits to be used for encoders
                List<byte> Temp_BitsUsed = new List<byte>();

                Temp_BitsUsed.Add((byte)dataArray.GetBits(1, 0, 5));
                Temp_BitsUsed.Add((byte)dataArray.GetBits(1, 0, 5));

                for (int idxPort = 0; idxPort < 2; idxPort++)
                {
                    long Temp_EncoderVal = dataArray.GetBits(1, 0, Temp_BitsUsed[idxPort]);
                    if ((Temp_EncoderVal & 0x01) == 1)
                    {
                        Temp_EncoderVal /= 2;
                        BrickPi.Motor[idxPort + idxArduino * 2].Encoder = (int)(Temp_EncoderVal) * -1;
                    }
                    else
                        BrickPi.Motor[idxPort + idxArduino * 2].Encoder = (int)(Temp_EncoderVal / 2);
                }
                // This is the part with sensors
                for (int idxPort = 0; idxPort < 2; idxPort++)
                {
                    int port = idxPort + (idxArduino * 2);
                    switch (BrickPi.Sensor[port].Type)
                    {
                        case BrickSensorType.SENSOR_RAW:
                        //this is 0 value, LIGHT_OFF is as well 0
                        //case BrickSensorType.LIGHT_OFF:
                            break;
                        case BrickSensorType.LIGHT_ON:
                            break;
                        case BrickSensorType.TOUCH:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 1);
                            break;
                        case BrickSensorType.ULTRASONIC_SS:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 8);
                            break;
                        case BrickSensorType.RCX_LIGHT:
                            break;
                        case BrickSensorType.COLOR_FULL:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 3);
                            BrickPi.Sensor[port].Array[INDEX_BLANK] = (int)dataArray.GetBits(1, 0, 10);
                            BrickPi.Sensor[port].Array[INDEX_RED] = (int)dataArray.GetBits(1, 0, 10);
                            BrickPi.Sensor[port].Array[INDEX_GREEN] = (int)dataArray.GetBits(1, 0, 10);
                            BrickPi.Sensor[port].Array[INDEX_BLUE] = (int)dataArray.GetBits(1, 0, 10);
                            break;
                        case BrickSensorType.ULTRASONIC_CONT:
                        case BrickSensorType.I2C:
                        case BrickSensorType.I2C_9V:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, BrickPi.I2C[port].Devices);
                            for (int device = 0; device < BrickPi.I2C[port].Devices; device++)
                            {
                                if ((BrickPi.Sensor[port].Value & (0x01 << device)) != 0)
                                    for (int in_byte = 0; in_byte < BrickPi.I2C[port].Read[device]; in_byte++)
                                        BrickPi.I2C[port].In[device].InOut[in_byte] = (int)dataArray.GetBits(1, 0, 8);
                            }
                            if (BrickPi.Sensor[port].Type == BrickSensorType.ULTRASONIC_CONT)
                            {
                                if (((int)BrickPi.Sensor[port].Value & (0x01 << US_I2C_IDX)) != 0)
                                    BrickPi.Sensor[port].Value = BrickPi.I2C[port].In[US_I2C_IDX].InOut[0];
                                else
                                    BrickPi.Sensor[port].Value = -1;
                            }

                            break;
                        case BrickSensorType.EV3_INFRARED_M2:
                        case BrickSensorType.EV3_GYRO_M3:
                        case BrickSensorType.EV3_COLOR_M3:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 32);
                            break;
                        case BrickSensorType.EV3_US_M0:
                        case BrickSensorType.EV3_US_M1:
                        case BrickSensorType.EV3_US_M2:
                        case BrickSensorType.EV3_US_M3:
                        case BrickSensorType.EV3_US_M4:
                        case BrickSensorType.EV3_US_M5:
                        case BrickSensorType.EV3_US_M6:
                        case BrickSensorType.EV3_COLOR_M0:
                        case BrickSensorType.EV3_COLOR_M1:
                        case BrickSensorType.EV3_COLOR_M2:
                        case BrickSensorType.EV3_COLOR_M4:
                        case BrickSensorType.EV3_COLOR_M5:
                        case BrickSensorType.EV3_GYRO_M0:
                        case BrickSensorType.EV3_GYRO_M1:
                        case BrickSensorType.EV3_GYRO_M2:
                        case BrickSensorType.EV3_GYRO_M4:
                        case BrickSensorType.EV3_INFRARED_M0:
                        case BrickSensorType.EV3_INFRARED_M1:
                        case BrickSensorType.EV3_INFRARED_M3:
                        case BrickSensorType.EV3_INFRARED_M4:
                        case BrickSensorType.EV3_INFRARED_M5:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 16);
                            //# EV3 Gyro Mode 0, Adjust sign
                            if (BrickPi.Sensor[port].Type == BrickSensorType.EV3_GYRO_M0)
                            {
                                if (BrickPi.Sensor[port].Value >= 32767)        //# Negative number.  This seems to return a 2 byte number.
                                    BrickPi.Sensor[port].Value = BrickPi.Sensor[port].Value - 65535;                             
                            }
                            //# EV3 Gyro Mode 1, Adjust sign
                            else if (BrickPi.Sensor[port].Type == BrickSensorType.EV3_GYRO_M1)
                            {
                                if (BrickPi.Sensor[port].Value >= 32767) //		# Negative number.  This seems to return a 2 byte number.
                                    BrickPi.Sensor[port].Value = BrickPi.Sensor[port].Value - 65535;
                            }
                            break;
                        case BrickSensorType.EV3_TOUCH_0:
                            BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 16);
                            break;
                        case BrickSensorType.EV3_TOUCH_DEBOUNCE:
                        case BrickSensorType.TOUCH_DEBOUNCE:
                        case BrickSensorType.COLOR_RED:
                        case BrickSensorType.COLOR_GREEN:
                        case BrickSensorType.COLOR_BLUE:
                        case BrickSensorType.COLOR_NONE:
                        default:
                            BrickPi.Sensor[idxPort + (idxArduino * 2)].Value = (int)dataArray.GetBits(1, 0, 10);
                            break;
                    }
                    #region oldcode
                    //if (BrickPi.Sensor[port].Type == BrickSensorType.TOUCH)
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 1);
                    //else if (BrickPi.Sensor[port].Type == BrickSensorType.ULTRASONIC_SS)
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 8);
                    //else if (BrickPi.Sensor[port].Type == BrickSensorType.COLOR_FULL)
                    //{
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 3);
                    //    BrickPi.Sensor[port].Array[INDEX_BLANK] = (int)dataArray.GetBits(1, 0, 10);
                    //    BrickPi.Sensor[port].Array[INDEX_RED] = (int)dataArray.GetBits(1, 0, 10);
                    //    BrickPi.Sensor[port].Array[INDEX_GREEN] = (int)dataArray.GetBits(1, 0, 10);
                    //    BrickPi.Sensor[port].Array[INDEX_BLUE] = (int)dataArray.GetBits(1, 0, 10);
                    //}
                    //else if ((BrickPi.Sensor[port].Type == BrickSensorType.I2C) || (BrickPi.Sensor[port].Type == BrickSensorType.I2C_9V) || (BrickPi.Sensor[port].Type == BrickSensorType.ULTRASONIC_CONT))
                    //{
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, BrickPi.I2C[port].Devices);
                    //    for (int device = 0; device < BrickPi.I2C[port].Devices; device++)
                    //    {
                    //        if ((BrickPi.Sensor[port].Value & (0x01 << device)) != 0)
                    //            for (int in_byte = 0; in_byte < BrickPi.I2C[port].Read[device]; in_byte++)
                    //                BrickPi.I2C[port].In[device].InOut[in_byte] = (int)dataArray.GetBits(1, 0, 8);
                    //    }
                    //}
                    //else if ((BrickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M3) || (BrickPi.Sensor[port].Type == BrickSensorType.EV3_GYRO_M3))
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 32);
                    //else if (BrickPi.Sensor[port].Type == BrickSensorType.EV3_INFRARED_M2)
                    //{
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 32);
                    //    if (BrickPi.Sensor[port].Value < 0)
                    //        Debug.WriteLine("IR SENSOR RETURNED ERROR");
                    //}
                    //else if ((BrickPi.Sensor[port].Type >= BrickSensorType.EV3_US_M0) && (BrickPi.Sensor[port].Type <= BrickSensorType.EV3_INFRARED_M5 + 1))
                    //    BrickPi.Sensor[port].Value = (int)dataArray.GetBits(1, 0, 16);
                    //else // #For all the light, color and raw sensors
                    //    BrickPi.Sensor[ii + (i * 2)].Value = (int)dataArray.GetBits(1, 0, 10);

                    //if (BrickPi.Sensor[port].Type == BrickSensorType.ULTRASONIC_CONT)
                    //{
                    //    if (((int)BrickPi.Sensor[port].Value & (0x01 << US_I2C_IDX)) != 0)
                    //        BrickPi.Sensor[port].Value = BrickPi.I2C[port].In[US_I2C_IDX].InOut[0];
                    //    else
                    //        BrickPi.Sensor[port].Value = -1;
                    //}


                    //# EV3 Gyro Mode 0, Adjust sign
                    //if (BrickPi.Sensor[port].Type == BrickSensorType.EV3_GYRO_M0)
                    //{
                    //    if (BrickPi.Sensor[port].Value >= 32767)		//# Negative number.  This seems to return a 2 byte number.
                    //        BrickPi.Sensor[port].Value = BrickPi.Sensor[port].Value - 65535;
                    //    //#else:					# Positive Number print str(gyro)
                    //    //#######################
                    //    //# EV3 Gyro Mode 1, Adjust sign
                    //}
                    //else if (BrickPi.Sensor[port].Type == BrickSensorType.EV3_GYRO_M1)
                    //{
                    //    //                # print "Gyro m1!"
                    //    if (BrickPi.Sensor[port].Value >= 32767) //		# Negative number.  This seems to return a 2 byte number.
                    //        BrickPi.Sensor[port].Value = BrickPi.Sensor[port].Value - 65535;
                    //    //				# else:					# Positive Number print str(gyro)
                    //}
                    //            # print BrickPi.SensorType[port]
                    #endregion
                }
                idxArduino++;
            }
            //if all went correctly, then ret should be false
            return !ret;
        }

        /// <summary>
        /// If the brick is running, reading sensors and moving motors
        /// </summary>
        public bool IsRunning
        { get { return isThreadRunning; } }

        /// <summary>
        /// Stop the brick, stop all reading, writing, motor movement
        /// </summary>
        public void Stop()
        {
            isThreadRunning = false;
            if (ThreadReading != null)
            {
                while (!ThreadReading.IsCompleted)
                {
                    //wait
                }
            }            
            Task.Delay(1000).Wait();
        }

        /// <summary>
        /// Start reading the brick, getting info from sensors, moving motors
        /// </summary>
        public void Start()
        {
            if (isThreadRunning == true)
                return;
            isThreadRunning = true;
            ThreadReading = Windows.System.Threading.ThreadPool.RunAsync(this.ContinuousUpdate, Windows.System.Threading.WorkItemPriority.High).AsTask();
            Task.Delay(100).Wait();
        }

        /// <summary>
        /// The main thread running all the time to check the sensor status
        /// </summary>
        /// <param name="action"></param>
        private void ContinuousUpdate(IAsyncAction action)
        {
            while(isThreadRunning)
                BrickPiUpdateValues().Wait();
        }

        /// <summary>
        /// update the type of sensors. Needed to be able to change mode
        /// don't call directly this function, it will brak the running thread. Call it only thru the 
        /// SetupSensros function.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> BrickPiSetupSensors(int idxArduino)
        {
            DataArray dataArray = new DataArray();
            bool retval = true;

            
                dataArray.Bit_Offset = 0;
                dataArray.myArray[BYTE_MSG_TYPE] = MSG_TYPE_SENSOR_TYPE;
                dataArray.myArray[BYTE_SENSOR_1_TYPE] = (byte)brickPi.Sensor[PORT_1 + idxArduino * 2].Type;
                dataArray.myArray[BYTE_SENSOR_2_TYPE] = (byte)brickPi.Sensor[PORT_2 + idxArduino * 2].Type;
                bool iscolor = false;
                for (int idxPort = 0; idxPort < 2; idxPort++)
                {                    
                    int port = idxArduino * 2 + idxPort;
                    //check if there is a sensor Light. If yes, then we'll need to wait super long to have it setup
                    //othewise, quite fast
                    if ((brickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M0) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M1) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M2) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M3) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M4) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.EV3_COLOR_M5) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.COLOR_BLUE) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.COLOR_FULL) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.COLOR_GREEN) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.COLOR_NONE) ||
                    (brickPi.Sensor[port].Type == BrickSensorType.COLOR_RED)
                    )
                        iscolor = true;
                    if (dataArray.myArray[BYTE_SENSOR_1_TYPE + idxPort] == (byte)BrickSensorType.ULTRASONIC_CONT)
                    {
                        dataArray.myArray[BYTE_SENSOR_1_TYPE + idxPort] = (byte)BrickSensorType.I2C;
                        brickPi.I2C[port].Speed = US_I2C_SPEED;
                        brickPi.I2C[port].Devices = 1;
                        brickPi.Sensor[port].Settings[US_I2C_IDX] = BIT_I2C_MID | BIT_I2C_SAME;
                        brickPi.I2C[port].Address[US_I2C_IDX] = LEGO_US_I2C_ADDR;
                        brickPi.I2C[port].Write[US_I2C_IDX] = 1;
                        brickPi.I2C[port].Read[US_I2C_IDX] = 1;
                        brickPi.I2C[port].Out[US_I2C_IDX].InOut[0] = LEGO_US_I2C_DATA_REG;
                    }
                    if ((dataArray.myArray[BYTE_SENSOR_1_TYPE + idxPort] == (byte)BrickSensorType.I2C) || (dataArray.myArray[BYTE_SENSOR_1_TYPE + idxPort] == (byte)BrickSensorType.I2C_9V))
                    {
                        dataArray.AddBits(3, 0, 8, brickPi.I2C[port].Speed);
                        if (brickPi.I2C[port].Devices > 8)
                            brickPi.I2C[port].Devices = 8;

                        if (brickPi.I2C[port].Devices == 0)
                            brickPi.I2C[port].Devices = 1;

                        dataArray.AddBits(3, 0, 3, (brickPi.I2C[port].Devices - 1));

                        for (int device = 0; device < brickPi.I2C[port].Devices; device++)
                        {
                            dataArray.AddBits(3, 0, 7, (brickPi.I2C[port].Address[device] >> 1));
                            dataArray.AddBits(3, 0, 2, brickPi.Sensor[port].Settings[device]);
                            if ((brickPi.Sensor[port].Settings[device] & BIT_I2C_SAME) == BIT_I2C_SAME)
                            {
                                dataArray.AddBits(3, 0, 4, brickPi.I2C[port].Write[device]);
                                dataArray.AddBits(3, 0, 4, brickPi.I2C[port].Read[device]);

                                for (int out_byte = 0; out_byte < brickPi.I2C[port].Write[device]; out_byte++)
                                    dataArray.AddBits(3, 0, 8, brickPi.I2C[port].Out[device].InOut[out_byte]);
                            }
                        }
                    }
                }
                int tx_bytes = (((dataArray.Bit_Offset + 7) / 8) + 3); //#eq to UART_TX_BYTES
                BrickPiTx(brickPi.Address[idxArduino], tx_bytes, dataArray.myArray);
                //# Timeout set to 5 seconds to setup EV3 sensors successfully
                int timeout = 100;
                if (iscolor)
                {
                    timeout = 5000;
                    Debug.WriteLine("initializing color sensor");
                }
                byte[] InArray = await BrickPiRx(timeout);
                if (!((InArray[BYTE_MSG_TYPE] == MSG_TYPE_SENSOR_TYPE) && (InArray.Length == 1)))
                    retval = false;
            
            return retval;
        }


    }
}
