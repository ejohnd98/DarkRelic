namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Move trail component
    /// </summary>
    public class MoveTrail : EntityComponent
    {
        /// <summary>
        /// Trail. No need to persist this
        /// </summary>
        public List<Trail> trail = new List<Trail>();

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public MoveTrail(BinaryReader reader) : base(ComponentType.MoveTrail)
        {
            Read(reader);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public MoveTrail() : base(ComponentType.MoveTrail)
        {
        }

        /// <summary>
        /// Add new trail bit
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="fade">Fade</param>
        public void Add(Vector2i pos, int fade)
        {
            var t = new Trail();
            t.pos = pos;
            t.fade = (byte)fade;

            trail.Add(t);
        }

        /// <summary>
        /// Remove first trail bit
        /// </summary>
        public void RemoveFirst()
        {
            trail.RemoveAt(0);
        }

        /// <summary>
        /// Clear the trail
        /// </summary>
        public void Clear()
        {
            trail.Clear();
        }

        /// <summary>
        /// Serialize the component
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            // Write nothing, trail doesn't need to be persisted
        }

        /// <summary>
        /// De-serialize the component
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            trail.Clear();
        }

        /// <summary>
        /// Trail
        /// </summary>
        public class Trail
        {
            /// <summary>
            /// Position
            /// </summary>
            public Vector2i pos;

            /// <summary>
            /// Fade
            /// </summary>
            public byte fade;
        }
    }
}
