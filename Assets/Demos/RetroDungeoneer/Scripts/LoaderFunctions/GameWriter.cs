namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Game writer class, this is an extension of BinaryWriter that writers markers for everything
    /// written to help with debugging saved games. Should not be used when not debugging as it writes
    /// too much data
    /// </summary>
    public class GameWriter : BinaryWriter
    {
        private uint mMarker = 1;
        private bool mHoldMarker = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fs">File stream</param>
        public GameWriter(FileStream fs) : base(fs)
        {
        }

        /// <summary>
        /// Write a boolean
        /// </summary>
        /// <param name="val">Boolean</param>
        public override void Write(bool val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a byte
        /// </summary>
        /// <param name="val">Byte</param>
        public override void Write(byte val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a byte array
        /// </summary>
        /// <param name="buf">Array</param>
        public override void Write(byte[] buf)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(buf);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a signed byte
        /// </summary>
        /// <param name="val">Signed byte</param>
        public override void Write(sbyte val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a byte array from index
        /// </summary>
        /// <param name="buf">Array</param>
        /// <param name="index">Index</param>
        /// <param name="count">Count of elements</param>
        public override void Write(byte[] buf, int index, int count)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(buf, index, count);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a char
        /// </summary>
        /// <param name="val">Char</param>
        public override void Write(char val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a char buffer
        /// </summary>
        /// <param name="buf">Buffer</param>
        public override void Write(char[] buf)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(buf);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a char buffer from index
        /// </summary>
        /// <param name="buf">Buffer</param>
        /// <param name="index">Index</param>
        /// <param name="count">Count of elements</param>
        public override void Write(char[] buf, int index, int count)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(buf, index, count);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a float
        /// </summary>
        /// <param name="val">Float</param>
        public override void Write(float val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a double
        /// </summary>
        /// <param name="val">Double</param>
        public override void Write(double val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write an unsigned short
        /// </summary>
        /// <param name="val">Short</param>
        public override void Write(ushort val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a signed short
        /// </summary>
        /// <param name="val">Short</param>
        public override void Write(short val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write an unsigned integer
        /// </summary>
        /// <param name="val">Integer</param>
        public override void Write(uint val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a signed integer
        /// </summary>
        /// <param name="val">Integer</param>
        public override void Write(int val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write an unsigned long
        /// </summary>
        /// <param name="val">Long</param>
        public override void Write(ulong val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a signed long
        /// </summary>
        /// <param name="val">Long</param>
        public override void Write(long val)
        {
            WriteMarker();

            mHoldMarker = true;
            base.Write(val);
            mHoldMarker = false;
        }

        /// <summary>
        /// Write a debug marker
        /// </summary>
        public void WriteMarker()
        {
            // On Unity some BinaryStream apis are implemented using other BinaryStream apis, eg ReadBoolean() calls
            // ReadByte(), this could cause a nested marker check. We can avoid that with mHoldMarker
            if (mHoldMarker)
            {
                return;
            }

            base.Write(0xC0FE0000 | mMarker);

            mMarker++;
        }
    }
}
