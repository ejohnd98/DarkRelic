namespace RetroBlitDemoBrickBust
{
    using UnityEngine;

    /// <summary>
    /// Simple wall, ball bounces off it, that is pretty much it
    /// </summary>
    public class Wall : Collidable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rect">Rect that encloses the wall</param>
        public Wall(Rect2i rect)
        {
            Rect = rect;
        }

        /// <summary>
        /// Handle the hit
        /// </summary>
        /// <param name="collider">Who hit us</param>
        /// <param name="pos">Position if impact</param>
        /// <param name="velocity">Velocity at impact</param>
        public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
        {
            base.Hit(collider, pos, velocity);

            var game = (BrickBustGame)RB.Game;

            game.Level.Particles.Impact(pos, velocity, Global.COLOR_BLACK_BRICK);

            RB.SoundPlay(game.assets.soundHitWall, 1, Random.Range(0.9f, 1.1f));
        }
    }
}
