namespace RetroBlitDemoRetroDungeoneer
{
    /// <summary>
    /// Type of scene
    /// </summary>
    public enum SceneEnum
    {
        /// <summary>
        /// Main menu scene
        /// </summary>
        MAIN_MENU,

        /// <summary>
        /// Game scene
        /// </summary>
        GAME
    }

    /// <summary>
    /// Generic scene definition
    /// </summary>
    public abstract class Scene
    {
        /// <summary>
        /// Enter the scene
        /// </summary>
        /// <param name="parameters">Scene parameters</param>
        public abstract void Enter(object parameters);

        /// <summary>
        /// Exit the scene
        /// </summary>
        public abstract void Exit();

        /// <summary>
        /// Update scene
        /// </summary>
        /// <returns>True if scene consumes the update and no further update should be propagated to parent scenes</returns>
        public abstract bool Update();

        /// <summary>
        /// Render the scene
        /// </summary>
        public abstract void Render();
    }
}
