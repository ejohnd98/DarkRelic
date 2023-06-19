namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// Player entity
    /// </summary>
    public class EntityPlayer : EntityMovable
    {
        private bool mJumpKeyDown = false;
        private int mPlayerNum;
        private int mDirX = 1;
        private float mFrameIndex = 0;
        private float mRunAnimSpeed = 0.125f;
        private int[] mRunFrames = new int[] { 0, 1, 0, 2 };
        private int mLastFrameIndex = 0;

        private EntityFlag mCarriedFlag = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pos">Position</param>
        /// <param name="playerNum">Player number of this player</param>
        public EntityPlayer(Vector2 pos, int playerNum) : base(pos)
        {
            mPlayerNum = playerNum;
            mColliderInfo.Rect = new Rect2i(2, 3, RB.SpriteSheetGet().grid.cellSize.width - 6, (RB.SpriteSheetGet().grid.cellSize.height * 2) - 4);

            if (playerNum == RB.PLAYER_ONE)
            {
                mDirX = 1;
            }
            else
            {
                mDirX = -1;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update()
        {
            var game = (SuperFlagRun)RB.Game;

            base.Update();
            if (Mathf.Abs(mPhysics.Velocity.x) > 0)
            {
                mFrameIndex += Mathf.Abs(mPhysics.Velocity.x) * mRunAnimSpeed;
                if (mFrameIndex >= mRunFrames.Length)
                {
                    mFrameIndex = 0;
                }
            }
            else
            {
                mFrameIndex = 0;
            }

            if (!mPhysics.IsOnGround)
            {
                mFrameIndex = 0;
            }

            int newFrameIndex = (int)mFrameIndex;
            if (mLastFrameIndex != newFrameIndex && (newFrameIndex == 1 || newFrameIndex == 3))
            {
                RB.SoundPlay(game.assets.soundFootStep, 0.35f, Random.Range(0.7f, 1.3f));
            }

            mLastFrameIndex = newFrameIndex;

            var scene = (SceneGame)game.CurrentScene;

            if (mCarriedFlag == null && scene.GetWinner() == 0)
            {
                EntityFlag flag = scene.GetEnemyFlag(mPlayerNum);

                if (flag.ColliderInfo.Rect.Offset(new Vector2i((int)flag.Pos.x, (int)flag.Pos.y)).Intersects(ColliderInfo.Rect.Offset(new Vector2i((int)Pos.x, (int)Pos.y))))
                {
                    mCarriedFlag = flag;
                    RB.SoundPlay(game.assets.soundPickupFlag);
                }
            }
            else if (mCarriedFlag != null)
            {
                if ((int)mFrameIndex == 1 || (int)mFrameIndex == 2)
                {
                    mCarriedFlag.Pos = new Vector2(Pos.x, Pos.y - 10);
                }
                else
                {
                    mCarriedFlag.Pos = new Vector2(Pos.x, Pos.y - 11);
                }

                if (scene.GetWinner() == 0)
                {
                    EntityFlagSlot flagSlot = scene.GetFlagSlot(mPlayerNum);
                    if (flagSlot.ColliderInfo.Rect.Offset(new Vector2i((int)flagSlot.Pos.x, (int)flagSlot.Pos.y)).Intersects(ColliderInfo.Rect.Offset(new Vector2i((int)Pos.x, (int)Pos.y))))
                    {
                        RB.SoundPlay(game.assets.soundDropFlag);
                        scene.SetWinner(mPlayerNum);
                        mCarriedFlag.Pos = flagSlot.Pos;
                        mCarriedFlag = null;
                    }
                }
            }
        }

        /// <summary>
        /// Render
        /// </summary>
        public override void Render()
        {
            int flip = mDirX == 1 ? 0 : RB.FLIP_H;

            int frame = 0;

            if (mPhysics.IsOnGround)
            {
                frame = mRunFrames[(int)mFrameIndex];
            }
            else
            {
                frame = 3;
            }

            if (mPlayerNum == RB.PLAYER_ONE)
            {
                RB.DrawSprite(RB.SpriteIndex(frame, 2), new Vector2i((int)Pos.x, (int)Pos.y), flip);
                RB.DrawSprite(RB.SpriteIndex(frame, 3), new Vector2i((int)Pos.x, (int)Pos.y + 16), flip);
            }
            else
            {
                RB.DrawSprite(RB.SpriteIndex(frame + 5, 2), new Vector2i((int)Pos.x, (int)Pos.y), flip);
                RB.DrawSprite(RB.SpriteIndex(frame + 5, 3), new Vector2i((int)Pos.x, (int)Pos.y + 16), flip);
            }

            base.Render();
        }

        /// <summary>
        /// Update velocity
        /// </summary>
        protected override void UpdateVelocity()
        {
            float movementSpeed = 0.01f;
            Vector2 movementForce = new Vector2();

            var game = (SuperFlagRun)RB.Game;
            var scene = (SceneGame)game.CurrentScene;

            if (scene.GetWinner() == 0)
            {
                if (RB.ButtonDown(RB.BTN_LEFT, mPlayerNum))
                {
                    movementForce.x -= movementSpeed;
                    mDirX = -1;
                }
                else if (RB.ButtonDown(RB.BTN_RIGHT, mPlayerNum))
                {
                    mDirX = 1;
                    movementForce.x += movementSpeed;
                }

                if (RB.ButtonDown(RB.BTN_ABXY, mPlayerNum))
                {
                    if (!mJumpKeyDown && mPhysics.IsOnGround)
                    {
                        mPhysics.Jump();
                        mJumpKeyDown = true;
                    }
                }

                if (!RB.ButtonDown(RB.BTN_ABXY, mPlayerNum))
                {
                    mJumpKeyDown = false;
                }
            }

            if (!mJumpKeyDown)
            {
                mPhysics.Velocity = new Vector2(mPhysics.Velocity.x, Mathf.Max(-1, mPhysics.Velocity.y));
            }

            mPhysics.AddForce(new Vector2(0, PlatformPhysics.GRAVITY));

            mPhysics.AddMovementForce(movementForce * mPhysics.MoveAccel);
        }
    }
}
