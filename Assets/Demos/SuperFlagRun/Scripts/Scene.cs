namespace RetroBlitDemoSuperFlagRun
{
    using UnityEngine;

    /// <summary>
    /// A simple game scene, supporting enter and exit transitions
    /// </summary>
    public class Scene
    {
        private TransitionState mTransitionState = TransitionState.DONE;
        private float mTransitionProgress = 0.0f;

        private enum TransitionState
        {
            DONE = 0,
            ENTERING = 1,
            EXITING = 2
        }

        /// <summary>
        /// Initialize
        /// </summary>
        /// <returns>True if successful</returns>
        public virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Called when the scene is entered
        /// </summary>
        public virtual void Enter()
        {
            mTransitionProgress = 0.0f;
            mTransitionState = TransitionState.ENTERING;
        }

        /// <summary>
        /// Called when the scene is exited
        /// </summary>
        public virtual void Exit()
        {
            mTransitionProgress = 0.0f;
            mTransitionState = TransitionState.EXITING;
        }

        /// <summary>
        /// Check if transition is done
        /// </summary>
        /// <returns>True if transition is done</returns>
        public bool TransitionDone()
        {
            return mTransitionState == TransitionState.DONE;
        }

        /// <summary>
        /// Update
        /// </summary>
        public virtual void Update()
        {
            if (mTransitionState == TransitionState.ENTERING || mTransitionState == TransitionState.EXITING)
            {
                mTransitionProgress += 0.025f;
                if (mTransitionProgress >= 1.0f)
                {
                    mTransitionState = TransitionState.DONE;
                }
            }
        }

        /// <summary>
        /// Render transition effect
        /// </summary>
        public virtual void Render()
        {
            RB.CameraReset();

            if (mTransitionState == TransitionState.ENTERING)
            {
                RB.EffectSet(RB.Effect.Pinhole, 1.0f - mTransitionProgress, new Vector2i(RB.DisplaySize.width / 2, RB.DisplaySize.height / 2), Color.black);
            }
            else if (mTransitionState == TransitionState.EXITING)
            {
                RB.EffectSet(RB.Effect.Pinhole, mTransitionProgress, new Vector2i(RB.DisplaySize.width / 2, RB.DisplaySize.height / 2), Color.black);
            }
            else
            {
                RB.EffectSet(RB.Effect.Pinhole, 0);
            }
        }
    }
}
