namespace RetroBlitDemoBrickBust
{
    /// <summary>
    /// Multiball power up, ball splits into 4 balls
    /// </summary>
    public class PowerUpMultiBall : PowerUp
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        public PowerUpMultiBall(Vector2i pos) : base("M", Global.COLOR_GREEN_BRICK, pos)
        {
        }

        /// <summary>
        /// Activate the power up
        /// </summary>
        protected override void Activate()
        {
            base.Activate();

            var game = (BrickBustGame)RB.Game;
            var level = game.Level;

            var balls = level.Balls;
            if (balls == null || balls.Count == 0)
            {
                return;
            }

            for (int i = 0; i < 3; i++)
            {
                var randomBall = balls[UnityEngine.Random.Range(0, balls.Count)];

                var ball = new Ball(new Vector2i((int)randomBall.Rect.center.x, (int)randomBall.Rect.center.y));

                bool goodAngle = false;
                int angle = 0;
                while (!goodAngle)
                {
                    angle = UnityEngine.Random.Range(0, 360);
                    if (angle % 90 >= 25 && angle % 90 <= 75)
                    {
                        goodAngle = true;
                    }
                }

                ball.Dir = UnityEngine.Quaternion.Euler(0, 0, angle) * new UnityEngine.Vector2(1, 0);
                ball.Speed = randomBall.Speed;

                ball.StuckToPaddle = randomBall.StuckToPaddle;

                balls.Add(ball);
            }
        }
    }
}
