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

namespace BrickPi
{
    /// <summary>
    /// Used to store and analyze the retruned buffer
    /// </summary>
    sealed class DataArray
    {
        private byte[] mArray;
        //int BytesReceived = None;
        public int Bit_Offset { get; set; }

        public byte[] myArray
        { get { return mArray; } internal set { mArray = value; } }

        public DataArray(): this (256)
        { }
        public DataArray(int ArraySize)
        {
            mArray = new byte[ArraySize];
        }

        /// <summary>
        /// Get bits, so convert number of bits into a long
        /// </summary>
        /// <param name="byte_offset">where to start in byte from the array</param>
        /// <param name="bit_offset">where to start in bits to read</param>
        /// <param name="bits">number of bits to read</param>
        /// <returns></returns>
        public long GetBits(int byte_offset, int bit_offset, int bits)
        {
            long result = 0;
            //global Bit_Offset
            int i = bits;
            while (i > 0)
            {
                result *= 2;
                result |= ((myArray[((byte_offset + ((bit_offset + Bit_Offset + (i - 1)) / 8)))] >> ((bit_offset + Bit_Offset + (i - 1)) % 8)) & 0x01);
                i--;
            }
            Bit_Offset += bits;
            return result;
        }

        /// <summary>
        /// Number of bits need to store the value
        /// </summary>
        /// <param name="value">data to store</param>
        /// <returns></returns>
        public byte BitsNeeded(int value)
        {
            byte retval = 0;
            while (retval < 32)
            {
                if (value == 0)
                    return retval;
                value /= 2;
                retval++;
            }
            return 31;
        }

        /// <summary>
        /// Add bits to the main buffer
        /// </summary>
        /// <param name="byte_offset">where to start in byte from the array</param>
        /// <param name="bit_offset">where to start in bits to read</param>
        /// <param name="bits">number of bits to write</param>
        /// <param name="value">the data to transform as bit</param>
        public void AddBits(byte byte_offset, byte bit_offset, byte bits, int value)
        {
            byte i = 0;
            while (i < bits)
            {
                if (((value) & 0x01) == 1)
                {
                    myArray[((byte_offset + ((bit_offset + Bit_Offset + i) / 8)))] |= (byte)(0x01 << ((bit_offset + Bit_Offset + i) % 8));
                }
                value /= 2;
                i++;
            }
            Bit_Offset += bits;
        }
    }
}
