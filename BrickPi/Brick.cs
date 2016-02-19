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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace BrickPi
{
    public sealed partial class Brick
    {
        /// <summary>
        /// Private variables all are static as only 1 brick can run with this code.
        /// </summary>
        static private SerialDevice serialPort = null;
        static private BrickPiStruct brickPi = null;
        static private BrickButtonsStruct brickButtons;
        static private CancellationTokenSource ReadCancellationTokenSource;
        static private bool isThreadRunning = false;
        static private DataReader dataReaderObject = null;
        static private DataWriter dataWriteObject = null;
        static private bool needSetup = false;

        /// <summary>
        /// Property for the main BrickPi Structure
        /// </summary>
        public BrickPiStruct BrickPi
        {
            get
            {
                return brickPi;
            }

            internal set
            {
                brickPi = value;
            }
        }

        /// <summary>
        /// This returns status of joystick/pad
        /// This has not been tested at all
        /// </summary>
        public BrickButtonsStruct BrickButtons
        {
            get
            {
                return brickButtons;
            }

            set
            {
                brickButtons = value;
            }
        }

        /// <summary>
        /// Initialize the birck, create the strcture and setup the serial port
        /// </summary>
        public Brick():this(null)
        {

        }

        public Brick(SerialDevice myserial)
        {
            if (brickPi == null)
                brickPi = new BrickPiStruct();
            if (serialPort == null)
                SelectAndInitSerial(myserial).Wait();
            needSetup = true;
            if (!isThreadRunning)
            {
                Start();
                Task.Delay(100).Wait();
            }
        }

        /// <summary>
        /// Initialize the Serial port, the BrickPi 
        /// </summary>
        /// <returns></returns>
        private async Task SelectAndInitSerial(SerialDevice myserial)
        {
            try
            {
                if (myserial == null)
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
                }
                else
                    serialPort = myserial;
                if (serialPort!=null)
                {
                    serialPort.BaudRate = 500000; //the communication speed with the 2 arduinos.
                    // you may need to do the following steps into your RaspberryPi rinnong Windows IoT core:
                    //Reg add hklm\system\controlset001\services\serpl011\parameters /v MaxBaudRateNoDmaBPS /t REG_DWORD /d 921600
                    //Devcon restart acpi\bcm2837

                    //initializing the defaults timeout and other serial data
                    serialPort.WriteTimeout = TimeSpan.FromMilliseconds(BrickPi.Timeout);
                    serialPort.ReadTimeout = TimeSpan.FromMilliseconds(BrickPi.Timeout);
                    serialPort.Parity = SerialParity.None;
                    serialPort.StopBits = SerialStopBitCount.One;
                    serialPort.DataBits = 8;
                    //this will be used only for debug. Nothing else is implemented so far
                    serialPort.ErrorReceived += SerialPort_ErrorReceived;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Exception initializing serrail port: {0}", ex.Message));
            }
        }

        /// <summary>
        /// Used for debug only is any issue happen to the Serial port
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void SerialPort_ErrorReceived(SerialDevice sender, ErrorReceivedEventArgs args)
        {
            Debug.WriteLine(string.Format("Serial error {0}", args.Error));
        }

        /// <summary>
        /// Send data to the BrickPi
        /// </summary>
        /// <param name="dest">The target Arduino, can be 1 or 2</param>
        /// <param name="byteCount">number of bytes to send</param>
        /// <param name="OutArray">byte array to send</param>
        private async void BrickPiTx(int dest, int byteCount, byte[] OutArray)
        {
            if (byteCount > OutArray.Length)
                return;
            byte[] txBuff = new byte[3 + byteCount];
            txBuff[0] = (byte)dest;
            txBuff[1] = (byte)((dest + byteCount + sum(0, byteCount, OutArray)) % 256);
            txBuff[2] = (byte)byteCount;
            //OutArray.CopyTo(txBuff, 3);
            for (int i = 0; i < byteCount; i++)
                txBuff[i + 3] = OutArray[i];
            
            try
            {
                if (serialPort != null)
                {
                    Task<UInt32> storeAsyncTask;
                    //Launch the WriteAsync task to perform the write
                    if (dataWriteObject == null)
                        dataWriteObject = new DataWriter(serialPort.OutputStream);
                    dataWriteObject.WriteBytes(txBuff);
                    storeAsyncTask = dataWriteObject.StoreAsync().AsTask();
                    UInt32 bytesWritten = await storeAsyncTask;
                    if (bytesWritten > 0)
                    {
                        //Debug.WriteLine(string.Format("Bytes written successfully: {0}", bytesWritten));
                    } else
                    {
                        Debug.WriteLine("Error sending data");
                    }
                }
                else
                {
                    Debug.WriteLine("No serial port initialized");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("Eror sending data: {0}", ex.Message));
                // Cleanup once complete
                if (dataWriteObject != null)
                {
                    dataWriteObject.DetachStream();
                    dataWriteObject = null;
                }
            }
        }

        /// <summary>
        /// Read data coming from the BrickPi
        /// </summary>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns></returns>
        private async Task<byte[]> BrickPiRx(int timeout)
        {
            // clean token
            if (ReadCancellationTokenSource == null)
                ReadCancellationTokenSource = new CancellationTokenSource();
            //ReadCancellationTokenSource.CancelAfter(timeout + brickPi.Timeout);
            //read from the serial port
            byte[] rx_buffer = await ReadAsync(timeout, ReadCancellationTokenSource.Token);
            //make sure token is clened up
            //CancelReadTask();
            // check if data are valids
            if (rx_buffer == null)
                return null;
            if (rx_buffer.Length < 2)
                return null;
            if (rx_buffer.Length < (rx_buffer[1] + 2))
                return null;
            long CheckSum = (sum(1, rx_buffer.Length - 1, rx_buffer) % 256);
            if (CheckSum != rx_buffer[0])
                return null;
            byte[] retval = new byte[rx_buffer.Length - 2];
            //create the returned buffer, remove the message lenght and checksum
            for (int i = 0; i < (rx_buffer.Length-2); i++)
                retval[i] = rx_buffer[i + 2];
            return retval;
        }

        /// <summary>
        /// checksum calculator
        /// </summary>
        /// <param name="idxStart">start index</param>
        /// <param name="byteCount">number of bytes</param>
        /// <param name="arrayToCount">byte array</param>
        /// <returns></returns>
        private long sum(int idxStart, int byteCount, byte[] arrayToCount)
        {
            //check erros
            if (idxStart < 0)
                return long.MaxValue;
            if ((idxStart + byteCount) > arrayToCount.Length)
                return long.MaxValue;
            //do the sum
            long retval = 0;
            for(int i = idxStart; i<(idxStart + byteCount); i++)
            {
                retval += arrayToCount[i];
            }
            return retval;
        }

        /// <summary>
        /// ReadAsync: Task that waits on data and reads asynchronously from the serial device InputStream
        /// </summary>
        /// <param name="cancellationToken">cancellation token for async read</param>
        /// <returns></returns>
        private async Task<byte[]> ReadAsync(int timeout, CancellationToken cancellationToken)
        {
            try
            {
                Task<UInt32> loadAsyncTask;

                uint ReadBufferLength = 1024;

                // If task cancellation was requested, comply
                cancellationToken.ThrowIfCancellationRequested();
                if (dataReaderObject == null)
                    dataReaderObject = new DataReader(serialPort.InputStream);
                // Set InputStreamOptions to complete the asynchronous read operation when one or more bytes is available
                dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
                // set serial timeout for reading. initialize the timout reading function
                serialPort.ReadTimeout = TimeSpan.FromMilliseconds(timeout);
                //await Task.Delay(timeout);
                // Create a task object to wait for data on the serialPort.InputStream
                loadAsyncTask = dataReaderObject.LoadAsync(ReadBufferLength).AsTask(cancellationToken);
                // Launch the task and wait
                UInt32 bytesRead = await loadAsyncTask;
                byte[] retval = null;
                if (bytesRead > 0)
                {
                    retval = new byte[bytesRead];
                    dataReaderObject.ReadBytes(retval); //return the bytes read
                    //Debug.WriteLine(String.Format("Bytes received successfully: {0}", bytesRead));
                }
                else
                    Debug.WriteLine(String.Format("No bytes received successfully"));
                return retval;
            }
            catch (Exception)
            {
                // Cleanup once complete
                if (dataReaderObject != null)
                {
                    dataReaderObject.DetachStream();
                    dataReaderObject = null;
                }
                //if the cause if the token, then reset it 
                return null;
            }
            
        }

        /// <summary>
        /// CancelReadTask:
        /// - Uses the ReadCancellationTokenSource to cancel read operations
        /// </summary>
        private void CancelReadTask()
        {
            if (ReadCancellationTokenSource != null)
            {
                if (!ReadCancellationTokenSource.IsCancellationRequested)
                {
                    ReadCancellationTokenSource.Cancel();
                }
            }
        }

        /// <summary>
        /// CloseDevice:
        /// - Disposes SerialDevice object
        /// - Clears the enumerated device Id list
        /// </summary>
        private void CloseDevice()
        {
            if (serialPort != null)
            {
                serialPort.Dispose();
            }
            serialPort = null;

        }


}

}
