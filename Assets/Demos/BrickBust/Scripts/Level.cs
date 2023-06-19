namespace RetroBlitDemoBrickBust
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Defines a the current game level. Level object is disposed of when moving to next level, which is
    /// neater/easier than cleaning up and trying to reset the level.
    /// </summary>
    public class GameLevel
    {
        private static int mScore; // Static lets us retain score between levels
        private static int mLives;

        private bool mWon = false;
        private Paddle mPaddle = null;
        private int mHiScore = 0;
        private int mLevelIndex = 0;
        private GameHud mHud = new GameHud();
        private Particles mParticles = new Particles();
        private List<Brick> mBricks = new List<Brick>();
        private List<Wall> mWalls = new List<Wall>();
        private List<Ball> mBalls = new List<Ball>();
        private List<PowerUp> mPowerUps = new List<PowerUp>();
        private List<LaserShot> mLaserShots = new List<LaserShot>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="levelIndex">Index of the level</param>
        public GameLevel(int levelIndex)
        {
            var game = (BrickBustGame)RB.Game;

            mPaddle = new Paddle();

            SpawnBallAtPaddle();

            // Top wall
            AddWall(new Wall(new Rect2i(-100, -100, RB.DisplaySize.width + 200, 100 + ((game.assets.spriteSheet.grid.cellSize.height * 3) - 4))));

            // Left wall
            AddWall(new Wall(new Rect2i(-100, 0, 100 + (game.assets.spriteSheet.grid.cellSize.width - 4), RB.DisplaySize.height + 100)));

            // Right wall
            AddWall(new Wall(new Rect2i(RB.DisplaySize.width - (game.assets.spriteSheet.grid.cellSize.width - 4), 0, 100, RB.DisplaySize.height + 100)));

            mLevelIndex = levelIndex;
            SpawnBricks();

            MakeBackground();

            // Load hiscore
            if (UnityEngine.PlayerPrefs.HasKey("HiScore"))
            {
                mHiScore = UnityEngine.PlayerPrefs.GetInt("HiScore");
            }

            if (mHiScore < 100)
            {
                mHiScore = 100;
            }

            if (mLevelIndex == 0)
            {
                mScore = 0;
                mLives = 3;
            }
        }

        /// <summary>
        /// The index of this level
        /// </summary>
        public int Index
        {
            get { return mLevelIndex; }
        }

        /// <summary>
        /// "Won" flag
        /// </summary>
        public bool Won
        {
            get { return mWon; }
        }

        /// <summary>
        /// The bricks in the level
        /// </summary>
        public List<Brick> Bricks
        {
            get { return mBricks; }
        }

        /// <summary>
        /// The walls in the level
        /// </summary>
        public List<Wall> Walls
        {
            get { return mWalls; }
        }

        /// <summary>
        /// Get/Set remaining lives
        /// </summary>
        public int Lives
        {
            get { return mLives; }
            set { mLives = Math.Min(value, 10); }
        }

        /// <summary>
        /// The balls in the level, could be more than one if multiball powerup is picked up
        /// </summary>
        public List<Ball> Balls
        {
            get { return mBalls; }
        }

        /// <summary>
        /// The player paddle
        /// </summary>
        public Paddle Paddle
        {
            get { return mPaddle; }
        }

        /// <summary>
        /// The particle system
        /// </summary>
        public Particles Particles
        {
            get { return mParticles; }
        }

        /// <summary>
        /// The current score
        /// </summary>
        public int Score
        {
            get
            {
                return mScore;
            }

            set
            {
                mScore = value;
                if (mScore >= mHiScore)
                {
                    mHiScore = mScore;
                }
            }
        }

        /// <summary>
        /// The hiscore
        /// </summary>
        public int HiScore
        {
            get { return mHiScore; }
        }

        /// <summary>
        /// The HUD UI
        /// </summary>
        public GameHud Hud
        {
            get { return mHud; }
        }

        /// <summary>
        /// Spawn the bricks for this level, as defined by LevelDef
        /// </summary>
        public void SpawnBricks()
        {
            var levelInfo = LevelDef.GetLevel(mLevelIndex);
            if (levelInfo == null)
            {
                return;
            }

            int bx = 0;
            int by = 0;
            for (int x = 8; x + Global.BRICK_WIDTH < RB.DisplaySize.width; x += Global.BRICK_WIDTH)
            {
                for (int y = Global.BRICK_HEIGHT * 3; y < Global.BRICK_HEIGHT * 32; y += Global.BRICK_HEIGHT)
                {
                    var brickType = levelInfo.GetBrickAt(bx, by);

                    if (brickType != Brick.BrickType.None)
                    {
                        AddBrick(new Brick(new Vector2i(x, y), brickType));
                    }

                    by++;
                }

                bx++;
                by = 0;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            for (int i = mBricks.Count - 1; i >= 0; i--)
            {
                if (mBricks[i].Life == 0)
                {
                    var brickRect = mBricks[i].Rect;

                    mBricks.RemoveAt(i);
                    CheckWin();

                    // 1 in 3 chance of powerup dropping
                    if (UnityEngine.Random.Range(0, 3) == 0)
                    {
                        SpawnPowerUp(new Vector2i(brickRect.x, brickRect.y));
                    }
                }
                else
                {
                    mBricks[i].Update();
                }
            }

            for (int i = mBalls.Count - 1; i >= 0; i--)
            {
                mBalls[i].Update();
                if (mBalls[i].Dead)
                {
                    mBalls.RemoveAt(i);
                }
            }

            for (int i = mPowerUps.Count - 1; i >= 0; i--)
            {
                mPowerUps[i].Update();
                if (mPowerUps[i].Dead)
                {
                    mPowerUps.RemoveAt(i);
                }
            }

            for (int i = mLaserShots.Count - 1; i >= 0; i--)
            {
                mLaserShots[i].Update();
                if (mLaserShots[i].Dead)
                {
                    mLaserShots.RemoveAt(i);
                }
            }

            // No balls left in play! Lost a life
            if (!mWon && mBalls.Count == 0)
            {
                if (mLives > 0)
                {
                    mLives--;
                    SpawnBallAtPaddle();
                    Paddle.CancelPowerups();

                    SaveHiScore();
                }
                else
                {
                    // Game Over!
                    mHud.ShowGameOver();

                    SaveHiScore();
                }
            }

            mPaddle.Update();

            mParticles.Update();
            mHud.Update();
        }

        /// <summary>
        /// Render
        /// </summary>
        public void Render()
        {
            var game = (BrickBustGame)RB.Game;

            RB.DrawMapLayer(0);

            // Render in 2 passes, first pass renders shadows
            for (int renderPass = 0; renderPass < 2; renderPass++)
            {
                if (renderPass == 0)
                {
                    RB.ShaderSet(game.assets.shaderShadow);
                    game.assets.shaderShadow.ColorSet("_ShadowColor", new Color32(0, 0, 0, 96));
                    RB.CameraSet(new Vector2i(-4, -4));
                }
                else
                {
                    RB.TintColorSet(Color.white);
                    RB.ShaderReset();
                }

                for (int i = 0; i < mBricks.Count; i++)
                {
                    mBricks[i].Render();
                }

                mPaddle.Render();

                for (int i = 0; i < mBalls.Count; i++)
                {
                    mBalls[i].Render();
                }

                for (int i = 0; i < mPowerUps.Count; i++)
                {
                    mPowerUps[i].Render();
                }

                for (int i = 0; i < mLaserShots.Count; i++)
                {
                    mLaserShots[i].Render();
                }

                mParticles.Render();

                if (renderPass == 0)
                {
                    RB.AlphaSet(255);
                    RB.CameraReset();
                }
            }

            RB.DrawMapLayer(1, new Vector2i(-2, 0));

            mHud.Render();
        }

        /// <summary>
        /// Add a laser shot
        /// </summary>
        /// <param name="shot">Lasershot to add</param>
        public void AddShot(LaserShot shot)
        {
            mLaserShots.Add(shot);
        }

        /// <summary>
        /// Spawn new ball in the middle of the paddle
        /// </summary>
        private void SpawnBallAtPaddle()
        {
            mBalls.Add(new Ball(new Vector2i((int)mPaddle.Rect.center.x, (int)mPaddle.Rect.center.y - 12)));
        }

        /// <summary>
        /// Check if we won by checking if all required bricks are destroyed
        /// </summary>
        private void CheckWin()
        {
            for (int i = 0; i < mBricks.Count; i++)
            {
                if (mBricks[i].RequiredForWin)
                {
                    return;
                }
            }

            mWon = true;
            mHud.ShowWin();
            SaveHiScore();
        }

        /// <summary>
        /// Persist the hiscore
        /// </summary>
        private void SaveHiScore()
        {
            if (mScore >= mHiScore)
            {
                PlayerPrefs.SetInt("HiScore", mScore);
            }
        }

        /// <summary>
        /// Add a brick
        /// </summary>
        /// <param name="brick">Brick to add</param>
        private void AddBrick(Brick brick)
        {
            mBricks.Add(brick);
        }

        /// <summary>
        /// Add a wall
        /// </summary>
        /// <param name="wall">Wall to add</param>
        private void AddWall(Wall wall)
        {
            mWalls.Add(wall);
        }

        /// <summary>
        /// Spawn a random powerup, some power ups have a greater chance of being spawned than other
        /// </summary>
        /// <param name="pos">Position to spawn at</param>
        private void SpawnPowerUp(Vector2i pos)
        {
            PowerUp powerUp = null;
            int[] types = { 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 5, 5, 5 };
            int type = types[UnityEngine.Random.Range(0, types.Length)];

            switch (type)
            {
                case 0:
                    powerUp = new PowerUpExtraLife(pos);
                    break;

                case 1:
                    powerUp = new PowerUpMultiBall(pos);
                    break;

                case 2:
                    powerUp = new PowerUpExtend(pos);
                    break;

                case 3:
                    powerUp = new PowerUpCatch(pos);
                    break;

                case 4:
                    powerUp = new PowerUpLaser(pos);
                    break;

                case 5:
                    powerUp = new PowerUpSlow(pos);
                    break;
            }

            mPowerUps.Add(powerUp);
        }

        /// <summary>
        /// Create the background pattern using random background tiles, and wall frame tiles
        /// </summary>
        private void MakeBackground()
        {
            var game = (BrickBustGame)RB.Game;

            Vector2i topLeft = new Vector2i(0, 2);
            Vector2i topRight = new Vector2i(RB.DisplaySize.width / game.assets.spriteSheet.grid.cellSize.width, 2);
            Vector2i bottomLeft = new Vector2i(0, RB.DisplaySize.height / game.assets.spriteSheet.grid.cellSize.height);

            // Background
            for (int x = topLeft.x; x <= topRight.x; x++)
            {
                for (int y = topLeft.y + 1; y <= bottomLeft.y; y++)
                {
                    RB.MapSpriteSet(0, new Vector2i(x, y), RB.SpriteIndex(UnityEngine.Random.Range(0, 4), 3));
                }
            }

            // Frame
            // Horizontal
            for (int x = topLeft.x + 1; x < topRight.x; x++)
            {
                RB.MapSpriteSet(1, new Vector2i(x, topLeft.y), RB.SpriteIndex(4, 3), RB.ROT_90_CW);
            }

            // Vertical
            for (int y = topLeft.y + 1; y <= bottomLeft.y; y++)
            {
                RB.MapSpriteSet(1, new Vector2i(topLeft.x, y), RB.SpriteIndex(4, 3));
                RB.MapSpriteSet(1, new Vector2i(topRight.x, y), RB.SpriteIndex(4, 3));
            }

            // Corners
            RB.MapSpriteSet(1, topLeft, RB.SpriteIndex(6, 3));
            RB.MapSpriteSet(1, topRight, RB.SpriteIndex(6, 3), RB.FLIP_H);

            RB.MapLayerSpriteSheetSet(0, game.assets.spriteSheet);
            RB.MapLayerSpriteSheetSet(1, game.assets.spriteSheet);
        }
    }
}