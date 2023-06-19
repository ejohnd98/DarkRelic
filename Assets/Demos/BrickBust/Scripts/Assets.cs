namespace RetroBlitDemoBrickBust
{
    /// <summary>
    /// Contains all game assets
    /// </summary>
    public class Assets
    {
        /// <summary>
        /// Ball hits brick sound
        /// </summary>
        public AudioAsset soundHitBrick = new AudioAsset();

        /// <summary>
        /// Ball hits wall sound
        /// </summary>
        public AudioAsset soundHitWall = new AudioAsset();

        /// <summary>
        /// Ball "dies" sound
        /// </summary>
        public AudioAsset soundDeath = new AudioAsset();

        /// <summary>
        /// Brick explodes sound
        /// </summary>
        public AudioAsset soundExplode = new AudioAsset();

        /// <summary>
        /// Game started sound
        /// </summary>
        public AudioAsset soundStart = new AudioAsset();

        /// <summary>
        /// Powerup collected sound
        /// </summary>
        public AudioAsset soundPowerUp = new AudioAsset();

        /// <summary>
        /// Laser shot sound
        /// </summary>
        public AudioAsset soundLaserShot = new AudioAsset();

        /// <summary>
        /// Laser hit sound
        /// </summary>
        public AudioAsset soundLaserHit = new AudioAsset();

        /// <summary>
        /// Main menu music
        /// </summary>
        public AudioAsset musicMenu = new AudioAsset();

        /// <summary>
        /// Level music
        /// </summary>
        public AudioAsset musicLevel = new AudioAsset();

        /// <summary>
        /// Game sprite sheet
        /// </summary>
        public SpriteSheetAsset spriteSheet = new SpriteSheetAsset();

        /// <summary>
        /// Shader for drop shadows
        /// </summary>
        public ShaderAsset shaderShadow = new ShaderAsset();

        /// <summary>
        /// Load all assets
        /// </summary>
        public void LoadAll()
        {
            soundHitBrick.Load("BrickBust/Sounds/hit");
            soundHitWall.Load("BrickBust/Sounds/hit2");
            soundExplode.Load("BrickBust/Sounds/explode");
            soundDeath.Load("BrickBust/Sounds/death");
            soundStart.Load("BrickBust/Sounds/start");
            soundPowerUp.Load("BrickBust/Sounds/powerup");
            soundLaserShot.Load("BrickBust/Sounds/lasershot");
            soundLaserHit.Load("BrickBust/Sounds/laserhit");

            musicMenu.Load("BrickBust/Music/BossFight");
            musicLevel.Load("BrickBust/Music/Stage2");

            // You can load a spritesheet here
            spriteSheet.Load("BrickBust/Sprites");
            spriteSheet.grid = new SpriteGrid(new Vector2i(10, 10));
            RB.SpriteSheetSet(spriteSheet);

            shaderShadow.Load("BrickBust/DrawShaderShadow");
            shaderShadow.SpriteSheetTextureSet("_SpritesTexture", spriteSheet);
        }
    }
}
