namespace RetroBlitDemoRetroDungeoneer
{
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Confusion effect
    /// </summary>
    public class EffectTeleport : Effect
    {
        private const int FADE_FRAMES = 10;
        private const float FLOAT_HEIGHT = 16;
        private const float FLOAT_SPEED = 0.2f;
        private const int PARTICLE_COUNT = 40;

        private Vector2 mDestPos;
        private Vector2 mSrcPos;
        private List<Particle> mParticles = new List<Particle>(PARTICLE_COUNT);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="srcPos">Source position</param>
        /// <param name="destPos">Destination position</param>
        public EffectTeleport(Vector2i srcPos, Vector2i destPos) : base(EffectType.Teleport)
        {
            var game = (RetroDungeoneerGame)RB.Game;

            mFramesTotal = 30;
            mFramesRemaining = mFramesTotal;
            renderOrder = RenderFunctions.RenderOrder.ACTOR_OVERLAY_EFFECTS;

            mSrcPos = new Vector2(srcPos.x * game.assets.spriteSheet.grid.cellSize.width, srcPos.y * game.assets.spriteSheet.grid.cellSize.height);
            mDestPos = new Vector2(destPos.x * game.assets.spriteSheet.grid.cellSize.width, destPos.y * game.assets.spriteSheet.grid.cellSize.height);

            for (int i = 0; i < PARTICLE_COUNT; i++)
            {
                mParticles.Add(NewParticle());
            }

            SoundBank.Instance.SoundPlayDelayed(game.assets.soundTeleport, 1.0f, RandomUtils.RandomPitch(0.1f), Random.Range(0, 5));
        }

        /// <summary>
        /// Constructor that deserializes from a reader
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public EffectTeleport(BinaryReader reader) : base(EffectType.Teleport)
        {
            Read(reader);
        }

        /// <summary>
        /// Update effect
        /// </summary>
        /// <param name="resultSet">Result set</param>
        public override void Update(ResultSet resultSet)
        {
            for (int i = 0; i < mParticles.Count; i++)
            {
                mParticles[i].pos.y -= mParticles[i].speed;
            }

            mFramesRemaining--;
        }

        /// <summary>
        /// Render the effect
        /// </summary>
        public override void Render()
        {
            var globalAlpha = mFramesRemaining <= FADE_FRAMES ? (mFramesRemaining / (float)FADE_FRAMES) : 1.0f;

            RB.AlphaSet((byte)(globalAlpha * 255));
            RB.TintColorSet(Color.white);

            for (int i = 0; i < mParticles.Count; i++)
            {
                RB.DrawPixel(mParticles[i].pos, mParticles[i].color);
            }

            RB.AlphaSet(255);
        }

        /// <summary>
        /// Serialize the effect
        /// </summary>
        /// <param name="writer">Writer instance</param>
        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(mSrcPos);
            writer.Write(mDestPos);

            writer.Write(mParticles.Count);
            for (int i = 0; i < mParticles.Count; i++)
            {
                writer.Write(mParticles[i].pos);
                writer.Write(mParticles[i].color);
                writer.Write(mParticles[i].speed);
            }
        }

        /// <summary>
        /// De-serialize the effect
        /// </summary>
        /// <param name="reader">Reader instance</param>
        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            mSrcPos = reader.ReadVector2();
            mDestPos = reader.ReadVector2();

            var count = reader.ReadInt32();
            mParticles.Clear();
            for (int i = 0; i < count; i++)
            {
                var particle = new Particle();
                particle.pos = reader.ReadVector2();
                particle.color = reader.ReadColor32();
                particle.speed = reader.ReadSingle();

                mParticles.Add(particle);
            }
        }

        private Particle NewParticle()
        {
            var game = (RetroDungeoneerGame)RB.Game;
            var p = new Particle();

            Vector2 pos = mSrcPos;
            if (Random.Range(0, 2) == 0)
            {
                pos = mDestPos;
            }

            p.pos = new Vector2(
                    Random.Range(pos.x, pos.x + game.assets.spriteSheet.grid.cellSize.width + 1),
                    Random.Range(pos.y, pos.y + game.assets.spriteSheet.grid.cellSize.height + 1));

            float inc = Random.Range(0.5f, 1.5f);
            int r = 0xc1;
            int g = 0x7a;
            int b = 0xd6;

            r = (int)(r * inc);
            g = (int)(g * inc);
            b = (int)(b * inc);

            if (r > 255)
            {
                r = 255;
            }

            if (g > 255)
            {
                g = 255;
            }

            if (b > 255)
            {
                b = 255;
            }

            p.color = new Color32((byte)r, (byte)g, (byte)b, 255);
            p.speed = Random.Range(FLOAT_SPEED, FLOAT_SPEED * 2);

            return p;
        }

        private class Particle
        {
            public Vector2 pos;
            public Color32 color;
            public float speed;
        }
    }
}
