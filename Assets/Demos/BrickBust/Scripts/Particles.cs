namespace RetroBlitDemoBrickBust
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Simple particle system
    /// </summary>
    public class Particles
    {
        private List<Particle> mParticles = new List<Particle>();
        private Vector2 mGravity = new Vector2(0.0f, 0.05f);

        /// <summary>
        /// Create an explosion particle effect by dividing up a sprite into little chunks and giving them
        /// random velocity
        /// </summary>
        /// <param name="spriteRect">Sprite rect to divide up</param>
        /// <param name="pos">Position of the explosion</param>
        /// <param name="colorTint">Color tint to apply</param>
        public void Explode(Rect2i spriteRect, Vector2i pos, Color32 colorTint)
        {
            int particleSize = 2;

            for (int x = 0; x < spriteRect.width; x += particleSize)
            {
                for (int y = 0; y < spriteRect.height; y += particleSize)
                {
                    var particle = new Particle();
                    particle.Rect = new Rect2i(spriteRect.x + x, spriteRect.y + y, particleSize, particleSize);
                    particle.Pos = new Vector2(pos.x + x, pos.y + y);
                    particle.Velocity = new Vector2(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f));
                    particle.Velocity.Normalize();
                    particle.Velocity *= 1.0f;
                    particle.Life = 1.0f + UnityEngine.Random.Range(0.0f, 1.0f);
                    particle.ColorTint = colorTint;

                    mParticles.Add(particle);
                }
            }
        }

        /// <summary>
        /// Create an impact particle effect that sprays random particle from impact position
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="velocity">Velocity of impact</param>
        /// <param name="colorTint">Color swap to use</param>
        public void Impact(Vector2i pos, Vector2 velocity, Color32 colorTint)
        {
            int count = UnityEngine.Random.Range(0, 3) + 3;
            for (int i = 0; i < count; i++)
            {
                var particle = new Particle();
                particle.Rect = new Rect2i(140 + UnityEngine.Random.Range(0, 3), 0, 3, 3);
                particle.Pos = new Vector2(pos.x, pos.y);

                // Velocity of particle is opposite of velocity of impact on the X axis, and always upwards on Y axis
                // with some angle wiggle of +/- 15 degrees
                particle.Velocity = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-15, 15)) * (new Vector2(-velocity.x, -Mathf.Abs(velocity.y)));
                particle.Velocity.Normalize();
                particle.Velocity *= 1.0f;
                particle.Life = 1.75f + UnityEngine.Random.Range(0.0f, 1.5f);
                particle.ColorTint = colorTint;

                mParticles.Add(particle);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            for (int i = mParticles.Count - 1; i >= 0; i--)
            {
                var particle = mParticles[i];
                particle.Pos += particle.Velocity;
                particle.Velocity += mGravity;

                particle.Life -= 0.05f;

                if (particle.Life <= 0)
                {
                    mParticles.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            for (int i = 0; i < mParticles.Count; i++)
            {
                RB.TintColorSet(mParticles[i].ColorTint);

                float alpha = 1.0f;
                if (mParticles[i].Life < 0.5f)
                {
                    alpha = mParticles[i].Life / 0.5f;
                }

                byte prevAlpha = RB.AlphaGet();
                RB.AlphaSet((byte)(prevAlpha * alpha));

                RB.DrawCopy(mParticles[i].Rect, new Vector2i((int)mParticles[i].Pos.x, (int)mParticles[i].Pos.y));

                RB.AlphaSet(prevAlpha);

                RB.TintColorSet(Color.white);
            }
        }

        private class Particle
        {
            public Rect2i Rect;
            public Vector2 Pos;
            public Vector2 Velocity;
            public float MaxLife = 1.0f;
            public float Life = 1.0f;
            public Color32 ColorTint;
        }
    }
}