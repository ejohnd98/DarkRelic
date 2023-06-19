namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Game reader class, this is an extension of BinaryReader that reads saved game files written by GameWriter
    /// </summary>
    public class GameReader : BinaryReader
    {
        private uint mMarker = 1;
        private bool mHoldMarker = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fs">File stream</param>
        public GameReader(FileStream fs) : base(fs)
        {
        }

        /// <summary>
        /// Read a boolean value
        /// </summary>
        /// <returns>Boolean</returns>
        public override bool ReadBoolean()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadBoolean();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a byte value
        /// </summary>
        /// <returns>Byte</returns>
        public override byte ReadByte()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadByte();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a byte array
        /// </summary>
        /// <param name="count">Element count</param>
        /// <returns>Byte array</returns>
        public override byte[] ReadBytes(int count)
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadBytes(count);
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a signed byte
        /// </summary>
        /// <returns>Signed byte</returns>
        public override sbyte ReadSByte()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadSByte();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a byte array into an existing array offset
        /// </summary>
        /// <param name="buf">Destination</param>
        /// <param name="index">Index to insert at</param>
        /// <param name="count">Count of elements</param>
        /// <returns>Count read</returns>
        public override int Read(byte[] buf, int index, int count)
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.Read(buf, index, count);
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a char
        /// </summary>
        /// <returns>Char</returns>
        public override char ReadChar()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadChar();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a char array
        /// </summary>
        /// <param name="count">Count of elements</param>
        /// <returns>Char array</returns>
        public override char[] ReadChars(int count)
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadChars(count);
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a single
        /// </summary>
        /// <returns>Single</returns>
        public override float ReadSingle()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadSingle();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a double
        /// </summary>
        /// <returns>Double</returns>
        public override double ReadDouble()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadDouble();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read an unsigned short
        /// </summary>
        /// <returns>Unsigned short</returns>
        public override ushort ReadUInt16()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadUInt16();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a signed short
        /// </summary>
        /// <returns>Short</returns>
        public override short ReadInt16()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadInt16();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read an unsigned integer
        /// </summary>
        /// <returns>Unsigned integer</returns>
        public override uint ReadUInt32()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadUInt32();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a signed integer
        /// </summary>
        /// <returns>Integer</returns>
        public override int ReadInt32()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadInt32();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read an unsigned long
        /// </summary>
        /// <returns>Unsigned long</returns>
        public override ulong ReadUInt64()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadUInt64();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Read a signed long
        /// </summary>
        /// <returns>Long</returns>
        public override long ReadInt64()
        {
            CheckMarker();

            mHoldMarker = true;
            var value = base.ReadInt64();
            mHoldMarker = false;

            return value;
        }

        /// <summary>
        /// Check that a written debug marker is valid
        /// </summary>
        private void CheckMarker()
        {
            // On Unity some BinaryStream apis are implemented using other BinaryStream apis, eg ReadBoolean() calls
            // ReadByte(), this could cause a nested marker check. We can avoid that with mHoldMarker
            if (mHoldMarker)
            {
                return;
            }

            uint actualMarker = base.ReadUInt32();
            uint expectedMarker = 0xC0FE0000 | mMarker;

            if (actualMarker != expectedMarker)
            {
                throw new System.Exception("Corrupt saved game data, marker at: " + actualMarker.ToString("X") + ", expected: " + expectedMarker.ToString("X"));
            }

            mMarker++;
        }
    }
}
