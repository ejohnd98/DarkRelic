namespace RetroBlitDemoBrickBust
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Standard issue brick, can be of varying colors, each color having different amount of hits required to
    /// destroy it
    /// </summary>
    public class Brick : Collidable
    {
        /// <summary>
        /// Hits lefts before this brick is destroyed
        /// </summary>
        public int Life = 1;

        /// <summary>
        /// Maximum hits the brick has
        /// </summary>
        public int MaxLife = 1;

        /// <summary>
        /// Score points awarded for destroying this brick
        /// </summary>
        public int Score;

        /// <summary>
        /// What type/color this brick is
        /// </summary>
        public BrickType Type;

        private float mAlpha;
        private Color32 ColorTint;
        private Rect2i LastSpriteRect;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position of the brick</param>
        /// <param name="type">Type of the brick</param>
        public Brick(Vector2i pos, BrickType type)
        {
            Rect = new Rect2i(pos.x, pos.y, Global.BRICK_WIDTH, Global.BRICK_HEIGHT);

            Type = type;

            switch (Type)
            {
                case BrickType.Blue:
                    ColorTint = Global.COLOR_BLUE_BRICK;
                    Life = 2;
                    Score = 20;
                    RequiredForWin = true;
                    break;

                case BrickType.Green:
                    ColorTint = Global.COLOR_GREEN_BRICK;
                    Life = 1;
                    Score = 10;
                    RequiredForWin = true;
                    break;

                case BrickType.Gold:
                    ColorTint = Global.COLOR_GOLD_BRICK;
                    Life = 5;
                    Score = 50;
                    RequiredForWin = true;
                    break;

                case BrickType.Brown:
                    ColorTint = Global.COLOR_BROWN_BRICK;
                    Life = 3;
                    Score = 30;
                    RequiredForWin = true;
                    break;

                case BrickType.Pink:
                    ColorTint = Global.COLOR_PINK_BRICK;
                    Life = 4;
                    Score = 40;
                    RequiredForWin = true;
                    break;

                case BrickType.Black:
                    ColorTint = Global.COLOR_BLACK_BRICK;
                    Life = 6;
                    Score = 60;
                    RequiredForWin = true;
                    break;

                case BrickType.Block:
                    ColorTint = Color.white;
                    Life = int.MaxValue;
                    Score = 0;
                    RequiredForWin = false;
                    break;
            }

            MaxLife = Life;

            mAlpha = UnityEngine.Random.Range(-0.5f, 0.0f);
        }

        /// <summary>
        /// Type of brick. We could instead create a Brick base class and inherit brick colors from it,
        /// but Bricks don't differ from each other much, and it would just create too many classes
        /// </summary>
        public enum BrickType
        {
            /// <summary>
            /// No brick at all
            /// </summary>
            None,

            /// <summary>
            /// Blue brick
            /// </summary>
            Blue,

            /// <summary>
            /// Green brick
            /// </summary>
            Green,

            /// <summary>
            /// Gold brick
            /// </summary>
            Gold,

            /// <summary>
            /// Brown brick
            /// </summary>
            Brown,

            /// <summary>
            /// Pink brick
            /// </summary>
            Pink,

            /// <summary>
            /// Black brick
            /// </summary>
            Black,

            /// <summary>
            /// Block brick
            /// </summary>
            Block
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            base.Update();

            mAlpha += 0.02f;
            if (mAlpha > 1.0f)
            {
                mAlpha = 1.0f;
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            if (mAlpha <= 0)
            {
                return;
            }

            RB.TintColorSet(ColorTint);

            byte prevAlpha = RB.AlphaGet();
            RB.AlphaSet((byte)(mAlpha * prevAlpha));

            int LifeFrame = 5 - (int)(((float)Life / (float)MaxLife) * 5);

            if (Type != BrickType.Block)
            {
                LastSpriteRect = new Rect2i(Global.BRICK_WIDTH * LifeFrame, 0, Rect.width, Rect.height);
            }
            else
            {
                LastSpriteRect = new Rect2i(120, 0, 20, 10);
            }

            RB.DrawCopy(LastSpriteRect, Rect);

            for (int i = 0; i < FlashFrame; i++)
            {
                RB.DrawRect(new Rect2i(Rect.x + i, Rect.y + i, Rect.width - (i * 2), Rect.height - (i * 2)), Color.white);
            }

            RB.TintColorSet(Color.white);

            RB.AlphaSet(prevAlpha);
        }

        /// <summary>
        /// Handle collision
        /// </summary>
        /// <param name="collider">Who hit us</param>
        /// <param name="pos">Position at impact</param>
        /// <param name="velocity">Velocity at impact</param>
        public override void Hit(Collidable collider, Vector2i pos, Vector2 velocity)
        {
            var game = (BrickBustGame)RB.Game;

            base.Hit(collider, pos, velocity);

            Life = Math.Max(0, Life - 1);

            if (collider is Ball)
            {
                RB.SoundPlay(game.assets.soundHitBrick, 1, UnityEngine.Random.Range(0.9f, 1.1f));
            }

            game.Level.Particles.Impact(pos, velocity, Global.COLOR_BLACK_BRICK);

            if (Life <= 0)
            {
                game.Level.Score += Score;
                game.Shake(0.3f);
                game.Level.Particles.Explode(LastSpriteRect, new Vector2i(Rect.x, Rect.y), ColorTint);

                RB.SoundPlay(game.assets.soundExplode, 1, UnityEngine.Random.Range(0.9f, 1.1f));
            }
        }
    }
}
