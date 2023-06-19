namespace RetroBlitDemoRetroDungeoneer
{
    using System.IO;

    /// <summary>
    /// Defines an entity id
    /// </summary>
    public struct EntityID
    {
        /// <summary>
        /// Empty/dummy entity, sometimes used to signify unset/null entity
        /// </summary>
        public static EntityID empty = new EntityID(0, -1);

        private int mIndex;
        private int mGen;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index">index into entity store</param>
        /// <param name="gen">generation of the entity</param>
        public EntityID(int index, int gen)
        {
            this.mIndex = index;
            this.mGen = gen;
        }

        /// <summary>
        /// Constructor, deserializes
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EntityID(BinaryReader reader)
        {
            mIndex = 0;
            mGen = 0;

            Read(reader);
        }

        /// <summary>
        /// Check if entity is empty/invalid
        /// </summary>
        public bool isEmpty
        {
            get
            {
                if (index == 0 || gen < 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Get the entity index
        /// </summary>
        public int index
        {
            get
            {
                return mIndex;
            }
        }

        /// <summary>
        /// Get the entity generation
        /// </summary>
        public int gen
        {
            get
            {
                return mGen;
            }
        }

        /// <summary>
        /// Get the entity
        /// </summary>
        public Entity e
        {
            get
            {
                return EntityStore.Get(this);
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <param name="a">Entity A</param>
        /// <param name="b">Entity B</param>
        /// <returns>True if equal</returns>
        public static bool operator ==(EntityID a, EntityID b)
        {
            return a.mIndex == b.mIndex && a.mGen == b.mGen;
        }

        /// <summary>
        /// Inequality operator
        /// </summary>
        /// <param name="a">Entity A</param>
        /// <param name="b">Entity B</param>
        /// <returns>True if not equal</returns>
        public static bool operator !=(EntityID a, EntityID b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Increment generation
        /// </summary>
        public void NextGen()
        {
            mGen++;
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <param name="format">Format</param>
        /// <returns>String</returns>
        public string ToString(string format)
        {
            return string.Format("({0}:{1})", new object[] { this.mIndex.ToString(format), this.mGen.ToString(format) });
        }

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="other">Other object</param>
        /// <returns>True if equal</returns>
        public override bool Equals(object other)
        {
            bool result;

            if (!(other is EntityID))
            {
                result = false;
            }
            else
            {
                EntityID r = (EntityID)other;
                result = mIndex.Equals(r.mIndex) && mGen.Equals(r.mGen);
            }

            return result;
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns>Hashcode</returns>
        public override int GetHashCode()
        {
            return this.mIndex.GetHashCode() ^ this.mGen.GetHashCode() << 2;
        }

        /// <summary>
        /// Write entity id
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public void Write(BinaryWriter writer)
        {
            writer.Write(mIndex);
            writer.Write(mGen);
        }

        /// <summary>
        /// Read entity id
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public void Read(BinaryReader reader)
        {
            mIndex = reader.ReadInt32();
            mGen = reader.ReadInt32();
        }
    }
}
