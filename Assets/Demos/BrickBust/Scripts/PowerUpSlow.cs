namespace RetroBlitDemoBrickBust
{
    /// <summary>
    /// Slow powerup, slows down all balls in play to base speed
    /// </summary>
    public class PowerUpSlow : PowerUp
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        public PowerUpSlow(Vector2i pos) : base("S", Global.COLOR_GOLD_BRICK, pos)
        {
        }

        /// <summary>
        /// Activate the power up
        /// </summary>
        protected override void Activate()
        {
            base.Activate();

            var game = (BrickBustGame)RB.Game;
            var balls = game.Level.Balls;

            for (int i = 0; i < balls.Count; i++)
            {
                balls[i].ResetSpeed();
            }

            game.Level.Paddle.CancelPowerups();
        }
    }
}
